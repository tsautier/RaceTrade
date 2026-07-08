using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Org.BouncyCastle.Tls;

namespace RaceTrade
{
    public partial class AddCbftp : Form
    {
        private string currentCbftpId;

        public AddCbftp()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            // Set RaceProfile_CheckBox as selected by default
            RaceProfile_CheckBox.Checked = true;

            // Attach event handlers for mutual exclusivity
            RaceProfile_CheckBox.CheckedChanged += RaceProfile_CheckBox_CheckedChanged;
            DistProfile_CheckBox.CheckedChanged += DistProfile_CheckBox_CheckedChanged;
         
        }


        public string ServerName => textBox_ServerName.Text.Trim();
        // Properties to get the values after the form is submitted
        // Password is ENCRYPTED when retrieved (for saving to config)
        public string Host => textBox1.Text;
        public string Port => textBox2.Text;
        public string Password => SecureConfig.Encrypt(textBox4.Text.Trim());
        public string Profile => RaceProfile_CheckBox.Checked ? "RACE" : "DISTRIBUTE";

        /// <summary>
        /// Ensures the cbftp directory and config file exist.
        /// Creates them with default empty structure if they don't exist.
        /// </summary>
        public static void EnsureConfigExists()
        {
            string directoryPath = "cbftp";
            string filePath = Path.Combine(directoryPath, "cbftp_config.json");

            try
            {
                // Create directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    LogManager.Info($"Created directory: {directoryPath}");
                }

                // Create file with empty config if it doesn't exist
                if (!File.Exists(filePath))
                {
                    var emptyConfig = new Config();
                    var emptyJson = JsonConvert.SerializeObject(emptyConfig, Formatting.Indented);
                    File.WriteAllText(filePath, emptyJson);
                    LogManager.Info($"Created configuration file: {filePath}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Failed to ensure config file exists");
                throw;
            }
        }

        /// <summary>
        /// Sets the configuration for editing an existing CBFTP server.
        /// Password is DECRYPTED when loaded (for display in textbox)
        /// </summary>
        public void SetCbftpConfig(string id, string host, string port, string password, string profile = "RACE", string serverName = "")
        {

            textBox_ServerName.Text = serverName;
            textBox1.Text = host;
            textBox2.Text = port;
            textBox4.Text = SecureConfig.Decrypt(password); // DECRYPT for display

            // Set profile checkboxes
            if (profile == "DISTRIBUTE")
            {
                DistProfile_CheckBox.Checked = true;
                RaceProfile_CheckBox.Checked = false;
            }
            else
            {
                RaceProfile_CheckBox.Checked = true;
                DistProfile_CheckBox.Checked = false;
            }

            currentCbftpId = string.IsNullOrEmpty(id) ? null : id;
        }

        /// <summary>
        /// Validates the form before saving.
        /// </summary>
        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (!FormValidation.ValidateNotEmpty(textBox_ServerName, "Server Name", out var nameError))
                errors.Add(nameError);

            if (!FormValidation.ValidateHostname(textBox1, out var hostError))
                errors.Add(hostError);

            if (!FormValidation.ValidatePort(textBox2, out var portError))
                errors.Add(portError);

            if (!FormValidation.ValidateNotEmpty(textBox4, "Password", out var pwdError))
                errors.Add(pwdError);

            if (!RaceProfile_CheckBox.Checked && !DistProfile_CheckBox.Checked)
            {
                errors.Add("Please select a profile: RACE or DISTRIBUTE.");
            }

            if (errors.Count > 0)
            {
                FormValidation.ShowValidationErrors(errors);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save button click handler.
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            // Ensure config directory and file exist before saving
            try
            {
                EnsureConfigExists();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Failed to create configuration directory/file");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Ensures mutual exclusivity between profile checkboxes.
        /// </summary>
        private void RaceProfile_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (RaceProfile_CheckBox.Checked)
            {
                DistProfile_CheckBox.Checked = false;
            }
        }

        /// <summary>
        /// Ensures mutual exclusivity between profile checkboxes.
        /// </summary>
        private void DistProfile_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DistProfile_CheckBox.Checked)
            {
                RaceProfile_CheckBox.Checked = false;
            }
        }

        /// <summary>
        /// Exit button click handler.
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Event to indicate deletion
        public event Action OnServerDeleted;


        /// <summary>
        /// Delete button click handler with confirmation.
        /// </summary>
        private void delete_cbftp_server_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentCbftpId))
            {
                DialogHelper.ShowWarning("No server information to delete.");
                return;
            }

            var result = DialogHelper.ShowDeleteConfirmation($"CBFTP server '{currentCbftpId}'");
            if (result != DialogResult.Yes)
                return;

            string directoryPath = "cbftp";
            string filePath = Path.Combine(directoryPath, "cbftp_config.json");

            try
            {
                // Ensure config exists before attempting to delete
                EnsureConfigExists();

                var jsonContent = File.ReadAllText(filePath);
                var config = JsonConvert.DeserializeObject<Config>(jsonContent) ?? new Config();
                var servers = config.CbftpServers ?? new List<CbftpServer>();

                var serverToDelete = servers.FirstOrDefault(s => s.Id == currentCbftpId);
                if (serverToDelete != null)
                {

                    string displayName = serverToDelete.Name ?? currentCbftpId;
                    servers.Remove(serverToDelete);
                    config.CbftpServers = servers;

                    var updatedJson = JsonConvert.SerializeObject(config, Formatting.Indented);
                    File.WriteAllText(filePath, updatedJson);

                    LogManager.Info($"CBFTP server '{serverToDelete.Name ?? currentCbftpId}' deleted successfully");
                    DialogHelper.ShowSuccess("CBFTP server successfully deleted.");

                    OnServerDeleted?.Invoke();
                    this.Close();
                }
                else
                {
                    DialogHelper.ShowError("Server not found in the configuration file.");
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Failed to delete CBFTP server");
                DialogHelper.ShowError(ex, "Failed to delete CBFTP server");
            }
        }

        private void ReloadDropdownInMainApp()
        {
            // Ensure MainApp is the active form or accessible instance
            if (Application.OpenForms["MainApp"] is MainApp mainApp)
            {
                mainApp.LoadConfigIntoDropdown();
            }
            else
            {
                MessageBox.Show("MainApp is not currently open.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

    // Configuration classes
    public class Config
    {
        [JsonProperty("cbftp_servers")]
        public List<CbftpServer> CbftpServers { get; set; } = new List<CbftpServer>();

        [JsonProperty("jobs")]
        public JobSettings Jobs { get; set; } = new JobSettings();
    }

    public class CbftpServer
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")] 
        public string Name { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("profile")]
        public string Profile { get; set; }
    }

    public class JobSettings
    {
        [JsonProperty("spreadjob")]
        public bool Spreadjob { get; set; }

        [JsonProperty("fxpjob")]
        public bool Fxpjob { get; set; }
    }
}