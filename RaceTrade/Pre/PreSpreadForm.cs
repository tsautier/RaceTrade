using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RaceTrade
{
    public partial class PreSpreadForm : AntdUI.Window
    {
        private List<PreCbftpServer> cbftpServers = new List<PreCbftpServer>();
        private List<PreSiteConfig> siteConfigs = new List<PreSiteConfig>();
        private List<string> releases = new List<string>();
        private string selectedRelease = null;
        private PreSiteConfig sourceSite = null;
        private List<PreSiteConfig> lastDistributedSites = new List<PreSiteConfig>();

        public PreSpreadForm()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);
            StylePreForm();
            LoadConfiguration();
        }

        /// <summary>
        /// Dashboard-matching visual polish for the Pre window (UI only): elevated
        /// card-style group boxes, semantic action buttons, accent titles and a
        /// subtle gradient background. No layout or logic changes.
        /// </summary>
        private void StylePreForm()
        {
            try
            {
                this.BackColor = ThemeManager.Colors.Background;

                // Group boxes read as elevated cards with cyan titles.
                foreach (var gb in new[] { groupBoxCbftpServers, groupBoxSitesConfig,
                                           groupBoxDistribution, groupBoxCompletionLog })
                {
                    if (gb == null) continue;
                    gb.BackColor = ThemeManager.Colors.Surface2;
                    gb.ForeColor = ThemeManager.Colors.AccentCyan;
                    gb.Font = new System.Drawing.Font(ThemeManager.Fonts.UiFamily, 9.5F,
                                                      System.Drawing.FontStyle.Bold);
                }
                if (panelSiteConfig != null)
                    panelSiteConfig.BackColor = ThemeManager.Colors.Surface2;

                // Rounded action buttons.
                foreach (var b in new[] {
                    btnAddCbftpServer, btnEditCbftpServer, btnRemoveCbftpServer, btnFetchAllSites,
                    btnSaveSiteConfig, btnSendPre, btnRefreshReleases, btnDistribute,
                    btnCheckCompletion, btnDeleteRelease, btnClearLog, btnSaveConfig })
                {
                    StylePreActionButton(b);
                }

                // Close button keeps a danger accent.
                if (btnClose != null)
                {
                    StylePreActionButton(btnClose);
                    ThemeManager.StyleDangerButton(btnClose);
                }
            }
            catch
            {
                // Styling must never break the Pre window.
            }
        }

        private static void StylePreActionButton(Button button)
        {
            if (button == null) return;

            ThemeManager.StyleActionButton(button);
            button.AutoSize = false;
            button.Padding = Padding.Empty;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.UseCompatibleTextRendering = false;
        }

        private void LoadConfiguration()
        {
            cbftpServers = PreSpreadConfigManager.LoadCbftpServers();
            siteConfigs = PreSpreadConfigManager.LoadSites();

            RefreshCbftpServersList();
            RefreshSitesList();
        }

        private void RefreshCbftpServersList()
        {
            listBoxCbftpServers.Items.Clear();
            foreach (var server in cbftpServers)
            {
                listBoxCbftpServers.Items.Add(server);
            }
        }

        private void RefreshSitesList()
        {
            checkedListBoxSites.Items.Clear();
            comboSourceSite.Items.Clear();
            checkedListBoxDestSites.Items.Clear();

            foreach (var site in siteConfigs.OrderBy(s => s.Name))
            {
                int index = checkedListBoxSites.Items.Add(site);
                checkedListBoxSites.SetItemChecked(index, site.Enabled);

                comboSourceSite.Items.Add(site);

                int destIndex = checkedListBoxDestSites.Items.Add(site);
                checkedListBoxDestSites.SetItemChecked(destIndex, site.Enabled);
            }

            if (comboSourceSite.Items.Count > 0)
                comboSourceSite.SelectedIndex = 0;
        }

        // CBFTP Server Management
        private void BtnAddCbftpServer_Click(object sender, EventArgs e)
        {
            using var dialog = new AddCbftp();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var newServer = new PreCbftpServer
                {
                    Id = Guid.NewGuid().ToString("N").Substring(0, 8),
                    Name = dialog.ServerName,
                    Host = dialog.Host,
                    Port = dialog.Port,
                    Password = dialog.Password,
                    Profile = dialog.Profile
                };

                cbftpServers.Add(newServer);
                PreSpreadConfigManager.SaveCbftpServers(cbftpServers);
                RefreshCbftpServersList();
                LogManager.Success($"Added CBFTP server: {newServer.Name}");
            }
        }

        private void BtnEditCbftpServer_Click(object sender, EventArgs e)
        {
            if (listBoxCbftpServers.SelectedItem == null)
            {
                MessageBox.Show("Please select a CBFTP server to edit", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedServer = (PreCbftpServer)listBoxCbftpServers.SelectedItem;

            using var dialog = new AddCbftp();
            dialog.SetCbftpConfig(
                selectedServer.Id,
                selectedServer.Host,
                selectedServer.Port,
                selectedServer.Password,
                selectedServer.Profile ?? "RACE",
                selectedServer.Name
            );

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                selectedServer.Name = dialog.ServerName;
                selectedServer.Host = dialog.Host;
                selectedServer.Port = dialog.Port;
                selectedServer.Password = dialog.Password;
                selectedServer.Profile = dialog.Profile;

                PreSpreadConfigManager.SaveCbftpServers(cbftpServers);
                RefreshCbftpServersList();
                LogManager.Success($"Updated CBFTP server: {selectedServer.Name}");
            }
        }

        private void BtnRemoveCbftpServer_Click(object sender, EventArgs e)
        {
            if (listBoxCbftpServers.SelectedItem == null)
            {
                MessageBox.Show("Please select a CBFTP server to remove", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedServer = (PreCbftpServer)listBoxCbftpServers.SelectedItem;

            var result = MessageBox.Show($"Remove CBFTP server '{selectedServer.Name}'?",
                "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                cbftpServers.Remove(selectedServer);
                PreSpreadConfigManager.SaveCbftpServers(cbftpServers);
                RefreshCbftpServersList();
                LogManager.Success($"Removed CBFTP server: {selectedServer.Name}");
            }
        }

        private async void BtnFetchAllSites_Click(object sender, EventArgs e)
        {
            if (!cbftpServers.Any())
            {
                MessageBox.Show("Please add at least one CBFTP server first", "No Servers",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnFetchAllSites.Enabled = false;
                progressBar.Visible = true;
                progressBar.Value = 0;
                progressBar.Maximum = cbftpServers.Count;

                lblStatus.Text = "Fetching sites from all CBFTP servers...";
                lblStatus.ForeColor = Color.Yellow;

                var newSites = new List<PreSiteConfig>();

                foreach (var server in cbftpServers)
                {
                    try
                    {
                        lblStatus.Text = $"Fetching sites from {server.Name}...";

                        string password = SecureConfig.Decrypt(server.Password);
                        var result = await CbftpSync.FetchSitesFromCbftp(server.Host, server.Port, password);

                        if (!result.IsSuccess)
                        {
                            LogManager.Error($"Error fetching from {server.Name}: {result.ErrorMessage}");
                            progressBar.Value++;
                            continue;
                        }

                        foreach (var cbftpSite in result.Sites)
                        {
                            if (!siteConfigs.Any(s => s.Name == cbftpSite.Name && s.CbftpServerId == server.Id))
                            {
                                newSites.Add(new PreSiteConfig
                                {
                                    Name = cbftpSite.Name,
                                    CbftpServerId = server.Id,
                                    AffilDirectory = "/pre",
                                    Section = "DEFAULT",
                                    Enabled = true
                                });
                            }
                        }

                        LogManager.Success($"Fetched {result.Sites.Count} sites from {server.Name}");
                        progressBar.Value++;
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error($"Error fetching from {server.Name}: {ex.Message}");
                        progressBar.Value++;
                    }
                }

                if (newSites.Any())
                {
                    siteConfigs.AddRange(newSites);
                    PreSpreadConfigManager.SaveSites(siteConfigs);
                    RefreshSitesList();
                    lblStatus.Text = $"Fetched {newSites.Count} new site(s)";
                    lblStatus.ForeColor = Color.LimeGreen;
                }
                else
                {
                    lblStatus.Text = "No new sites found";
                    lblStatus.ForeColor = Color.Yellow;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnFetchAllSites.Enabled = true;
                progressBar.Visible = false;
            }
        }

        // Site Configuration
        private void CheckedListBoxSites_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBoxSites.SelectedItem == null)
            {
                ClearSiteConfigPanel();
                return;
            }

            var selectedSite = (PreSiteConfig)checkedListBoxSites.SelectedItem;
            LoadSiteConfigPanel(selectedSite);
        }

        private void LoadSiteConfigPanel(PreSiteConfig site)
        {
            txtSiteName.Text = site.Name;
            txtAffilDirectory.Text = site.AffilDirectory;
            txtSection.Text = site.Section;

            comboCbftpServer.Items.Clear();
            foreach (var server in cbftpServers)
            {
                comboCbftpServer.Items.Add(server);
                if (server.Id == site.CbftpServerId)
                {
                    comboCbftpServer.SelectedItem = server;
                }
            }

            panelSiteConfig.Enabled = true;
        }

        private void ClearSiteConfigPanel()
        {
            txtSiteName.Clear();
            txtAffilDirectory.Clear();
            txtSection.Clear();
            comboCbftpServer.SelectedIndex = -1;
            panelSiteConfig.Enabled = false;
        }

        private void BtnSaveSiteConfig_Click(object sender, EventArgs e)
        {
            if (checkedListBoxSites.SelectedItem == null)
                return;

            var selectedSite = (PreSiteConfig)checkedListBoxSites.SelectedItem;

            selectedSite.AffilDirectory = txtAffilDirectory.Text.Trim();
            selectedSite.Section = txtSection.Text.Trim();

            if (comboCbftpServer.SelectedItem != null)
            {
                var server = (PreCbftpServer)comboCbftpServer.SelectedItem;
                selectedSite.CbftpServerId = server.Id;
            }

            PreSpreadConfigManager.SaveSites(siteConfigs);
            RefreshSitesList();
            LogManager.Success($"Saved configuration for site: {selectedSite.Name}");
        }

        private void CheckedListBoxSites_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var site = (PreSiteConfig)checkedListBoxSites.Items[e.Index];
            site.Enabled = e.NewValue == CheckState.Checked;
        }

        // Distribution
        private async void BtnRefreshReleases_Click(object sender, EventArgs e)
        {
            if (comboSourceSite.SelectedItem == null)
            {
                MessageBox.Show("Please select a source site first", "No Source Site",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sourceSite = (PreSiteConfig)comboSourceSite.SelectedItem;

            var server = cbftpServers.FirstOrDefault(s => s.Id == sourceSite.CbftpServerId);
            if (server == null)
            {
                MessageBox.Show("CBFTP server not found for this site", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnRefreshReleases.Enabled = false;
                lblStatus.Text = $"Listing {sourceSite.AffilDirectory} on {sourceSite.Name}...";
                lblStatus.ForeColor = Color.Yellow;

                releases = await ListRemoteDirectoryAsync(server, sourceSite.Name, sourceSite.AffilDirectory);

                listBoxReleases.Items.Clear();
                foreach (var release in releases)
                {
                    listBoxReleases.Items.Add(release);
                }

                lblStatus.Text = $"Found {releases.Count} release(s)";
                lblStatus.ForeColor = Color.LimeGreen;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnRefreshReleases.Enabled = true;
            }
        }

        private async Task<List<string>> ListRemoteDirectoryAsync(PreCbftpServer server, string siteName, string remotePath)
        {
            string endpoint = server.Host.Contains("://")
                ? (server.Host.EndsWith($":{server.Port}") ? server.Host : $"{server.Host}:{server.Port}")
                : $"https://{server.Host}:{server.Port}";

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };

            var password = SecureConfig.Decrypt(server.Password);
            var byteArray = Encoding.ASCII.GetBytes(":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            string apiUrl = $"{endpoint}/path?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(remotePath)}&timeout=30";

            var response = await client.GetAsync(apiUrl);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to list directory: {response.StatusCode}");
            }

            var directories = new List<string>();

            try
            {
                var listing = JArray.Parse(responseText);

                foreach (var item in listing)
                {
                    var obj = item as JObject;
                    if (obj == null) continue;

                    string itemType = obj["type"]?.ToString();
                    string itemName = obj["name"]?.ToString();

                    if (itemType == "DIR" && !string.IsNullOrEmpty(itemName))
                    {
                        if (itemName != "." && itemName != "..")
                        {
                            directories.Add(itemName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error parsing directory listing: {ex.Message}");
            }

            return directories;
        }

        private void ListBoxReleases_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedRelease = listBoxReleases.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedRelease))
            {
                string group = RaceHelper.ExtractGroupFromRelease(selectedRelease);
                lblSelectedRelease.Text = $"Selected: {selectedRelease}";
                lblSelectedGroup.Text = $"Group: {group}";

                btnDistribute.Enabled = true;
                btnCheckCompletion.Enabled = true;
                btnDeleteRelease.Enabled = true;
            }
            else
            {
                lblSelectedRelease.Text = "Selected: None";
                lblSelectedGroup.Text = "Group: None";
                btnDistribute.Enabled = false;
                btnCheckCompletion.Enabled = false;
                btnDeleteRelease.Enabled = false;
            }
        }

        private async void BtnDistribute_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRelease) || sourceSite == null)
            {
                MessageBox.Show("Please select a release", "No Release",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var destSites = new List<PreSiteConfig>();
            foreach (int index in checkedListBoxDestSites.CheckedIndices)
            {
                var site = (PreSiteConfig)checkedListBoxDestSites.Items[index];
                if (site.Name != sourceSite.Name)
                {
                    destSites.Add(site);
                }
            }

            if (!destSites.Any())
            {
                MessageBox.Show("Please check at least one destination site", "No Destinations",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnDistribute.Enabled = false;
                progressBar.Visible = true;
                progressBar.Value = 0;
                progressBar.Maximum = destSites.Count;

                lblStatus.Text = $"Distributing '{selectedRelease}' to {destSites.Count} site(s)...";
                lblStatus.ForeColor = Color.Yellow;

                await DistributeReleaseAsync(selectedRelease, sourceSite, destSites);

                lastDistributedSites = destSites.ToList();

                lblStatus.Text = $"Distribution complete";
                lblStatus.ForeColor = Color.LimeGreen;

                btnCheckCompletion.Enabled = true;
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnDistribute.Enabled = true;
                progressBar.Visible = false;
            }
        }

        private async Task DistributeReleaseAsync(string release, PreSiteConfig source, List<PreSiteConfig> destinations)
        {
            var sourceServer = cbftpServers.FirstOrDefault(s => s.Id == source.CbftpServerId);
            if (sourceServer == null)
            {
                throw new Exception($"Source CBFTP server not found");
            }

            string sourcePath = source.AffilDirectory.TrimEnd('/');

            foreach (var destSite in destinations)
            {
                try
                {
                    var destServer = cbftpServers.FirstOrDefault(s => s.Id == destSite.CbftpServerId);
                    if (destServer == null)
                    {
                        LogManager.Error($"CBFTP server not found for {destSite.Name}");
                        continue;
                    }

                    string destPath = destSite.AffilDirectory.TrimEnd('/');

                    var ok = await CbftpRacer.StartRequestTransferJob(
                        srcSite: source.Name,
                        dstSite: destSite.Name,
                        dstPath: destPath,
                        releaseName: release,
                        srcSectionOrPath: sourcePath,
                        srcIsSection: false
                    );

                    if (ok)
                    {
                        LogManager.Success($"Started FXP: {source.Name} → {destSite.Name}");
                        progressBar.Value++;
                    }
                    else
                    {
                        LogManager.Error($"Failed to start FXP to {destSite.Name}");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error distributing to {destSite.Name}: {ex.Message}");
                }
            }
        }

        // IMPROVED COMPLETION CHECK WITH FILE COMPARISON
        private async void BtnCheckCompletion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRelease))
                return;

            List<PreSiteConfig> sitesToCheck;

            if (lastDistributedSites.Any())
            {
                sitesToCheck = lastDistributedSites;
            }
            else
            {
                sitesToCheck = new List<PreSiteConfig>();
                foreach (int index in checkedListBoxDestSites.CheckedIndices)
                {
                    var site = (PreSiteConfig)checkedListBoxDestSites.Items[index];
                    if (site.Name != sourceSite?.Name)
                    {
                        sitesToCheck.Add(site);
                    }
                }
            }

            if (!sitesToCheck.Any())
            {
                MessageBox.Show("No sites to check", "No Sites",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnCheckCompletion.Enabled = false;
                lblStatus.Text = "Checking completion...";
                lblStatus.ForeColor = Color.Yellow;

                LogCompletion($"Starting completion check for '{selectedRelease}'", Color.Cyan);

                var results = await CheckCompletionAsync(selectedRelease, sitesToCheck);

                int complete = results.Count(r => r.Value);
                int total = results.Count;

                if (complete == total)
                {
                    lblStatus.Text = $"All {total} site(s) complete";
                    lblStatus.ForeColor = Color.LimeGreen;
                    btnSendPre.Enabled = true;
                    LogCompletion($"✓ All {total} site(s) complete!", Color.LimeGreen);
                }
                else
                {
                    lblStatus.Text = $"{complete}/{total} site(s) complete";
                    lblStatus.ForeColor = Color.Yellow;
                    LogCompletion($"⚠ {complete}/{total} site(s) complete", Color.Yellow);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                LogCompletion($"ERROR: {ex.Message}", Color.Red);
            }
            finally
            {
                btnCheckCompletion.Enabled = true;
            }
        }

        private class FileInfo
        {
            public string Name { get; set; }
            public long Size { get; set; }
            public bool IsDirectory { get; set; }
        }

        private async Task<Dictionary<string, bool>> CheckCompletionAsync(string release, List<PreSiteConfig> sites)
        {
            var results = new Dictionary<string, bool>();

            if (sourceSite == null)
            {
                LogCompletion("No source site configured", Color.Red);
                return results;
            }

            var sourceServer = cbftpServers.FirstOrDefault(s => s.Id == sourceSite.CbftpServerId);
            if (sourceServer == null)
            {
                LogCompletion("Source CBFTP server not found", Color.Red);
                return results;
            }

            try
            {
                string sourcePath = $"{sourceSite.AffilDirectory}/{release}";

                LogCompletion($"Listing files on source ({sourceSite.Name})...", Color.Yellow);

                var sourceFiles = await GetFileListRecursiveAsync(sourceServer, sourceSite.Name, sourcePath);

                if (!sourceFiles.Any())
                {
                    LogCompletion($"No files found on source", Color.Yellow);
                    foreach (var site in sites)
                    {
                        results[site.Name] = true;
                    }
                    return results;
                }

                LogCompletion($"Source: {sourceFiles.Count} files, {FormatBytes(sourceFiles.Sum(f => f.Size))}", Color.Cyan);

                foreach (var site in sites)
                {
                    try
                    {
                        var server = cbftpServers.FirstOrDefault(s => s.Id == site.CbftpServerId);
                        if (server == null)
                        {
                            LogCompletion($"{site.Name}: Server not found", Color.Red);
                            results[site.Name] = false;
                            continue;
                        }

                        string destPath = $"{site.AffilDirectory}/{release}";

                        LogCompletion($"Checking {site.Name}...", Color.Yellow);

                        bool dirExists = await CheckPathExistsAsync(server, site.Name, destPath);
                        if (!dirExists)
                        {
                            LogCompletion($"{site.Name}: ✗ Directory not found", Color.Red);
                            results[site.Name] = false;
                            continue;
                        }

                        var destFiles = await GetFileListRecursiveAsync(server, site.Name, destPath);

                        bool isComplete = CompareFileLists(sourceFiles, destFiles, site.Name);
                        results[site.Name] = isComplete;

                        if (isComplete)
                        {
                            LogCompletion($"{site.Name}: ✓ COMPLETE ({destFiles.Count} files, {FormatBytes(destFiles.Sum(f => f.Size))})", Color.LimeGreen);
                        }
                        else
                        {
                            int missing = sourceFiles.Count - destFiles.Count;
                            LogCompletion($"{site.Name}: ✗ INCOMPLETE ({destFiles.Count}/{sourceFiles.Count} files, {missing} missing)", Color.Red);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCompletion($"{site.Name}: Error - {ex.Message}", Color.Red);
                        results[site.Name] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCompletion($"ERROR: {ex.Message}", Color.Red);
            }

            return results;
        }

        private async Task<List<FileInfo>> GetFileListRecursiveAsync(PreCbftpServer server, string siteName, string remotePath)
        {
            var allFiles = new List<FileInfo>();

            try
            {
                var items = await GetDirectoryListingAsync(server, siteName, remotePath);

                foreach (var item in items)
                {
                    if (item.IsDirectory)
                    {
                        string subPath = $"{remotePath}/{item.Name}";
                        var subFiles = await GetFileListRecursiveAsync(server, siteName, subPath);

                        foreach (var subFile in subFiles)
                        {
                            subFile.Name = $"{item.Name}/{subFile.Name}";
                            allFiles.Add(subFile);
                        }
                    }
                    else
                    {
                        allFiles.Add(item);
                    }
                }
            }
            catch { }

            return allFiles;
        }

        private async Task<List<FileInfo>> GetDirectoryListingAsync(PreCbftpServer server, string siteName, string remotePath)
        {
            string endpoint = server.Host.Contains("://")
                ? (server.Host.EndsWith($":{server.Port}") ? server.Host : $"{server.Host}:{server.Port}")
                : $"https://{server.Host}:{server.Port}";

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };

            var password = SecureConfig.Decrypt(server.Password);
            var byteArray = Encoding.ASCII.GetBytes(":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            string apiUrl = $"{endpoint}/path?site={Uri.EscapeDataString(siteName)}&path={Uri.EscapeDataString(remotePath)}&timeout=30";

            var response = await client.GetAsync(apiUrl);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to list directory: {response.StatusCode}");
            }

            var files = new List<FileInfo>();

            try
            {
                var listing = JArray.Parse(responseText);

                foreach (var item in listing)
                {
                    var obj = item as JObject;
                    if (obj == null) continue;

                    string itemType = obj["type"]?.ToString();
                    string itemName = obj["name"]?.ToString();
                    long itemSize = obj["size"]?.ToObject<long>() ?? 0;

                    if (string.IsNullOrEmpty(itemName) || itemName == "." || itemName == "..")
                        continue;

                    files.Add(new FileInfo
                    {
                        Name = itemName,
                        Size = itemSize,
                        IsDirectory = itemType == "DIR"
                    });
                }
            }
            catch { }

            return files;
        }

        private bool CompareFileLists(List<FileInfo> sourceFiles, List<FileInfo> destFiles, string siteName)
        {
            var sourceFileNames = sourceFiles.Where(f => !f.IsDirectory).Select(f => f.Name).ToHashSet();
            var destFileNames = destFiles.Where(f => !f.IsDirectory).Select(f => f.Name).ToHashSet();

            var missingFiles = sourceFileNames.Except(destFileNames).ToList();

            if (missingFiles.Any())
            {
                if (MainApp.DebugEnabled)
                {
                    foreach (var missing in missingFiles.Take(5))
                    {
                        LogCompletion($"  Missing: {missing}", Color.Gray);
                    }
                    if (missingFiles.Count > 5)
                    {
                        LogCompletion($"  ... and {missingFiles.Count - 5} more", Color.Gray);
                    }
                }
                return false;
            }

            var destFilesDict = destFiles.Where(f => !f.IsDirectory).ToDictionary(f => f.Name, f => f.Size);

            foreach (var sourceFile in sourceFiles.Where(f => !f.IsDirectory))
            {
                if (destFilesDict.TryGetValue(sourceFile.Name, out long destSize))
                {
                    if (sourceFile.Size != destSize)
                    {
                        if (MainApp.DebugEnabled)
                        {
                            LogCompletion($"  Size mismatch: {sourceFile.Name}", Color.Gray);
                        }
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<bool> CheckPathExistsAsync(PreCbftpServer server, string siteName, string remotePath)
        {
            string endpoint = server.Host.Contains("://")
                ? (server.Host.EndsWith($":{server.Port}") ? server.Host : $"{server.Host}:{server.Port}")
                : $"https://{server.Host}:{server.Port}";

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            var password = SecureConfig.Decrypt(server.Password);
            var byteArray = Encoding.ASCII.GetBytes(":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var payload = new
            {
                command = $"CWD {remotePath}",
                sites = new[] { siteName }
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{endpoint}/raw", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return false;

                dynamic result = JsonConvert.DeserializeObject<dynamic>(responseText);

                if (result.successes != null && result.successes.Count > 0)
                {
                    string resultText = result.successes[0].result?.ToString() ?? "";
                    return resultText.Contains("250") || resultText.Contains("CWD command successful");
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        // DELETE RELEASE FUNCTIONALITY
        private async void BtnDeleteRelease_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRelease))
            {
                MessageBox.Show("Please select a release to delete", "No Release",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sitesToDelete = new List<PreSiteConfig>();
            foreach (int index in checkedListBoxDestSites.CheckedIndices)
            {
                var site = (PreSiteConfig)checkedListBoxDestSites.Items[index];
                sitesToDelete.Add(site);
            }

            if (!sitesToDelete.Any())
            {
                MessageBox.Show("Please check at least one site", "No Sites",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Delete '{selectedRelease}' from {sitesToDelete.Count} site(s)?\n\n" +
                string.Join("\n", sitesToDelete.Select(s => $"  • {s.Name}")) + "\n\n" +
                "This cannot be undone!",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnDeleteRelease.Enabled = false;
                lblStatus.Text = $"Deleting '{selectedRelease}'...";
                lblStatus.ForeColor = Color.Yellow;

                LogCompletion($"Deleting '{selectedRelease}' from {sitesToDelete.Count} site(s)", Color.Yellow);

                int deleted = 0;
                int failed = 0;

                foreach (var site in sitesToDelete)
                {
                    try
                    {
                        var server = cbftpServers.FirstOrDefault(s => s.Id == site.CbftpServerId);
                        if (server == null)
                        {
                            LogCompletion($"✗ {site.Name}: Server not found", Color.Red);
                            failed++;
                            continue;
                        }

                        string remotePath = $"{site.AffilDirectory}/{selectedRelease}";

                        bool success = await DeleteRemoteDirectoryAsync(server, site.Name, remotePath);

                        if (success)
                        {
                            LogCompletion($"✓ {site.Name}: Deleted", Color.LimeGreen);
                            deleted++;
                        }
                        else
                        {
                            LogCompletion($"✗ {site.Name}: Failed", Color.Red);
                            failed++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCompletion($"✗ {site.Name}: {ex.Message}", Color.Red);
                        failed++;
                    }
                }

                lblStatus.Text = $"Deletion: {deleted} succeeded, {failed} failed";
                lblStatus.ForeColor = deleted > 0 ? Color.LimeGreen : Color.Red;

                LogCompletion($"Deletion complete: {deleted} succeeded, {failed} failed", Color.Cyan);

                MessageBox.Show($"✓ Deleted: {deleted}\n✗ Failed: {failed}",
                    "Deletion Complete", MessageBoxButtons.OK,
                    deleted == sitesToDelete.Count ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
                LogCompletion($"ERROR: {ex.Message}", Color.Red);
            }
            finally
            {
                btnDeleteRelease.Enabled = true;
            }
        }

        private async Task<bool> DeleteRemoteDirectoryAsync(PreCbftpServer server, string siteName, string remotePath)
        {
            string endpoint = server.Host.Contains("://")
                ? (server.Host.EndsWith($":{server.Port}") ? server.Host : $"{server.Host}:{server.Port}")
                : $"https://{server.Host}:{server.Port}";

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

            var password = SecureConfig.Decrypt(server.Password);
            var byteArray = Encoding.ASCII.GetBytes(":" + password);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var payload = new
            {
                command = $"RMD {remotePath}",
                sites = new[] { siteName }
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync($"{endpoint}/raw", content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return false;

                dynamic result = JsonConvert.DeserializeObject<dynamic>(responseText);

                if (result.successes != null && result.successes.Count > 0)
                {
                    string resultText = result.successes[0].result?.ToString() ?? "";
                    return resultText.Contains("250") || resultText.Contains("RMD command successful");
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // SITE PRE
        private async void BtnSendPre_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedRelease))
            {
                MessageBox.Show("Please select a release", "No Release",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var preSites = new List<PreSiteConfig>();
            foreach (int index in checkedListBoxSites.CheckedIndices)
            {
                var site = (PreSiteConfig)checkedListBoxSites.Items[index];
                preSites.Add(site);
            }

            // If nothing is explicitly checked, PRE to ALL enabled sites "in one go".
            bool sendingToAll = !preSites.Any();
            if (sendingToAll)
            {
                preSites = siteConfigs.Where(s => s.Enabled).ToList();
            }

            if (!preSites.Any())
            {
                MessageBox.Show("No enabled sites are configured to PRE.", "No Sites",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string scope = sendingToAll
                ? $"ALL {preSites.Count} enabled site(s)"
                : $"{preSites.Count} selected site(s)";

            var result = MessageBox.Show(
                $"Send 'SITE PRE {selectedRelease}' to {scope}?\n\n" +
                "Each site uses its own configured section. This cannot be undone!",
                "Confirm PRE",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                btnSendPre.Enabled = false;
                lblStatus.Text = "Sending SITE PRE...";
                lblStatus.ForeColor = Color.Yellow;

                await SendPreSimultaneouslyAsync(selectedRelease, preSites);

                lblStatus.Text = $"SITE PRE sent to {preSites.Count} site(s)";
                lblStatus.ForeColor = Color.LimeGreen;

                MessageBox.Show($"SITE PRE sent to {preSites.Count} sites!",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
            finally
            {
                btnSendPre.Enabled = true;
            }
        }

        private async Task SendPreSimultaneouslyAsync(string release, List<PreSiteConfig> sites)
        {
            var sitesByServer = sites.GroupBy(s => s.CbftpServerId);
            var allTasks = new List<Task>();

            foreach (var serverGroup in sitesByServer)
            {
                var server = cbftpServers.FirstOrDefault(s => s.Id == serverGroup.Key);
                if (server == null)
                    continue;

                foreach (var site in serverGroup)
                {
                    string command = $"SITE PRE {release} {site.Section}";
                    allTasks.Add(SendRawCommandAsync(server, site.Name, command));
                }
            }

            await Task.WhenAll(allTasks);
        }

        private async Task SendRawCommandAsync(PreCbftpServer server, string siteName, string command)
        {
            try
            {
                string endpoint = server.Host.Contains("://")
                    ? (server.Host.EndsWith($":{server.Port}") ? server.Host : $"{server.Host}:{server.Port}")
                    : $"https://{server.Host}:{server.Port}";

                using var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };

                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

                var password = SecureConfig.Decrypt(server.Password);
                var byteArray = Encoding.ASCII.GetBytes(":" + password);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var payload = new
                {
                    command = command,
                    sites = new[] { siteName }
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{endpoint}/raw", content);

                if (response.IsSuccessStatusCode)
                {
                    LogManager.Success($"✓ {siteName}: {command}");
                }
                else
                {
                    LogManager.Error($"✗ {siteName}: Failed");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"✗ {siteName}: {ex.Message}");
            }
        }

        // COMPLETION LOG HELPERS
        private void LogCompletion(string message, Color color)
        {
            if (richTextBoxCompletionLog.InvokeRequired)
            {
                richTextBoxCompletionLog.Invoke(new Action(() => LogCompletion(message, color)));
                return;
            }

            richTextBoxCompletionLog.SelectionStart = richTextBoxCompletionLog.TextLength;
            richTextBoxCompletionLog.SelectionColor = color;
            richTextBoxCompletionLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            richTextBoxCompletionLog.ScrollToCaret();
        }

        private void BtnClearLog_Click(object sender, EventArgs e)
        {
            richTextBoxCompletionLog.Clear();
            LogCompletion("Log cleared", Color.Gray);
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                PreSpreadConfigManager.SaveCbftpServers(cbftpServers);
                PreSpreadConfigManager.SaveSites(siteConfigs);

                lblStatus.Text = "Configuration saved";
                lblStatus.ForeColor = Color.LimeGreen;

                LogManager.Success("Pre spread configuration saved");
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"Error: {ex.Message}";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
