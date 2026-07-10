using Newtonsoft.Json;
using RaceTrade;
using RaceTrader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

public class ChatIrcClient
{
    private readonly string host;
    private readonly int port;
    private readonly string username;
    private readonly string password;
    private readonly string botName;
    private readonly string siteName;
    private List<string> channels;
    private readonly Action<string, string> callback;
    private readonly IrcLog logOutput;
    private readonly SiteConfig siteConfig;
    private readonly CancellationToken cancellationToken;

    // Blowfish for channels / PM.
    // Channel names and IRC nicks are case-insensitive, so these must be too.
    // ALL access (read and write) must be inside lock (fishLock).
    private readonly Dictionary<string, FishDecryptor> fishDecryptors = new Dictionary<string, FishDecryptor>(StringComparer.OrdinalIgnoreCase);

    // FiSH PM key handling
    private readonly Dictionary<string, string> pmFishKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DH1080> pendingKeyExchanges = new Dictionary<string, DH1080>(StringComparer.OrdinalIgnoreCase);

    // Tabbed IRC UI
    private TabbedIrcLog tabbedLogOutput;

    // User tracking
    private readonly Dictionary<string, HashSet<string>> channelUsers = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, HashSet<string>> pendingNamesLists = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
    private bool enableUserTracking = false;
    private readonly object userTrackingLock = new object();

    private SslStream currentSslStream;
    private bool chatOnlyMode = false;

    private readonly Dictionary<string, Dictionary<string, char>> channelUserPrefixes =
    new Dictionary<string, Dictionary<string, char>>(StringComparer.OrdinalIgnoreCase);

    private readonly object fishLock = new object();
    private volatile bool isDisconnecting = false;
    private CancellationTokenSource localCancellationTokenSource;
    private Task listeningTask;
    private TcpClient currentTcpClient;
    private readonly SemaphoreSlim _sendGate = new SemaphoreSlim(1, 1);
    private int _disconnecting = 0;
    private bool IsDisconnecting => Volatile.Read(ref _disconnecting) == 1;

    // NOTE: there used to be a separate _keyLock here. Writers took _keyLock (or no
    // lock at all) while readers took fishLock, so the two never excluded each other
    // and the dictionaries could be mutated mid-read. Everything now uses fishLock.

