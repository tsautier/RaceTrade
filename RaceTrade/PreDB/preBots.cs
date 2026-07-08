using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class preBots : Form
    {
        public preBots()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            LoadCurrentPrebot();

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
                        textBox4.Text = currentSite.Server?.Password ?? string.Empty;

                        // Site Settings
                        textBox8.Text = currentSite.SiteSettings?.Sitename ?? string.Empty;
                        textBox5.Text = currentSite.SiteSettings?.BotName ?? string.Empty;
                        Channel1_TextBox.Text = currentSite.SiteSettings?.Chan1 ?? string.Empty;
                        BlowFish1_TextBox.Text = currentSite.SiteSettings?.BlowfishKey1 ?? string.Empty;
                        Channel2_TextBox.Text = currentSite.SiteSettings?.Chan2 ?? string.Empty;
                        BlowFish2_TextBox.Text = currentSite.SiteSettings?.BlowfishKey2 ?? string.Empty;
                        Channel3_TextBox.Text = currentSite.SiteSettings?.Chan3 ?? string.Empty;
                        BlowFish3_TextBox.Text = currentSite.SiteSettings?.BlowfishKey3 ?? string.Empty;

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
                // Only save server and site settings
                currentSite.Server = new ServerSettings
                {
                    Host = textBox1.Text.Trim(),
                    Port = int.TryParse(textBox2.Text.Trim(), out var port) ? port : 0,
                    Username = textBox3.Text.Trim(),
                    Password = textBox4.Text.Trim()
                };

                currentSite.SiteSettings = new SiteSettings
                {
                    Sitename = textBox8.Text.Trim(),
                    BotName = textBox5.Text.Trim(),
                    NewRegexPattern = New_Field_regex.Text.Trim(),
                    IgnoreWords = Ignore_Word_regex.Text.Trim(),
                    ReleaseRegexPattern = Name_Field_regex.Text.Trim(),
                    SectionRegexPattern = Section_Field_regex.Text.Trim(),
                    SectionPrefix = Section_Prefix.Text.Trim(),
                    SectionSuffix = Section_Suffix.Text.Trim(),
                    DlOnlySite = Dl_Only_CheckBox.Checked,
                    DisableSite = Disable_site_checkbox.Checked,
                    Chan1 = Channel1_TextBox.Text.Trim(),
                    BlowfishKey1 = BlowFish1_TextBox.Text.Trim(),
                    Chan2 = Channel2_TextBox.Text.Trim(),
                    BlowfishKey2 = BlowFish2_TextBox.Text.Trim(),
                    Chan3 = Channel3_TextBox.Text.Trim(),
                    BlowfishKey3 = BlowFish3_TextBox.Text.Trim()
                };

                // Do not save RaceSectionsEnabled or RaceSites here

                File.WriteAllText(currentSiteFilePath, JsonConvert.SerializeObject(currentSite, Formatting.Indented));
                MessageBox.Show("Site configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving site configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }
}
