using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaceTrade;

namespace RaceTrader
{
    public partial class SettingsForm : AntdUI.Window
    {
        private const string SETTINGS_FILE = "settings/settings.json";
        public string AppName { get; private set; }
        public bool DebugEnabled { get; private set; }
        public bool AllowInsecureSsl { get; private set; }
        public bool DisableRaceLog { get; private set; }
        public bool DisableCbftpLog { get; private set; }
        public bool DisableAppLog { get; private set; }
        public bool DisableIrcLog { get; private set; }
        public string TmdbApiKey { get; private set; }

        public SettingsForm()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                var settingsDir = Path.GetDirectoryName(SETTINGS_FILE);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                if (File.Exists(SETTINGS_FILE))
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    var settings = JObject.Parse(json);
        
                    appNameTextBox.Text = settings["app_name"]?.ToString()
                                          ?? "RaceTrader";
                    debugCheckBox.Checked = settings["debug_enabled"]?.ToObject<bool>() ?? false;
                    insecureSslCheckBox.Checked = settings["allow_insecure_ssl"]?.ToObject<bool>() ?? false;
                    tmdbApiKeyTextBox.Text = ReadSecretSetting(settings, "tmdb_api_key", "tmdb_key", "tmdb_bearer_token");

                    disableRaceLogCheckBox.Checked = settings["disable_race_log"]?.ToObject<bool>() ?? false;
                    disableCbftpLogCheckBox.Checked = settings["disable_cbftp_log"]?.ToObject<bool>() ?? false;
                    disableAppLogCheckBox.Checked = settings["disable_app_log"]?.ToObject<bool>() ?? false;

                    // keep properties in sync, in case caller reads them after closing
                    AppName = appNameTextBox.Text;
                    DebugEnabled = debugCheckBox.Checked;
                    AllowInsecureSsl = insecureSslCheckBox.Checked;
                    TmdbApiKey = tmdbApiKeyTextBox.Text;

                    DisableRaceLog = disableRaceLogCheckBox.Checked;
                    DisableCbftpLog = disableCbftpLogCheckBox.Checked;
                    DisableAppLog = disableAppLogCheckBox.Checked;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading settings: {ex.Message}");
            }
        }

        private async void TestButton_Click(object sender, EventArgs e)
        {
            testButton.Enabled = false;
            testButton.Text = "Testing...";
            statusLabel.Text = "Testing imdbapi.dev live + TMDb fallback...";
            statusLabel.ForeColor = Color.Yellow;

            try
            {
                // Race filtering uses title search, so test that live path instead
                // of proving only that a fixed IMDb ID exists in cache.
                var result = await IMDBHelper.SearchMovieDetailed("Back to the Future", 1985, 0);
                var testMovie = result.Movie;

                if (testMovie != null && testMovie.ImdbRating.HasValue)
                {

                    var ratingText = testMovie.ImdbRating.Value.ToString("F1");
                    statusLabel.Text = $"Success! Test: {testMovie.Title} ({testMovie.Year}) - Rating: {ratingText}/10";
                    statusLabel.ForeColor = Color.LimeGreen;
                    MessageBox.Show(
                        $"Live lookup successful.\n\n" +
                        $"Title-search test: {testMovie.Title} ({testMovie.Year})\n" +
                        $"Source: {result.Source}\n" +
                        $"Rating: {ratingText}/10\n" +
                        $"Votes: {testMovie.ImdbVotes:N0}\n\n" +
                        $"imdbapi.dev is working.",
                        "Test Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else if (testMovie != null)
                {
                    statusLabel.Text = "TMDb fallback found movie, IMDb rating unavailable";
                    statusLabel.ForeColor = Color.Orange;
                    MessageBox.Show(
                        $"TMDb fallback found the movie, but no IMDb rating was available.\n\n" +
                        $"Movie: {testMovie.Title} ({testMovie.Year})\n" +
                        $"IMDb ID: {testMovie.ImdbID ?? "N/A"}\n" +
                        $"Source: {result.Source}\n\n" +
                        $"{result.Message}\n\n" +
                        $"TMDb ratings are not used as IMDb ratings.",
                        "IMDb Rating Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    statusLabel.Text = "Failed - Could not connect to imdbapi.dev";
                    statusLabel.ForeColor = Color.Red;
                    MessageBox.Show(
                        $"Live title-search failed.\n\n{result.Message}\n\n" +
                        $"Race filters use title search, so cached IMDb ID tests are not enough. " +
                        $"Check internet/DNS, imdbapi.dev availability, and the TMDb key.",
                        "Test Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
                MessageBox.Show(
                    $"Test error:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                testButton.Enabled = true;
                testButton.Text = "Test Connection";
            }
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var appName = appNameTextBox.Text.Trim();

                if (string.IsNullOrEmpty(appName))
                {
                    appName = "RaceTrader";
                }

                var settingsDir = Path.GetDirectoryName(SETTINGS_FILE);
                if (!Directory.Exists(settingsDir))
                {
                    Directory.CreateDirectory(settingsDir);
                }

                // Load the existing file and only update OUR keys. Rebuilding the whole
                // object from scratch erased everything else stored here - most notably
                // the log-window layout and "dockLogsToMain" written by MainApp.
                JObject settings;
                try
                {
                    settings = File.Exists(SETTINGS_FILE)
                        ? JObject.Parse(File.ReadAllText(SETTINGS_FILE))
                        : new JObject();
                }
                catch
                {
                    settings = new JObject();
                }

                settings["app_name"] = appName;
                settings["debug_enabled"] = debugCheckBox.Checked;
                settings["allow_insecure_ssl"] = insecureSslCheckBox.Checked;
                settings["tmdb_api_key"] = WriteSecretSetting(tmdbApiKeyTextBox.Text.Trim());
                settings["disable_race_log"] = disableRaceLogCheckBox.Checked;
                settings["disable_cbftp_log"] = disableCbftpLogCheckBox.Checked;
                settings["disable_app_log"] = disableAppLogCheckBox.Checked;

                RaceTrade.AtomicFile.WriteAllText(SETTINGS_FILE, settings.ToString(Formatting.Indented));

                AppName = appName;
                DebugEnabled = debugCheckBox.Checked;
                AllowInsecureSsl = insecureSslCheckBox.Checked;
                TmdbApiKey = tmdbApiKeyTextBox.Text.Trim();
                DisableRaceLog = disableRaceLogCheckBox.Checked;
                DisableCbftpLog = disableCbftpLogCheckBox.Checked;
                DisableAppLog = disableAppLogCheckBox.Checked;

                LogManager.Success("Settings saved successfully");
                MessageBox.Show(
                    "Settings saved successfully!\n\nRestart the application to apply all changes.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error saving settings: {ex.Message}");
                MessageBox.Show(
                    $"Error saving settings:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static string ReadSecretSetting(JObject settings, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = settings[key]?.ToString();
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                try
                {
                    return SecureConfig.Decrypt(value);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error decrypting setting '{key}': {ex.Message}");
                    return string.Empty;
                }
            }

            return string.Empty;
        }

        private static string WriteSecretSetting(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : SecureConfig.EncryptIfNeeded(value);
        }

        private void getLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.themoviedb.org/settings/api",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error opening TMDb API key page: {ex.Message}");
            }
        }

        private void titleLabel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
