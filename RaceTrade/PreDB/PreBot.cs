using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RaceTrade;
using System.Linq;

namespace RaceTrade
{
    public partial class PreBot : AntdUI.Window
    {
        private string prebotFilePath;
        private PreBotConfig prebotConfig;

        public PreBot(string filePath = null)
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                // ADDING MODE - Hide dropdown, load specific file
                Edit_PreBot_comboBox.Visible = false;

                prebotFilePath = filePath;
                LoadPrebotConfig();

                string prebotName = prebotConfig?.SiteSettings?.Sitename ?? Path.GetFileNameWithoutExtension(filePath);
                this.Text = $"Add PreBot - {prebotName}";
            }
            else
            {
                // EDITING MODE - Show dropdown, load all PreBots
                Edit_PreBot_comboBox.Visible = true;
                LoadPreBotsIntoDropdown();

                prebotConfig = new PreBotConfig();
                this.Text = "Edit PreBot";
            }
        }


        private void LoadPreBotsIntoDropdown()
        {
            // Temporarily disable the event handler
            Edit_PreBot_comboBox.SelectedIndexChanged -= Edit_PreBot_comboBox_SelectedIndexChanged;

            Edit_PreBot_comboBox.Items.Clear();

            string preBotDirectory = "pre_bots";
            if (!Directory.Exists(preBotDirectory))
            {
                Edit_PreBot_comboBox.SelectedIndexChanged += Edit_PreBot_comboBox_SelectedIndexChanged;
                return;
            }

            var preBotFiles = Directory.GetFiles(preBotDirectory, "*.json")
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Where(name => !name.Equals("new_prebot", StringComparison.OrdinalIgnoreCase))
                .OrderBy(name => name)
                .ToList();

            foreach (var prebotName in preBotFiles)
            {
                Edit_PreBot_comboBox.Items.Add(prebotName);
            }

            if (Edit_PreBot_comboBox.Items.Count > 0)
            {
                Edit_PreBot_comboBox.SelectedIndex = 0;

                // MANUALLY load the first PreBot
                string firstPreBot = Edit_PreBot_comboBox.Items[0].ToString();
                string filePath = Path.Combine(preBotDirectory, $"{firstPreBot}.json");

                if (File.Exists(filePath))
                {
                    prebotFilePath = filePath;
                    LoadPrebotConfig();
                    this.Text = $"Edit PreBot - {firstPreBot}";
                }
            }

            // Re-enable the event handler
            Edit_PreBot_comboBox.SelectedIndexChanged += Edit_PreBot_comboBox_SelectedIndexChanged;
        }

        private void Edit_PreBot_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Edit_PreBot_comboBox.SelectedItem == null)
                return;

            string selectedPreBot = Edit_PreBot_comboBox.SelectedItem.ToString();
            string filePath = Path.Combine("pre_bots", $"{selectedPreBot}.json");

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"PreBot configuration file not found.",
                    "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // CRITICAL: Load the config!
            prebotFilePath = filePath;
            LoadPrebotConfig();  // load the data into the form fields
            this.Text = $"Edit PreBot - {selectedPreBot}";
        }

        private void LoadPrebotConfig()
        {
            try
            {
                string jsonContent = File.ReadAllText(prebotFilePath);
                prebotConfig = JsonConvert.DeserializeObject<PreBotConfig>(jsonContent) ?? new PreBotConfig();

                // Populate fields
                textBoxHost.Text = prebotConfig.ZncServer?.Host ?? string.Empty;
                textBoxPort.Text = prebotConfig.ZncServer?.Port.ToString() ?? string.Empty;
                textBoxUsername.Text = prebotConfig.ZncServer?.Username ?? string.Empty;
                textBoxPassword.Text = SecureConfig.Decrypt(prebotConfig.ZncServer?.Password ?? string.Empty);
                textBoxSitename.Text = prebotConfig.SiteSettings?.Sitename ?? string.Empty;
                textBoxBotName.Text = prebotConfig.SiteSettings?.BotName ?? string.Empty;
                textBoxChannel1.Text = prebotConfig.SiteSettings?.Channel1 ?? string.Empty;
                textBoxBlowfishKey1.Text = SecureConfig.Decrypt(prebotConfig.SiteSettings?.BlowfishKey1 ?? string.Empty);
                textBoxSectionRegex.Text = prebotConfig.SiteSettings?.SectionRegex ?? string.Empty;
                textBoxSectionPrefix.Text = prebotConfig.SiteSettings?.SectionPrefix ?? string.Empty;
                textBoxSectionSuffix.Text = prebotConfig.SiteSettings?.SectionSuffix ?? string.Empty;
                textBoxNameRegex.Text = prebotConfig.SiteSettings?.NameRegex ?? string.Empty;
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Failed to load PreBot configuration");
            }
        }

        private void SavePrebotConfig()
        {
            // Validate form first
            if (!ValidateForm())
                return;

            try
            {
                string plainPassword = textBoxPassword.Text.Trim();

                prebotConfig = new PreBotConfig
                {
                    ZncServer = new ZncServerSettings
                    {
                        Host = textBoxHost.Text.Trim(),
                        Port = int.Parse(textBoxPort.Text.Trim()),
                        Username = textBoxUsername.Text.Trim(),
                        Password = string.IsNullOrWhiteSpace(plainPassword) ? string.Empty : SecureConfig.Encrypt(plainPassword)
                    },
                    SiteSettings = new PreBotSiteSettings
                    {
                        Sitename = textBoxSitename.Text.Trim(),
                        BotName = textBoxBotName.Text.Trim(),
                        Channel1 = textBoxChannel1.Text.Trim(),
                        BlowfishKey1 = SecureConfig.Encrypt(textBoxBlowfishKey1.Text.Trim()),
                        SectionRegex = textBoxSectionRegex.Text.Trim(),
                        SectionPrefix = textBoxSectionPrefix.Text.Trim(),
                        SectionSuffix = textBoxSectionSuffix.Text.Trim(),
                        NameRegex = textBoxNameRegex.Text.Trim()
                    }
                };

                string sitename = prebotConfig.SiteSettings.Sitename?.Trim();

                if (string.IsNullOrWhiteSpace(sitename))
                {
                    DialogHelper.ShowError("Site name cannot be empty.");
                    return;
                }

                string directory = "pre_bots";
                string newFilePath = Path.Combine(directory, $"{sitename}.json");

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string oldFilePath = prebotFilePath;
                bool isRename = !string.IsNullOrEmpty(oldFilePath) &&
                               !oldFilePath.Equals(newFilePath, StringComparison.OrdinalIgnoreCase);

                string jsonContent = JsonConvert.SerializeObject(prebotConfig, Formatting.Indented);
                AtomicFile.WriteAllText(newFilePath, jsonContent);

                if (isRename && File.Exists(oldFilePath))
                {
                    try
                    {
                        File.Delete(oldFilePath);
                        LogManager.Info($"Deleted old PreBot file: {Path.GetFileName(oldFilePath)}");
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning($"Could not delete old file: {ex.Message}");
                    }
                }

                DialogHelper.ShowSuccess($"Configuration saved successfully as '{Path.GetFileName(newFilePath)}'!");
                LogManager.Success($"Saved PreBot: {sitename}");

                prebotFilePath = newFilePath;
                this.Text = $"Edit PreBot - {sitename}";

                // Only refresh dropdown if it's visible (EDIT mode)
                if (Edit_PreBot_comboBox.Visible)
                {
                    LoadPreBotsIntoDropdown();
                    if (Edit_PreBot_comboBox.Items.Contains(sitename))
                    {
                        Edit_PreBot_comboBox.SelectedItem = sitename;
                    }
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Failed to save PreBot configuration");
                LogManager.Error($"Error saving PreBot: {ex.Message}");
            }
        }

        private bool ValidateForm()
        {
            var errors = new System.Collections.Generic.List<string>();

            if (!FormValidation.ValidateNotEmpty(textBoxHost, "Host", out var hostError))
                errors.Add(hostError);

            if (!FormValidation.ValidatePort(textBoxPort, out var portError))
                errors.Add(portError);

            if (!FormValidation.ValidateNotEmpty(textBoxUsername, "Username", out var userError))
                errors.Add(userError);

            //if (!FormValidation.ValidateNotEmpty(textBoxPassword, "Password", out var pwdError))
            //    errors.Add(pwdError);

            if (!FormValidation.ValidateNotEmpty(textBoxSitename, "Site Name", out var siteError))
                errors.Add(siteError);

            if (!FormValidation.ValidateRegex(textBoxSectionRegex, "Section Regex", out var sectionError))
                errors.Add(sectionError);

            if (!FormValidation.ValidateRegex(textBoxNameRegex, "Release Name Regex", out var nameError))
                errors.Add(nameError);

            if (errors.Count > 0)
            {
                FormValidation.ShowValidationErrors(errors);
                return false;
            }

            return true;
        }

        private void DeletePrebotConfig()
        {
            if (string.IsNullOrEmpty(prebotFilePath) || !File.Exists(prebotFilePath))
            {
                DialogHelper.ShowWarning("No configuration file to delete or file already deleted.");
                this.Close();
                return;
            }

            string prebotName = prebotConfig?.SiteSettings?.Sitename ?? Path.GetFileNameWithoutExtension(prebotFilePath);

            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete the PreBot '{prebotName}'?\n\n" +
                "This action cannot be undone!",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(prebotFilePath);
                    DialogHelper.ShowSuccess($"PreBot '{prebotName}' deleted successfully!");
                    LogManager.Success($"Deleted PreBot: {prebotName}");

                    // Refresh dropdown after delete
                    prebotFilePath = null;
                    prebotConfig = new PreBotConfig();
                    LoadPreBotsIntoDropdown();
                    this.Text = "PreBot Settings";

                    // Clear all fields
                    textBoxHost.Text = "";
                    textBoxPort.Text = "";
                    textBoxUsername.Text = "";
                    textBoxPassword.Text = "";
                    textBoxSitename.Text = "";
                    textBoxBotName.Text = "";
                    textBoxChannel1.Text = "";
                    textBoxBlowfishKey1.Text = "";
                    textBoxSectionRegex.Text = "";
                    textBoxSectionPrefix.Text = "";
                    textBoxSectionSuffix.Text = "";
                    textBoxNameRegex.Text = "";
                }
                catch (Exception ex)
                {
                    DialogHelper.ShowError(ex, "Failed to delete PreBot configuration");
                    LogManager.Error($"Error deleting PreBot: {ex.Message}");
                }
            }
        }


        private void TestRegex()
        {
            try
            {
                string input = textBoxTest.Text.Trim();
                string sectionRegexPattern = textBoxSectionRegex.Text.Trim();
                string nameRegexPattern = textBoxNameRegex.Text.Trim();
                string sectionPrefix = textBoxSectionPrefix.Text.Trim();
                string sectionSuffix = textBoxSectionSuffix.Text.Trim();

                if (string.IsNullOrEmpty(input))
                {
                    DialogHelper.ShowWarning("Please provide an input line to test.");
                    return;
                }

                if (string.IsNullOrEmpty(sectionRegexPattern) || string.IsNullOrEmpty(nameRegexPattern))
                {
                    DialogHelper.ShowWarning("Please provide both Section and Release Name regex patterns.");
                    return;
                }

                // Match the Section
                var sectionMatch = Regex.Match(input, sectionRegexPattern, RegexOptions.IgnoreCase);
                string rawSectionGroup = sectionMatch.Success && sectionMatch.Groups.Count > 1
                    ? sectionMatch.Groups[1].Value
                    : string.Empty;

                string section = rawSectionGroup;

                // Trim prefix and suffix
                if (!string.IsNullOrEmpty(sectionPrefix) && section.StartsWith(sectionPrefix))
                {
                    section = section.Substring(sectionPrefix.Length);
                }

                if (!string.IsNullOrEmpty(sectionSuffix) && section.EndsWith(sectionSuffix))
                {
                    section = section.Substring(0, section.Length - sectionSuffix.Length);
                }

                // Match the Release Name
                var releaseMatch = Regex.Match(input, nameRegexPattern, RegexOptions.IgnoreCase);
                string rawReleaseGroup = releaseMatch.Success && releaseMatch.Groups.Count > 1
                    ? releaseMatch.Groups[1].Value
                    : string.Empty;

                string releaseName = rawReleaseGroup;

                // Show results in a styled dialog
                ShowRegexTestResults(
                    input,
                    section,
                    releaseName,
                    sectionMatch.Success,
                    releaseMatch.Success,
                    rawSectionGroup,
                    rawReleaseGroup,
                    sectionMatch.Value,
                    releaseMatch.Value);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Regex test failed");
            }
        }

        private void ShowRegexTestResults(
            string input,
            string section,
            string releaseName,
            bool sectionMatched,
            bool releaseMatched,
            string rawSectionGroup,
            string rawReleaseGroup,
            string sectionFullMatch,
            string releaseFullMatch)
        {
            var resultForm = new Form
            {
                Text = "Regex Test Results",
                Width = 550,
                Height = 500,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(22, 26, 36)
            };

            var resultRichTextBox = new RichTextBox
            {
                Location = new Point(15, 15),
                Size = new Size(500, 350),
                ReadOnly = true,
                Font = new Font("Cascadia Mono", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(13, 16, 24),
                ForeColor = Color.White
            };

            void AppendColored(string text, Color color)
            {
                resultRichTextBox.SelectionStart = resultRichTextBox.TextLength;
                resultRichTextBox.SelectionColor = color;
                resultRichTextBox.AppendText(text);
            }

            // Input line

            AppendColored("Test Input:\n", Color.Cyan);
            AppendColored($"{input}\n\n", Color.White);

            // SECTION RESULT
            AppendColored("SECTION MATCH\n", Color.Yellow);
            if (sectionMatched)
            {
                // Full match
                AppendColored("  Full match: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(sectionFullMatch) ? "(empty)" : sectionFullMatch,
                    Color.LimeGreen);
                AppendColored("\n", Color.White);

                // Group[1] raw
                AppendColored("  Group[1] raw: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(rawSectionGroup) ? "(empty)" : rawSectionGroup,
                    Color.LightBlue);
                AppendColored("\n", Color.White);

                // Group[1] after prefix/suffix trim
                AppendColored("  Group[1] trimmed: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(section) ? "(empty)" : section,
                    Color.LightBlue);
                AppendColored("\n", Color.White);

                AppendColored("  Status: ✓ Match\n\n", Color.LimeGreen);
            }
            else
            {
                AppendColored("  Status: ✗ No match\n", Color.Red);
                AppendColored("  → Check your Section regex pattern.\n\n", Color.Orange);
            }

            // RELEASE RESULT
            AppendColored("RELEASE MATCH\n", Color.Yellow);
            if (releaseMatched)
            {
                // Full match
                AppendColored("  Full match: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(releaseFullMatch) ? "(empty)" : releaseFullMatch,
                    Color.LimeGreen);
                AppendColored("\n", Color.White);

                // Group[1] raw
                AppendColored("  Group[1] raw: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(rawReleaseGroup) ? "(empty)" : rawReleaseGroup,
                    Color.LightBlue);
                AppendColored("\n", Color.White);

                // Group[1] (no extra trimming done here)
                AppendColored("  Parsed name: ", Color.LightGray);
                AppendColored(
                    string.IsNullOrEmpty(releaseName) ? "(empty)" : releaseName,
                    Color.LightBlue);
                AppendColored("\n", Color.White);

                AppendColored("  Status: ✓ Match\n\n", Color.LimeGreen);
            }
            else
            {
                AppendColored("  Status: ✗ No match\n", Color.Red);
                AppendColored("  → Check your Release Name regex pattern.\n\n", Color.Orange);
            }


            // Overall status
            if (sectionMatched && releaseMatched)
            {
                AppendColored("✓ Both patterns matched successfully!", Color.LimeGreen);
            }
            else if (sectionMatched || releaseMatched)
            {
                AppendColored("⚠ Only one of the patterns matched.", Color.Orange);
            }
            else
            {
                AppendColored("✗ Neither pattern matched.", Color.Red);
            }

            var closeButton = new Button
            {
                Text = "Close",
                Location = new Point(230, 380),
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(37, 42, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            ThemeManager.StyleDangerButton(closeButton);
            closeButton.Click += (s, e) => resultForm.Close();

            resultForm.Controls.Add(resultRichTextBox);
            resultForm.Controls.Add(closeButton);
            resultForm.ShowDialog();
        }

        // Event Handlers
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SavePrebotConfig();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DeletePrebotConfig();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            TestRegex();
        }

    }

    // Configuration Classes (unchanged)
    public class PreBotConfig
    {
        public ZncServerSettings ZncServer { get; set; } = new ZncServerSettings();
        public PreBotSiteSettings SiteSettings { get; set; } = new PreBotSiteSettings();
    }

    public class ZncServerSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class PreBotSiteSettings
    {
        public string Sitename { get; set; }
        public string BotName { get; set; }
        public string Channel1 { get; set; }
        public string BlowfishKey1 { get; set; }
        public string SectionRegex { get; set; }
        public string SectionPrefix { get; set; }
        public string SectionSuffix { get; set; }
        public string NameRegex { get; set; }
    }
}