    public ChatIrcClient(SiteConfig config, string siteName, Action<string, string> callback, IrcLog logOutput, CancellationToken cancellationToken)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config), "Site configuration cannot be null.");

        this.siteConfig = config;
        this.siteName = siteName;
        this.callback = callback;
        this.logOutput = logOutput;
        this.cancellationToken = cancellationToken;

        // Server settings
        if (string.IsNullOrEmpty(config.Server?.Host))
            throw new ArgumentException("Host cannot be null or empty.");

        this.host = config.Server.Host;
        this.port = config.Server.Port > 0 ? config.Server.Port : 6667;

        if (string.IsNullOrEmpty(config.Server.Username))
            throw new ArgumentException("Username cannot be null or empty.");

        this.username = config.Server.Username;

        if (string.IsNullOrEmpty(config.Server.Password))
            throw new ArgumentException("Password cannot be null or empty.");

        this.password = SecureConfig.Decrypt(config.Server.Password);

        if (string.IsNullOrEmpty(config.SiteSettings?.BotName))
            throw new ArgumentException("Bot name cannot be null or empty.");

        this.botName = config.SiteSettings.BotName;

        // Join ALL configured channels (Chan1..Chan20), not just the first three.
        // The site editor supports 20 and LoadChannelKeys loads keys for all 20, so
        // anything past Chan3 was never joined and never decrypted.
        this.channels = new List<string>();
        for (int i = 1; i <= 20; i++)
        {
            var chanProp = config.SiteSettings.GetType().GetProperty($"Chan{i}");
            var chanValue = chanProp?.GetValue(config.SiteSettings) as string;

            if (string.IsNullOrWhiteSpace(chanValue))
                continue;

            chanValue = NormalizeKeyName(chanValue);

            if (!this.channels.Contains(chanValue, StringComparer.OrdinalIgnoreCase))
                this.channels.Add(chanValue);
        }

        LoadChannelKeys(config);
    }

    private void AppendOutput(string message, Color color)
    {
        if (logOutput != null && !logOutput.IsDisposed)
        {
            logOutput.AppendLog(message, color);
        }
    }

    /// <summary>
    /// Sets the tabbed log output for this IRC client
    /// </summary>
    public void SetTabbedLogOutput(TabbedIrcLog tabbedLog)
    {
        this.tabbedLogOutput = tabbedLog;
    }

    public void SetUserTrackingEnabled(bool enabled)
    {
        lock (userTrackingLock)
        {
            this.enableUserTracking = enabled;
        }

        if (MainApp.DebugEnabled)
        {
            AppendOutput($"[INFO] User tracking {(enabled ? "enabled" : "disabled")} for {siteName}", Color.Cyan);
        }

        // Request user list when enabling
        if (enabled && currentSslStream != null)
        {
            Task.Run(async () =>
            {
                try
                {
                    await RequestUserList();
                }
                catch (Exception ex)
                {
                    AppendOutput($"[ERROR] Failed to request user list: {ex.Message}", Color.Red);
                }
            });
        }
    }

    public void SetChatOnlyMode(bool chatOnly)
    {
        this.chatOnlyMode = chatOnly;

        if (chatOnly)
        {
            // Collect ALL channels (Chan1-3 + chat_keys)
            var allChannels = new List<string>
            {
                siteConfig.SiteSettings.Chan1,
                siteConfig.SiteSettings.Chan2,
                siteConfig.SiteSettings.Chan3
            }.Where(c => !string.IsNullOrEmpty(c)).ToList();

            // Add chat_keys channels
            if (siteConfig.SiteSettings.ChatKeys != null)
            {
                var chatChannels = siteConfig.SiteSettings.ChatKeys.Keys
                    .Where(k => k.StartsWith("#") && !allChannels.Contains(k))
                    .ToList();
                allChannels.AddRange(chatChannels);
            }

            this.channels = allChannels;

            AppendOutput($"[INFO] Chat-only mode: monitoring {allChannels.Count} channel(s): {string.Join(", ", allChannels)}",
                Color.Green);
        }

        if (MainApp.DebugEnabled)
        {
            AppendOutput($"[INFO] Chat-only mode {(chatOnly ? "enabled" : "disabled")} for {siteName}", Color.Cyan);
        }
    }

    public async Task RequestUserList()
    {
        if (currentSslStream == null || !IsHandleCreated(currentSslStream))
            return;

        try
        {
            foreach (var channel in channels)
            {
                await SendMessageAsync(currentSslStream, $"NAMES {channel}");
            }
        }
        catch (Exception ex)
        {
            AppendOutput($"[ERROR] Failed to request user list: {ex.Message}", Color.Red);
        }
    }

    private bool IsHandleCreated(SslStream stream)
    {
        try
        {
            return stream != null && stream.CanWrite;
        }
        catch
        {
            return false;
        }
    }

    private void LoadChannelKeys(SiteConfig config)
    {
        if (config?.SiteSettings == null)
        {
            AppendOutput("[WARN] Site configuration or settings are null, cannot load channel keys.", Color.Orange);
            return;
        }

        var siteSettings = config.SiteSettings;

        // 1) Load chan1..chan20 / blowfish_key1..20
        for (int i = 1; i <= 20; i++)
        {
            var chanProp = siteSettings.GetType().GetProperty($"Chan{i}");
            var keyProp = siteSettings.GetType().GetProperty($"BlowfishKey{i}");

            if (chanProp == null || keyProp == null)
                continue;

            var chanValue = chanProp.GetValue(siteSettings) as string;
            var encKey = keyProp.GetValue(siteSettings) as string;

            if (string.IsNullOrWhiteSpace(chanValue) || string.IsNullOrWhiteSpace(encKey))
                continue;

            // Normalize like SetChannelKey does, so a channel written without '#'
            // (or with different casing) still matches the server's channel name.
            chanValue = NormalizeKeyName(chanValue);

            try
            {
                var plainKey = SecureConfig.Decrypt(encKey);

                if (!string.IsNullOrWhiteSpace(plainKey))
                {
                    fishDecryptors[chanValue] = new FishDecryptor(plainKey);
                }
            }
            catch (Exception ex)
            {
                AppendOutput($"[ERROR] Failed to load Blowfish key for {chanValue}: {ex.Message}", Color.Red);
            }
        }

        // 2) Load chat-only keys from site_settings.chat_keys
        if (siteSettings.ChatKeys != null)
        {
            foreach (var kvp in siteSettings.ChatKeys)
            {
                var channel = kvp.Key;
                var encKey = kvp.Value;

                if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(encKey))
                    continue;

                try
                {
                    var plainKey = SecureConfig.Decrypt(encKey);

                    if (!string.IsNullOrWhiteSpace(plainKey))
                    {
                        fishDecryptors[channel] = new FishDecryptor(plainKey);
                    }
                }
                catch (Exception ex)
                {
                    AppendOutput($"[ERROR] Failed to load chat key for {channel}: {ex.Message}", Color.Red);
                }
            }
        }

        var channelKeys = fishDecryptors.Keys
            .Where(k => k.StartsWith("#"))
            .ToList();

        if (channelKeys.Any())
        {
            AppendOutput(
                $"[INFO] Blowfish decryption initialized for {channelKeys.Count} channel(s): {string.Join(", ", channelKeys)}",
                Color.Green);
        }
        else
        {
            AppendOutput("[WARN] No Blowfish keys configured for any channels. Messages will not be decrypted.", Color.Orange);
        }
    }

    public void SetChannelKey(string channel, string utf8Key, bool persist)
    {
        if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(utf8Key))
            return;

        channel = NormalizeKeyName(channel);

        try
        {
            lock (fishLock)
            {
                fishDecryptors[channel] = new FishDecryptor(utf8Key);
            }

            if (!persist)
                return;

            if (siteConfig.SiteSettings.ChatKeys == null)
                siteConfig.SiteSettings.ChatKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            var encKey = SecureConfig.Encrypt(utf8Key);
            siteConfig.SiteSettings.ChatKeys[channel] = encKey;

            var siteFile = Path.Combine("sites", $"{siteConfig.SiteSettings.Sitename}.json");
            if (!File.Exists(siteFile))
            {
                AppendOutput($"[WARN] Site file not found when saving key: {siteFile}", Color.Yellow);
                return;
            }

            var json = File.ReadAllText(siteFile);
            var fileConfig = JsonConvert.DeserializeObject<SiteConfig>(json) ?? new SiteConfig();
            if (fileConfig.SiteSettings == null)
                fileConfig.SiteSettings = new SiteSettings();
            if (fileConfig.SiteSettings.ChatKeys == null)
                fileConfig.SiteSettings.ChatKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            fileConfig.SiteSettings.ChatKeys[channel] = encKey;

            AtomicFile.WriteAllText(siteFile, JsonConvert.SerializeObject(fileConfig, Formatting.Indented));

            AppendOutput($"[FiSH] Saved key for {channel} to site config", Color.Green);
        }
        catch (Exception ex)
        {
            AppendOutput($"[ERROR] Failed to save channel key for {channel}: {ex.Message}", Color.Red);
        }
    }

    public string GetChannelKey(string channel)
    {
        if (string.IsNullOrWhiteSpace(channel) || siteConfig?.SiteSettings == null)
            return string.Empty;

        channel = NormalizeKeyName(channel);
        var siteSettings = siteConfig.SiteSettings;

        if (channel.StartsWith("PM:", StringComparison.OrdinalIgnoreCase))
        {
            string pmNick = channel.Substring(3);
            lock (fishLock)
            {
                return pmFishKeys.TryGetValue(pmNick, out var pmKey) ? pmKey : string.Empty;
            }
        }

        if (siteSettings.ChatKeys != null)
        {
            foreach (var kvp in siteSettings.ChatKeys)
            {
                if (!string.Equals(NormalizeKeyName(kvp.Key), channel, StringComparison.OrdinalIgnoreCase))
                    continue;

                return SecureConfig.Decrypt(kvp.Value) ?? string.Empty;
            }
        }

        for (int i = 1; i <= 20; i++)
        {
            var chanProp = siteSettings.GetType().GetProperty($"Chan{i}");
            var keyProp = siteSettings.GetType().GetProperty($"BlowfishKey{i}");
            var chanValue = chanProp?.GetValue(siteSettings) as string;

            if (!string.Equals(NormalizeKeyName(chanValue), channel, StringComparison.OrdinalIgnoreCase))
                continue;

            var encKey = keyProp?.GetValue(siteSettings) as string;
            return string.IsNullOrWhiteSpace(encKey) ? string.Empty : SecureConfig.Decrypt(encKey) ?? string.Empty;
        }

        return string.Empty;
    }

    private static string NormalizeKeyName(string channel)
    {
        if (string.IsNullOrWhiteSpace(channel))
            return string.Empty;

        channel = channel.Trim();

        if (!channel.StartsWith("#") && !channel.StartsWith("PM:", StringComparison.OrdinalIgnoreCase))
            channel = "#" + channel.TrimStart('#');

        return channel;
    }

    public void SetChannelBlowfishKey(string channel, string utf8Key)
    {
        if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(utf8Key))
            return;

        try
        {
            channel = NormalizeKeyName(channel);

            lock (fishLock)
            {
                fishDecryptors[channel] = new FishDecryptor(utf8Key);
            }

            AppendOutput($"[FiSH] Manual Blowfish key set for channel {channel}", Color.Yellow);
        }
        catch (Exception ex)
        {
            AppendOutput($"[ERROR] Failed to set Blowfish key for {channel}: {ex.Message}", Color.Red);
        }
    }

    private void SafeTabbedLogAction(Action<TabbedIrcLog> action)
    {
        bool trackingEnabled;
        lock (userTrackingLock)
        {
            trackingEnabled = enableUserTracking;
        }

        if (!trackingEnabled)
            return;

        if (tabbedLogOutput == null || tabbedLogOutput.IsDisposed)
            return;

        try
        {
            if (!tabbedLogOutput.IsHandleCreated)
                return;

            if (tabbedLogOutput.InvokeRequired)
            {
                tabbedLogOutput.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (!tabbedLogOutput.IsDisposed && tabbedLogOutput.IsHandleCreated)
                            action(tabbedLogOutput);
                    }
                    catch (ObjectDisposedException) { }
                    catch (InvalidOperationException) { }
                }));
            }
            else
            {
                if (!tabbedLogOutput.IsDisposed && tabbedLogOutput.IsHandleCreated)
                    action(tabbedLogOutput);
            }
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
    }

    public void Disconnect()
    {
        if (Interlocked.Exchange(ref _disconnecting, 1) == 1)
            return;

        try
        {
            lock (userTrackingLock)
            {
                enableUserTracking = false;
            }

            lock (userTrackingLock)
            {
                channelUsers.Clear();
                pendingNamesLists.Clear();
            }

            lock (userTrackingLock)
            {
                channelUserPrefixes.Clear();
            }

            try { localCancellationTokenSource?.Cancel(); } catch { }

            // Abort socket I/O to break ReadAsync
            try { currentTcpClient?.Close(); } catch { }

            // Do NOT wait on listeningTask here
            // Do NOT close/dispose currentSslStream here

            lock (fishLock)
            {
                fishDecryptors.Clear();
                pmFishKeys.Clear();
                pendingKeyExchanges.Clear();
            }

            LogManager.LogIRC(IRCEventType.Disconnection, $"Disconnect requested for {siteName}");
        }
        catch (Exception ex)
        {
            try { LogManager.LogIRC(IRCEventType.Error, $"Error during disconnect request: {ex.Message}"); } catch { }
        }
    }

    public async Task ConnectToZNCAsync()
    {

        Interlocked.Exchange(ref _disconnecting, 0);
        TcpClient tcpClient = null;
        SslStream sslStream = null;

        try
        {
            localCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(host, port);
            this.currentTcpClient = tcpClient; // ← STORE IT

            AppendOutput($"[INFO] Connected to IRC server: {host}:{port}", Color.Green);

            sslStream = new SslStream(tcpClient.GetStream(), false,
                (sender, certificate, chain, sslPolicyErrors) => true);

            this.currentSslStream = sslStream;

            await sslStream.AuthenticateAsClientAsync(host);
            AppendOutput("[INFO] SSL/TLS authentication successful.", Color.Green);

            await SendMessageAsync(sslStream, $"USER {username} 0 * :{username}");
            await SendMessageAsync(sslStream, $"NICK {username}");
            await SendMessageAsync(sslStream, $"PASS {password}");

            foreach (var channel in channels)
            {
                await SendMessageAsync(sslStream, $"JOIN {channel}");

                if (chatOnlyMode)
                {
                    AppendOutput($"[INFO] Joined channel for chat: {channel}", Color.Green);
                }
                else
                {
                    AppendOutput($"[INFO] Monitoring channel for chat: {channel}", Color.Cyan);
                }
            }

            listeningTask = ListenForMessagesAsync(sslStream);
            await listeningTask;
        }
        catch (OperationCanceledException)
        {
            AppendOutput($"Connection canceled for Site {siteName}.", Color.Orange);
        }
        catch (Exception ex)
        {
            AppendOutput($"Error connecting to IRC server for Site {siteName}: {ex.Message}", Color.Red);
        }
        finally
        {
            try { localCancellationTokenSource?.Cancel(); } catch { }

            var tcp = Interlocked.Exchange(ref currentTcpClient, null);
            try { tcp?.Close(); } catch { }

            var stream = Interlocked.Exchange(ref currentSslStream, null);
            try { stream?.Close(); } catch { }
            try { stream?.Dispose(); } catch { }

            var cts = Interlocked.Exchange(ref localCancellationTokenSource, null);
            try { cts?.Dispose(); } catch { }

            listeningTask = null;
        }
    }

    private async Task ListenForMessagesAsync(SslStream sslStream)
    {
        try
        {
            byte[] buffer = new byte[4096];
            var messageBuilder = new StringBuilder();

            while (!localCancellationTokenSource.Token.IsCancellationRequested)
            {
                int bytesRead = await sslStream.ReadAsync(buffer, 0, buffer.Length, localCancellationTokenSource.Token);
                if (bytesRead == 0) break;

                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                messageBuilder.Append(data);

                string messages = messageBuilder.ToString();
                string[] lines = messages.Split(new[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length - 1; i++)
                {
                    string line = lines[i];

                    if (line.StartsWith("PING"))
                    {
                        string pongResponse = line.Replace("PING", "PONG");
                        await SendMessageAsync(sslStream, pongResponse);
                        continue;
                    }

                    await ProcessIrcUserTracking(line);
                }

                messageBuilder.Clear();
                messageBuilder.Append(lines[lines.Length - 1]);
            }
        }
        catch (OperationCanceledException)
        {
            AppendOutput($"Listening loop canceled for Site {siteName}.", Color.Orange);
        }
        catch (Exception ex)
        {
            AppendOutput($"Error while listening for messages: {ex.Message}", Color.Red);
        }
    }

    private string NormalizeNick(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return raw;

        var nick = raw;

        int bangIdx = nick.IndexOf('!');
        if (bangIdx >= 0)
            nick = nick.Substring(0, bangIdx);

        int slashIdx = nick.IndexOf('/');
        if (slashIdx >= 0)
            nick = nick.Substring(0, slashIdx);

        return nick;
    }

    private async Task ProcessIrcUserTracking(string line)
    {
        bool trackingEnabled;
        lock (userTrackingLock)
        {
            trackingEnabled = enableUserTracking;
        }

        if (!trackingEnabled)
            return;

        if (tabbedLogOutput == null || tabbedLogOutput.IsDisposed || !tabbedLogOutput.IsHandleCreated)
            return;

        try
        {
            // JOIN
            var joinMatch = Regex.Match(line, @"^:(\S+)!\S+@\S+ JOIN :?(\S+)$");
            if (joinMatch.Success)
            {
                string rawUsername = joinMatch.Groups[1].Value;
                string username = NormalizeNick(rawUsername);
                string channel = joinMatch.Groups[2].Value;
                string thisNick = NormalizeNick(this.username);

                if (username.Equals(thisNick, StringComparison.OrdinalIgnoreCase))
                    return;

                if (!channelUsers.ContainsKey(channel))
                    channelUsers[channel] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (channelUsers[channel].Contains(username))
                    return;

                channelUsers[channel].Add(username);
                SafeTabbedLogAction(t => t.AddUser(siteName, channel, username));
                SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, channel, $"*** {username} has joined {channel}", Color.Gray));
                return;
            }

            // PART
            var partMatch = Regex.Match(line, @":(\S+)!\S+@\S+ PART (\S+)");
            if (partMatch.Success)
            {
                string rawUsername = partMatch.Groups[1].Value;
                string username = NormalizeNick(rawUsername);
                string channel = partMatch.Groups[2].Value;

                if (channelUserPrefixes.ContainsKey(channel))
                    channelUserPrefixes[channel].Remove(username);

                SafeTabbedLogAction(t => t.RemoveUser(siteName, channel, username));
                SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, channel, $"*** {username} has left {channel}", Color.Gray));
                return;
            }

            // QUIT
            var quitMatch = Regex.Match(line, @":(\S+)!\S+@\S+ QUIT");
            if (quitMatch.Success)
            {
                string rawUsername = quitMatch.Groups[1].Value;
                string username = NormalizeNick(rawUsername);

                foreach (var channel in channelUsers.Keys.ToList())
                {
                    if (channelUsers[channel].Contains(username))
                    {
                        channelUsers[channel].Remove(username);

                        if (channelUserPrefixes.ContainsKey(channel))
                            channelUserPrefixes[channel].Remove(username);
                        SafeTabbedLogAction(t => t.RemoveUser(siteName, channel, username));
                        SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, channel, $"*** {username} has quit", Color.Gray));
                    }
                }
                return;
            }

            // NICK
            var nickMatch = Regex.Match(line, @":(\S+)!\S+@\S+ NICK :?(\S+)");
            if (nickMatch.Success)
            {
                string rawOldNick = nickMatch.Groups[1].Value;
                string oldNick = NormalizeNick(rawOldNick);
                string newNick = NormalizeNick(nickMatch.Groups[2].Value);

                foreach (var channel in channelUsers.Keys.ToList())
                {
                    if (channelUsers[channel].Contains(oldNick))
                    {
                        // Update the main user list
                        channelUsers[channel].Remove(oldNick);
                        channelUsers[channel].Add(newNick);

                        // 🔹 Also move the prefix (~ & @ % +) from oldNick → newNick
                        if (channelUserPrefixes.TryGetValue(channel, out var prefixes) &&
                            prefixes.TryGetValue(oldNick, out var prefixChar))
                        {
                            prefixes.Remove(oldNick);
                            prefixes[newNick] = prefixChar;
                        }

                        // Update UI
                        SafeTabbedLogAction(t => t.RemoveUser(siteName, channel, oldNick));
                        SafeTabbedLogAction(t => t.AddUser(siteName, channel, newNick));
                        SafeTabbedLogAction(t => t?.AppendChannelMessage(
                            siteName,
                            channel,
                            $"*** {oldNick} is now known as {newNick}",
                            Color.Gray
                        ));
                    }
                }

                string oldPmKey = $"PM:{oldNick}";
                string newPmKey = $"PM:{newNick}";

                lock (fishLock)
                {
                    if (fishDecryptors.ContainsKey(oldPmKey))
                    {
                        fishDecryptors[newPmKey] = fishDecryptors[oldPmKey];
                        fishDecryptors.Remove(oldPmKey);
                    }

                    if (pmFishKeys.ContainsKey(oldNick))
                    {
                        pmFishKeys[newNick] = pmFishKeys[oldNick];
                        pmFishKeys.Remove(oldNick);
                    }

                    if (pendingKeyExchanges.ContainsKey(oldNick))
                    {
                        pendingKeyExchanges[newNick] = pendingKeyExchanges[oldNick];
                        pendingKeyExchanges.Remove(oldNick);
                    }
                }

                return;
            }

            // NAMES
            var namesMatch = Regex.Match(line, @":(\S+) 353 \S+ . (\S+) :(.+)");
            if (namesMatch.Success)
            {
                string channel = namesMatch.Groups[2].Value;
                string userList = namesMatch.Groups[3].Value;

                if (!pendingNamesLists.ContainsKey(channel))
                    pendingNamesLists[channel] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var users = userList.Split(' ').Where(u => !string.IsNullOrWhiteSpace(u)).ToList();

                foreach (var user in users)
                {
                    pendingNamesLists[channel].Add(user);
                }

                return;
            }

            // END OF NAMES (366)
            var endOfNamesMatch = Regex.Match(line, @":(\S+) 366 \S+ (\S+)");
            if (endOfNamesMatch.Success)
            {
                string channel = endOfNamesMatch.Groups[2].Value;

                if (pendingNamesLists.ContainsKey(channel))
                {
                    var allUsersRaw = pendingNamesLists[channel].ToList();

                    if (!channelUsers.ContainsKey(channel))
                        channelUsers[channel] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    if (!channelUserPrefixes.ContainsKey(channel))
                        channelUserPrefixes[channel] = new Dictionary<string, char>(StringComparer.OrdinalIgnoreCase);

                    channelUsers[channel].Clear();
                    channelUserPrefixes[channel].Clear();

                    foreach (var userRaw in allUsersRaw)
                    {
                        if (string.IsNullOrWhiteSpace(userRaw))
                            continue;

                        var u = userRaw.Trim();

                        char prefix = '\0';
                        if ("~&@%+".IndexOf(u[0]) >= 0)
                        {
                            prefix = u[0];
                            u = u.Substring(1); // strip prefix from display nick
                        }

                        string cleanUser = NormalizeNick(u);
                        channelUsers[channel].Add(cleanUser);

                        if (prefix != '\0')
                        {
                            channelUserPrefixes[channel][cleanUser] = prefix;
                        }
                    }

                    // still send the raw list (with prefixes) to the UI for the user list
                    SafeTabbedLogAction(t => t.UpdateUserList(siteName, channel, allUsersRaw));
                    pendingNamesLists.Remove(channel);
                }

                return;
            }

            // DH1080_INIT
            if (line.Contains("DH1080_INIT") && (line.Contains("PRIVMSG") || line.Contains("NOTICE")))
            {
                var dhMatch = Regex.Match(line, @":(\S+)!\S+@\S+ (PRIVMSG|NOTICE) (\S+) :DH1080_INIT (.+)");
                if (dhMatch.Success)
                {
                    string rawUsername = dhMatch.Groups[1].Value;
                    string username = NormalizeNick(rawUsername);
                    string theirPublicKeyRaw = dhMatch.Groups[4].Value.Trim();

                    string thisNick = NormalizeNick(this.username);
                    if (username.Equals(thisNick, StringComparison.OrdinalIgnoreCase))
                        return;

                    try
                    {
                        var dh = new DH1080();
                        string ourPublicKey = dh.GetPublicKey();
                        string sharedSecret = dh.ComputeSharedSecret(theirPublicKeyRaw);

                        lock (fishLock)
                        {
                            pmFishKeys[username] = sharedSecret;
                            fishDecryptors[$"PM:{username}"] = new FishDecryptor(sharedSecret);
                        }

                        await SendMessageAsync(currentSslStream, $"NOTICE {username} :DH1080_FINISH {ourPublicKey}");

                        SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}",
                            $"[FiSH] Received key exchange from {username}, replied with DH1080_FINISH", Color.Yellow));
                        SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}",
                            "[FiSH] ✓ Key exchange completed! Messages are now encrypted.", Color.Green));
                        SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}",
                            $"[FiSH] Key for {username} set to *censored* (CBC Mode)", Color.Green));
                    }
                    catch (Exception ex)
                    {
                        SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}", $"[ERROR] Failed to respond to key exchange: {ex.Message}", Color.Red));
                    }
                    return;
                }
            }

            // DH1080_FINISH
            if (line.Contains("DH1080_FINISH") && (line.Contains("PRIVMSG") || line.Contains("NOTICE")))
            {
                var dhMatch = Regex.Match(line, @":(\S+)!\S+@\S+ (PRIVMSG|NOTICE) (\S+) :DH1080_FINISH (.+)");
                if (dhMatch.Success)
                {
                    string rawUsername = dhMatch.Groups[1].Value;
                    string username = NormalizeNick(rawUsername);
                    string theirPublicKeyRaw = dhMatch.Groups[4].Value.Trim();

                    string thisNick = NormalizeNick(this.username);
                    if (username.Equals(thisNick, StringComparison.OrdinalIgnoreCase))
                        return;

                    DH1080 pendingDh;
                    lock (fishLock)
                    {
                        pendingKeyExchanges.TryGetValue(username, out pendingDh);
                    }

                    if (pendingDh != null)
                    {
                        try
                        {
                            string sharedSecret = pendingDh.ComputeSharedSecret(theirPublicKeyRaw);

                            // Mutate all three dictionaries under the one lock. Never call
                            // back into the UI while holding it.
                            lock (fishLock)
                            {
                                pmFishKeys[username] = sharedSecret;
                                fishDecryptors[$"PM:{username}"] = new FishDecryptor(sharedSecret);
                                pendingKeyExchanges.Remove(username);
                            }

                            SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}",
                                "[FiSH] ✓ Key exchange completed! Messages are now encrypted.", Color.Green));
                            SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}",
                                $"[FiSH] Key for {username} set to *censored* (CBC Mode)", Color.Green));
                        }
                        catch (Exception ex)
                        {
                            SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, $"PM:{username}", $"[ERROR] Failed to complete key exchange: {ex.Message}", Color.Red));
                        }
                    }
                    return;
                }
            }

            // PRIVMSG
            var privmsgMatch = Regex.Match(line, @":(\S+)!\S+@\S+ PRIVMSG (\S+) :(.+)");
            if (privmsgMatch.Success)
            {
                string rawUsername = privmsgMatch.Groups[1].Value;
                string username = NormalizeNick(rawUsername);
                string target = privmsgMatch.Groups[2].Value;
                string message = privmsgMatch.Groups[3].Value;

                string thisNick = NormalizeNick(this.username);
                if (username.Equals(thisNick, StringComparison.OrdinalIgnoreCase))
                    return;

                if (target.StartsWith("#"))
                {
                    Color msgColor = Color.White;
                    string decryptedMessage = message;

                    FishDecryptor decryptor = null;
                    lock (fishLock)
                    {
                        fishDecryptors.TryGetValue(target, out decryptor);
                    }

                    if (isDisconnecting)
                        return;

                    if (decryptor != null)
                    {
                        try
                        {
                            if (message.StartsWith("+OK ") || message.StartsWith("mcps"))
                            {
                                decryptedMessage = decryptor.DecryptMessage(message);
                                msgColor = Color.LightBlue;
                            }
                        }
                        catch
                        {
                            decryptedMessage = $"[DECRYPT ERROR] {message}";
                            msgColor = Color.Red;
                        }
                    }

                    // Get status prefix (if any)
                    string displayNick = username;
                    if (channelUserPrefixes.TryGetValue(target, out var prefixes) &&
                        prefixes.TryGetValue(username, out var prefixChar))
                    {
                        displayNick = prefixChar + username; // e.g. "@Finity"
                    }

                    SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, target, $"<{displayNick}> {decryptedMessage}", msgColor));
                }
                else
                {
                    string pmKey = $"PM:{username}";
                    string decryptedMessage = message;
                    Color msgColor = Color.Yellow;

                    // ⚠️ ADD LOCK HERE
                    FishDecryptor pmDecryptor = null;
                    lock (fishLock)
                    {
                        if (fishDecryptors.ContainsKey(pmKey))
                        {
                            fishDecryptors.TryGetValue(pmKey, out pmDecryptor);
                        }
                    }

                    if (isDisconnecting)
                        return;

                    if (pmDecryptor != null && (message.StartsWith("+OK ") || message.StartsWith("mcps")))
                    {
                        try
                        {
                            decryptedMessage = pmDecryptor.DecryptMessage(message);
                            msgColor = Color.LightBlue;
                        }
                        catch
                        {
                            decryptedMessage = $"[DECRYPT ERROR] {message}";
                            msgColor = Color.Red;
                        }
                    }

                    SafeTabbedLogAction(t => t?.AppendChannelMessage(siteName, pmKey,
                        $"<{username}> {decryptedMessage}", msgColor));
                }
            }
        }
        catch (ObjectDisposedException)
        {
            // Chat window was disposed, stop tracking silently
        }
        catch (InvalidOperationException)
        {
            // Handle was destroyed, stop tracking silently
        }
        catch (Exception ex)
        {
            if (!(ex is ObjectDisposedException) && !(ex is InvalidOperationException))
            {
                LogManager.LogIRC(IRCEventType.Error, $"User tracking error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Sends a message to a specific channel/PM (tabbed chat)
    /// </summary>
    public async Task SendChannelMessage(string channel, string message)
    {
        if (currentSslStream == null)
        {
            AppendOutput("[ERROR] Not connected to IRC server", Color.Red);
            return;
        }

        try
        {
            string target = channel;
            string keyName = channel;

            if (channel.StartsWith("PM:", StringComparison.OrdinalIgnoreCase))
            {
                target = channel.Substring(3);
                keyName = $"PM:{target}";
            }

            string messageToSend = message;

            // ⚠️ ADD LOCK HERE
            FishDecryptor decryptor = null;
            lock (fishLock)
            {
                fishDecryptors.TryGetValue(keyName, out decryptor);
            }

            if (isDisconnecting)
                return;

            if (decryptor != null)
            {
                try
                {
                    messageToSend = decryptor.EncryptMessage(message);
                }
                catch (Exception ex)
                {
                    tabbedLogOutput?.AppendChannelMessage(siteName, channel,
                        $"[ERROR] FiSH encryption failed: {ex.Message}", Color.Red);
                    return;
                }
            }
            else
            {
                tabbedLogOutput?.AppendChannelMessage(siteName, channel,
                    $"[DEBUG] No FiSH key found for {keyName}", Color.Orange);
            }

            await SendMessageAsync(currentSslStream, $"PRIVMSG {target} :{messageToSend}");

            // ... rest of code
        }
        catch (Exception ex)
        {
            AppendOutput($"[ERROR] Failed to send message: {ex.Message}", Color.Red);
        }
    }

    /// <summary>
    /// Initiates FiSH DH1080 key exchange with a user (chat)
    /// </summary>
    public async Task InitiateFishKeyExchange(string targetUser)
    {
        if (currentSslStream == null)
        {
            tabbedLogOutput?.AppendChannelMessage(siteName, $"PM:{targetUser}",
                "[ERROR] Not connected to IRC server", Color.Red);
            return;
        }

        try
        {
            var dh = new DH1080();
            string publicKey = dh.GetPublicKey();

            if (MainApp.DebugEnabled)
            {
                AppendOutput($"[DEBUG] Our public key being sent: {publicKey}", Color.Cyan);
                AppendOutput($"[DEBUG] Our public key length: {publicKey.Length}", Color.Cyan);
            }

            lock (fishLock)
            {
                pendingKeyExchanges[targetUser] = dh;
            }

            await SendMessageAsync(currentSslStream, $"NOTICE {targetUser} :DH1080_INIT {publicKey}");

            tabbedLogOutput?.AppendChannelMessage(siteName, $"PM:{targetUser}",
                "[FiSH] Sent DH1080_INIT, waiting for DH1080_FINISH...", Color.Yellow);
        }
        catch (Exception ex)
        {
            tabbedLogOutput?.AppendChannelMessage(siteName, $"PM:{targetUser}",
                $"[ERROR] FiSH key exchange failed: {ex.Message}", Color.Red);
        }
    }

    private async Task SendMessageAsync(SslStream sslStream, string message)
    {
        if (IsDisconnecting)
            return;

        if (sslStream == null)
            return;

        await _sendGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsDisconnecting)
                return;

            if (!sslStream.CanWrite)
                return;

            byte[] buffer = Encoding.UTF8.GetBytes(message + "\r\n");
            await sslStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            await sslStream.FlushAsync().ConfigureAwait(false);
        }
        catch (ObjectDisposedException) { }
        catch (IOException) { }
        catch (Exception ex)
        {
            if (!IsDisconnecting)
                AppendOutput($"Error sending message for Site {siteName}: {ex.Message}", Color.Red);
        }
        finally
        {
            _sendGate.Release();
        }
    }




}
