using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class preBots : AntdUI.Window
    {
        public preBots()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            LoadCurrentPrebot();

        }


        // Original stored (encrypted) secrets, kept so a value we could not decrypt is
        // written back untouched instead of being silently wiped on save.
        private string _origPassword, _origKey1, _origKey2, _origKey3;
        private bool _pwdFailed, _key1Failed, _key2Failed, _key3Failed;

        /// <summary>Decrypts a stored secret; on failure returns "" and flags it.</summary>
        private static string SafeDecrypt(string stored, string label, out bool failed)
        {
            failed = false;
            if (string.IsNullOrWhiteSpace(stored))
                return string.Empty;

            try
            {
                return SecureConfig.Decrypt(stored);
            }
            catch (Exception ex)
            {
                failed = true;
                LogManager.Error($"Could not decrypt {label} - it was encrypted by another Windows user/machine. Please re-enter it. ({ex.Message})");
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the value to persist: the freshly typed secret (encrypted), or the
        /// original ciphertext when we couldn't decrypt it and the user left it blank.
        /// </summary>
        private static string ResolveSecret(string typed, string original, bool decryptFailed)
        {
            if (decryptFailed && string.IsNullOrWhiteSpace(typed))
                return original;

            return SecureConfig.Encrypt(typed);
        }

        private void LoadCurrentPrebot()
        {
            try
            {
                if (File.Exists(currentSiteFilePath))
                {
                    string jsonContent = File.ReadAllText(currentSiteFilePath);
                    currentSite = JsonConvert.DeserializeObject<SiteConfig>(jsonContent);

                    // Populate the fields from the currentSite object
                    if (currentSite != null)
                    {
                        // Server Settings
                        textBox1.Text = currentSite.Server?.Host ?? string.Empty;
                        textBox2.Text = currentSite.Server?.Port.ToString() ?? string.Empty;
                        textBox3.Text = currentSite.Server?.Username ?? string.Empty;

                        // Secrets are stored encrypted - decrypt for display, and remember
                        // the originals so a failed decrypt isn't wiped on save.
                        _origPassword = currentSite.Server?.Password;
                        _origKey1 = currentSite.SiteSettings?.BlowfishKey1;
                        _origKey2 = currentSite.SiteSettings?.BlowfishKey2;
                        _origKey3 = currentSite.SiteSettings?.BlowfishKey3;

                        textBox4.Text = SafeDecrypt(_origPassword, "prebot server password", out _pwdFailed);

                        // Site Settings
                        textBox8.Text = currentSite.SiteSettings?.Sitename ?? string.Empty;
                        textBox5.Text = currentSite.SiteSettings?.BotName ?? string.Empty;
                        Channel1_TextBox.Text = currentSite.SiteSettings?.Chan1 ?? string.Empty;
                        BlowFish1_TextBox.Text = SafeDecrypt(_origKey1, "blowfish_key1", out _key1Failed);
                        Channel2_TextBox.Text = currentSite.SiteSettings?.Chan2 ?? string.Empty;
                        BlowFish2_TextBox.Text = SafeDecrypt(_origKey2, "blowfish_key2", out _key2Failed);
                        Channel3_TextBox.Text = currentSite.SiteSettings?.Chan3 ?? string.Empty;
                        BlowFish3_TextBox.Text = SafeDecrypt(_origKey3, "blowfish_key3", out _key3Failed);

                        // Regex and Prefix/Suffix
                        New_Field_regex.Text = currentSite.SiteSettings?.NewRegexPattern ?? string.Empty;
                        Ignore_Word_regex.Text = currentSite.SiteSettings?.IgnoreWords ?? string.Empty;
                        Name_Field_regex.Text = currentSite.SiteSettings?.ReleaseRegexPattern ?? string.Empty;
                        Section_Field_regex.Text = currentSite.SiteSettings?.SectionRegexPattern ?? string.Empty;
                        Section_Prefix.Text = currentSite.SiteSettings?.SectionPrefix ?? string.Empty;
                        Section_Suffix.Text = currentSite.SiteSettings?.SectionSuffix ?? string.Empty;

                        // Checkbox
                        Dl_Only_CheckBox.Checked = currentSite.SiteSettings?.DlOnlySite ?? false;
                        Disable_site_checkbox.Checked = currentSite.SiteSettings?.DisableSite ?? false;

                        // Update CurrentSiteName
                        CurrentSiteName = currentSite.SiteSettings?.Sitename ?? "Unknown Site";
                        Console.WriteLine($"Current Site: {CurrentSiteName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading site: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Save_site_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Mutate the loaded config IN PLACE. Rebuilding these objects from the form
                // would null every field this editor doesn't expose (Chan4-20, ChatKeys,
                // Pre*/Request* settings, MaxPreTime, release prefix/suffix, ...).
                if (currentSite.Server == null)
                    currentSite.Server = new ServerSettings();

                currentSite.Server.Host = textBox1.Text.Trim();
                currentSite.Server.Port = int.TryParse(textBox2.Text.Trim(), out var port) ? port : 0;
                currentSite.Server.Username = textBox3.Text.Trim();
                currentSite.Server.Password = ResolveSecret(textBox4.Text.Trim(), _origPassword, _pwdFailed);

                if (currentSite.SiteSettings == null)
                    currentSite.SiteSettings = new SiteSettings();

                var ss = currentSite.SiteSettings;
                ss.Sitename = textBox8.Text.Trim();
                ss.BotName = textBox5.Text.Trim();
                ss.NewRegexPattern = New_Field_regex.Text.Trim();
                ss.IgnoreWords = Ignore_Word_regex.Text.Trim();
                ss.ReleaseRegexPattern = Name_Field_regex.Text.Trim();
                ss.SectionRegexPattern = Section_Field_regex.Text.Trim();
                ss.SectionPrefix = Section_Prefix.Text.Trim();
                ss.SectionSuffix = Section_Suffix.Text.Trim();
                ss.DlOnlySite = Dl_Only_CheckBox.Checked;
                ss.DisableSite = Disable_site_checkbox.Checked;
                ss.Chan1 = Channel1_TextBox.Text.Trim();
                ss.Chan2 = Channel2_TextBox.Text.Trim();
                ss.Chan3 = Channel3_TextBox.Text.Trim();
                ss.BlowfishKey1 = ResolveSecret(BlowFish1_TextBox.Text.Trim(), _origKey1, _key1Failed);
                ss.BlowfishKey2 = ResolveSecret(BlowFish2_TextBox.Text.Trim(), _origKey2, _key2Failed);
                ss.BlowfishKey3 = ResolveSecret(BlowFish3_TextBox.Text.Trim(), _origKey3, _key3Failed);

                // Do not save RaceSectionsEnabled or RaceSites here

                AtomicFile.WriteAllText(currentSiteFilePath, JsonConvert.SerializeObject(currentSite, Formatting.Indented));
                MessageBox.Show("Site configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving site configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }
}