/// <summary>
/// Imports latest releases from predb.club API
/// </summary>
public static class PreBotManager
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<int> ImportFromPredbClubAsync(int count = 100)
    {
        try
        {
            LogManager.Info($"Importing {count} releases from predb.club...");

            string apiUrl = $"https://predb.club/api/v1/?count={count}";
            var response = await httpClient.GetStringAsync(apiUrl);
            var json = JObject.Parse(response);

            if (json["status"]?.ToString() != "success")
            {
                LogManager.Error("predb.club API returned non-success status");
                return 0;
            }

            var rows = json["data"]?["rows"] as JArray;
            if (rows == null || rows.Count == 0)
            {
                LogManager.Warning("No releases returned from predb.club");
                return 0;
            }

            // SORT BY preAt TIMESTAMP (oldest first, newest last)
            var sortedRows = rows
                .OrderBy(row => row["preAt"]?.ToObject<long>() ?? 0)
                .ToList();

            int imported = 0;
            int skipped = 0;

            // Import in chronological order
            foreach (var row in sortedRows)
            {
                try
                {
                    string releaseName = row["name"]?.ToString();
                    string category = row["cat"]?.ToString();
                    long preAtUnix = row["preAt"]?.ToObject<long>() ?? 0;

                    if (string.IsNullOrWhiteSpace(releaseName) || preAtUnix == 0)
                    {
                        skipped++;
                        continue;
                    }

                    // Convert Unix timestamp to DateTime
                    var preTime = DateTimeOffset.FromUnixTimeSeconds(preAtUnix).UtcDateTime;

                    // Store in database (oldest entries get lowest IDs)
                    await SQLiteHelper.StorePretimeAsync(releaseName, category, preTime);
                    imported++;

                    //  Log progress every 25 releases
                    if (imported % 25 == 0)
                    {
                        LogManager.Info($"Imported {imported}/{sortedRows.Count} releases...");
                    }
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error importing release: {ex.Message}");
                    skipped++;
                }
            }

            LogManager.Success($"Import complete: {imported} releases imported, {skipped} skipped");
            return imported;
        }
        catch (Exception ex)
        {
            LogManager.Error($"Error importing from predb.club: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Stores pretime when a PreBot announces a release
    /// </summary>
    public static async Task StorePretimeAsync(string releaseName, string section)
    {
        await SQLiteHelper.StorePretimeAsync(releaseName, section, DateTime.UtcNow);
    }

    /// <summary>
    /// Checks if release exceeds max pretime (returns true if OK to race)
    /// </summary>
    /// <summary>

    public static async Task<(bool allowed, int pretimeSeconds, string reason)> CheckMaxPretimeAsync(
        string releaseName,
        int? maxPretimeSeconds)
    {
        if (!maxPretimeSeconds.HasValue || maxPretimeSeconds.Value <= 0)
        {
            return (true, 0, "No max pretime configured");
        }

        var pretimeDiff = await SQLiteHelper.GetPretimeDifferenceSecondsAsync(releaseName);

        if (pretimeDiff == -1)
        {
            // No pretime found - allow
            return (true, -1, "No pretime found in database");
        }

        if (pretimeDiff > maxPretimeSeconds.Value)
        {
            // Too old
            return (false, pretimeDiff, $"Pretime {pretimeDiff}s exceeds max {maxPretimeSeconds}s");
        }

        // OK to race
        return (true, pretimeDiff, $"Pretime {pretimeDiff}s < {maxPretimeSeconds}s");
    }



}
