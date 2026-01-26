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
    public partial class SettingsForm : Form
    {
        private const string SETTINGS_FILE = "settings/settings.json";
        public string AppName { get; private set; }
        public bool DebugEnabled { get; private set; }
        public bool AllowInsecureSsl { get; private set; }
        public bool DisableRaceLog { get; private set; }
        public bool DisableCbftpLog { get; private set; }
        public bool DisableAppLog { get; private set; }
        public bool DisableIrcLog { get; private set; }

        public SettingsForm()
        {
            InitializeComponent();
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

                    disableRaceLogCheckBox.Checked = settings["disable_race_log"]?.ToObject<bool>() ?? false;
                    disableCbftpLogCheckBox.Checked = settings["disable_cbftp_log"]?.ToObject<bool>() ?? false;
                    disableAppLogCheckBox.Checked = settings["disable_app_log"]?.ToObject<bool>() ?? false;

                    // keep properties in sync, in case caller reads them after closing
                    AppName = appNameTextBox.Text;
                    DebugEnabled = debugCheckBox.Checked;
                    AllowInsecureSsl = insecureSslCheckBox.Checked;

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
            statusLabel.Text = "Testing imdbapi.dev...";
            statusLabel.ForeColor = Color.Yellow;

            try
            {
                // Test the new FREE API 
                var testMovie = await IMDBHelper.LookupByImdb("tt0468569"); // The Dark Knight

                if (testMovie != null)
                {

                    var ratingText = testMovie.ImdbRating.HasValue
                    ? testMovie.ImdbRating.Value.ToString("F1")
    :               "N/A";
                    statusLabel.Text = $"✓ Success! Test: {testMovie.Title} ({testMovie.Year}) - Rating: {testMovie.ImdbRating}/10";
                    statusLabel.ForeColor = Color.LimeGreen;
                    MessageBox.Show(
                        $"Connection successful!\n\n" +
                        $"Test movie: {testMovie.Title} ({testMovie.Year})\n" +
                        $"Rating: {ratingText}/10\n" +
                        $"Votes: {testMovie.ImdbVotes:N0}\n\n" +
                        $"✅ imdbapi.dev is working!",
                        "Test Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    statusLabel.Text = "✗ Failed - Could not connect to imdbapi.dev";
                    statusLabel.ForeColor = Color.Red;
                    MessageBox.Show(
                        "Connection failed!\n\nPlease check your internet connection.",
                        "Test Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"✗ Error: {ex.Message}";
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

                var settings = new JObject
                {
                    
                    ["app_name"] = appName,
                    ["debug_enabled"] = debugCheckBox.Checked,
                    ["allow_insecure_ssl"] = insecureSslCheckBox.Checked,
                    ["disable_race_log"] = disableRaceLogCheckBox.Checked,
                    ["disable_cbftp_log"] = disableCbftpLogCheckBox.Checked,
                    ["disable_app_log"] = disableAppLogCheckBox.Checked,
                };

                File.WriteAllText(SETTINGS_FILE, settings.ToString(Formatting.Indented));

                AppName = appName;
                DebugEnabled = debugCheckBox.Checked;
                AllowInsecureSsl = insecureSslCheckBox.Checked;
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

        private void titleLabel_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}