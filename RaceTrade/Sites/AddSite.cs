using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RaceTrader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace RaceTrade
{
    public partial class AddSite : Form
    {

        private string currentSiteFilePath; // Path to the currently selected site's JSON file
        private List<RaceMapping> raceMappings = new List<RaceMapping>(); // Holds race mappings
        private string cbftpSectionsFilePath = "sections/cbftp_sections.json"; // Path to CBFTP sections JSON
        //public event Action<Dictionary<string, dynamic>> OnSaveSite;
        private SiteConfig currentSite;

        private string CurrentSiteName { get; set; } = "Unknown Site";
        private bool isEditMode = false; // Track if we're in edit mode
        private Dictionary<string, string> preBots = new Dictionary<string, string>(); // Store PreBot names and their file paths
        private List<string> affilGroups = new List<string>();


        // Constructor for editing an existing site
        private HelpForm helpForm;
        public AddSite(string siteFileName)
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);


            // In constructor, after InitializeComponent():
            Add_affil_textbox.Text = "Enter group name (e.g., GROUPiSO)";
            Add_affil_textbox.ForeColor = Color.Gray;

            Add_affil_textbox.Enter += (s, e) => {
                if (Add_affil_textbox.Text == "Enter group name (e.g., GROUPiSO)")
                {
                    Add_affil_textbox.Text = "";
                    Add_affil_textbox.ForeColor = Color.Black;
                }
            };

            Add_affil_textbox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(Add_affil_textbox.Text))
                {
                    Add_affil_textbox.Text = "Enter group name (e.g., GROUPiSO)";
                    Add_affil_textbox.ForeColor = Color.Gray;
                }
            };



            comboBox_announce.SelectedIndexChanged += comboBox_announce_SelectedIndexChanged;
            groupBox1.ForeColor = Color.White;
            groupBox2.ForeColor = Color.White;
            groupBox4.ForeColor = Color.White;
            groupBox3.ForeColor = Color.White;
            groupBox5.ForeColor = Color.White;
            groupBox6.ForeColor = Color.White;
            groupBox7.ForeColor = Color.White;
            groupBox8.ForeColor = Color.White;
            groupBox9.ForeColor = Color.White;

            TVMazeSettingsButton.DialogResult = DialogResult.None;

            // Channel management
            Add_channel_button.Click += Add_channel_button_Click;
            Remove_channel_button.Click += Remove_channel_button_Click;

            // Click a channel line -> load "#channel:key" into the textbox so it can be
            // edited, then press Add to update it in place.
            ListboxChannels.SelectedIndexChanged += (s, e) =>
            {
                if (ListboxChannels.SelectedItem is ChannelInfo selected)
                {
                    textBox_Channel_Key.Text = $"{selected.Channel}:{selected.BlowfishKey}";
                    textBox_Channel_Key.ForeColor = ThemeManager.Colors.Foreground;
                }
            };

            // Set placeholder text
            textBox_Channel_Key.Text = "Format: #channel:blowfishkey";
            textBox_Channel_Key.ForeColor = Color.Gray;

            // Handle placeholder behavior
            textBox_Channel_Key.Enter += (s, e) => {
                if (textBox_Channel_Key.Text == "Format: #channel:blowfishkey")
                {
                    textBox_Channel_Key.Text = "";
                    textBox_Channel_Key.ForeColor = ThemeManager.Colors.Foreground;
                }
            };

            textBox_Channel_Key.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(textBox_Channel_Key.Text))
                {
                    textBox_Channel_Key.Text = "Format: #channel:blowfishkey";
                    textBox_Channel_Key.ForeColor = Color.Gray;
                }
            };

            // Attach event handlers
            ListBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            ListBox5.SelectedIndexChanged += ListBox5_SelectedIndexChanged;
            btnAddGlobalRule.Click += btnAddGlobalRule_Click;
            btnRemoveGlobalRule.Click += btnRemoveGlobalRule_Click;
            ListBox6.SelectedIndexChanged += ListBox6_SelectedIndexChanged;
            AddTagRuleButton.Click += AddTagRuleButton_Click;
            RemoveTagRuleButton.Click += RemoveTagRuleButton_Click;
            TagRulelistBox.SelectedIndexChanged += TagRulelistBox_SelectedIndexChanged;
            add_affil_button.Click += add_affil_button_Click;
            remove_affil_button.Click += remove_affil_button_Click;
            btnEnableRaceSection.Click += btnEnableRaceSection_Click;
            btnDisableRaceSection.Click += btnDisableRaceSection_Click;
            listBox_affils_sites.BackColor = Color.FromArgb(22, 26, 36);
            listBox_affils_sites.ForeColor = Color.White;
            ListBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
            ListBox2.DrawMode = DrawMode.OwnerDrawFixed;
            ListBox2.DrawItem += ListBox2_DrawItem;
            site_rules_button.Click += site_rules_button_Click;

            // Add event handler for LoadEditSites_Combobox
            LoadEditSites_Combobox.SelectedIndexChanged += LoadEditSites_Combobox_SelectedIndexChanged;

            // Check if this is edit mode (null siteFileName) or add mode
            if (string.IsNullOrEmpty(siteFileName))
            {
                isEditMode = true;
                LoadPreBots();
                PopulateSiteDropdown(); // Load all sites into the dropdown

                // Select first site if available
                if (LoadEditSites_Combobox.Items.Count > 0)
                {
                    LoadEditSites_Combobox.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("No sites found. Please add a site first.", "No Sites",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                    return;
                }
            }
            else
            {
                // hide the dropdown
                isEditMode = false;
                LoadEditSites_Combobox.Visible = false;

                // Ensure the file name has the .json extension
                if (!siteFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    siteFileName += ".json";
                }

                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", siteFileName);
                currentSiteFilePath = fullPath;

                if (File.Exists(fullPath))
                {
                    LoadPreBots();
                    LoadCurrentSite();
                }
                else
                {
                    MessageBox.Show($"Site file '{fullPath}' not found. Please ensure the file exists and try again.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }

            LoadAvailableSections();
            LoadCbftpSections();
            LoadRaceSections();
            LoadMappings();
        }

        private void PopulateSiteDropdown()
        {
            try
            {
                LoadEditSites_Combobox.Items.Clear();

                var sitesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites");
                if (!Directory.Exists(sitesDirectory))
                {
                    Directory.CreateDirectory(sitesDirectory);
                    return;
                }

                var siteFiles = Directory.GetFiles(sitesDirectory, "*.json");

                foreach (var file in siteFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);

                    // Skip template files
                    if (fileName.Equals("default_site", StringComparison.OrdinalIgnoreCase) ||
                        fileName.Equals("new_site", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    try
                    {
                        var jsonContent = File.ReadAllText(file);
                        var siteConfig = JsonConvert.DeserializeObject<SiteConfig>(jsonContent);

                        if (siteConfig?.SiteSettings?.Sitename != null)
                        {
                            LoadEditSites_Combobox.Items.Add(siteConfig.SiteSettings.Sitename);
                        }
                        else
                        {
                            // Fallback to filename if sitename not found
                            LoadEditSites_Combobox.Items.Add(fileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning($"Could not load site '{fileName}': {ex.Message}");
                    }
                }

                LogManager.Info($"Loaded {LoadEditSites_Combobox.Items.Count} sites into dropdown");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error populating site dropdown: {ex.Message}");
            }
        }

        // Handle site selection from dropdown
        private void LoadEditSites_Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isEditMode) return;

            if (LoadEditSites_Combobox.SelectedItem == null) return;

            string selectedSiteName = LoadEditSites_Combobox.SelectedItem.ToString();
            string siteFileName = selectedSiteName + ".json";
            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", siteFileName);

            if (!File.Exists(fullPath))
            {
                MessageBox.Show($"Site file '{siteFileName}' not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                currentSiteFilePath = fullPath;
                LoadCurrentSite();
                LoadAvailableSections();
                LoadCbftpSections();
                LoadRaceSections();
                LoadMappings();

                LogManager.Info($"Loaded site: {selectedSiteName}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading site '{selectedSiteName}': {ex.Message}");
                MessageBox.Show($"Error loading site:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadAffils()
        {
            affilGroups.Clear();
            listBox_affils_sites.Items.Clear();

            if (currentSite?.Affils != null)
            {
                affilGroups = currentSite.Affils.ToList();

                foreach (var group in affilGroups.OrderBy(g => g))
                {
                    listBox_affils_sites.Items.Add(group);
                }
            }
        }



        private void add_affil_button_Click(object sender, EventArgs e)
        {
            var input = Add_affil_textbox.Text.Trim();

            if (input == "Enter group name (e.g., GROUPiSO)" || string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a group name.",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string groupName = input.ToUpper(); // Store in uppercase

            if (affilGroups.Contains(groupName, StringComparer.OrdinalIgnoreCase))
            {
                MessageBox.Show($"Group '{groupName}' already in affils list!",
                    "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            affilGroups.Add(groupName);
            listBox_affils_sites.Items.Add(groupName);

            // Save to config
            if (currentSite.Affils == null)
                currentSite.Affils = new List<string>();

            currentSite.Affils = affilGroups.ToList();
            SaveSiteConfiguration();

            Add_affil_textbox.Clear();
            Add_affil_textbox.Focus();

            LogManager.Success($"Added affil group: {groupName}");
        }


        private void remove_affil_button_Click(object sender, EventArgs e)
        {
            if (listBox_affils_sites.SelectedItem == null)
            {
                MessageBox.Show("Please select an affil group to remove.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedGroup = listBox_affils_sites.SelectedItem.ToString();

            affilGroups.Remove(selectedGroup);
            listBox_affils_sites.Items.Remove(selectedGroup);

            currentSite.Affils = affilGroups.ToList();
            SaveSiteConfiguration();

            LogManager.Success($"Removed affil group: {selectedGroup}");
        }


        private void Help_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Create help form if it doesn't exist or was disposed
                if (helpForm == null || helpForm.IsDisposed)
                {
                    helpForm = new HelpForm
                    {
                        StartPosition = FormStartPosition.CenterParent
                    };
                }

                // Show it as modeless, owned by this AddSite form
                helpForm.Show(this);    
                helpForm.BringToFront();
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening help");
                MessageBox.Show($"Error opening help:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void LoadPreBots()
        {
            try
            {
                //Console.WriteLine("[DEBUG] Starting LoadPreBots...");

                var preBotsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pre_bots");
                if (!Directory.Exists(preBotsDirectory))
                {
                    throw new DirectoryNotFoundException($"The directory '{preBotsDirectory}' does not exist.");
                }

                // Clear previous PreBots from the dropdown
                preBots.Clear();
                comboBox_announce.Items.Clear();

                // Add default options
                comboBox_announce.Items.Add("PreBot");
                comboBox_announce.Items.Add("SiteBot");

                //Console.WriteLine("[DEBUG] Added default items: PreBot, SiteBot");

                // Load all PreBot JSON files
                var preBotFiles = Directory.GetFiles(preBotsDirectory, "*.json");
                //Console.WriteLine($"[DEBUG] Found {preBotFiles.Length} JSON files in 'pre_bots' directory.");

                foreach (var file in preBotFiles)
                {
                    if (Path.GetFileName(file).Equals("new_prebot.json", StringComparison.OrdinalIgnoreCase))
                    {
                        //Console.WriteLine("[DEBUG] Skipping file: new_prebot.json");
                        continue; // Skip loading this file
                    }

                    //Console.WriteLine($"[DEBUG] Reading file: {file}");

                    try
                    {
                        var jsonContent = File.ReadAllText(file);
                        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonContent);

                        // Check if SiteSettings and Sitename exist in the JSON
                        if (jsonData.ContainsKey("SiteSettings") && jsonData["SiteSettings"] is JObject siteSettings &&
                            siteSettings.ContainsKey("Sitename"))
                        {
                            var preBotName = siteSettings["Sitename"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(preBotName))
                            {
                                var displayName = $"Global PreBot ({preBotName})";
                                preBots[displayName] = file; // Map PreBot display name to its file path
                                comboBox_announce.Items.Add(displayName); // Add to the dropdown
                                //Console.WriteLine($"[DEBUG] Added Global PreBot: {displayName}");
                            }
                            else
                            {
                                //Console.WriteLine("[WARNING] 'Sitename' field is empty or missing.");
                            }
                        }
                        else
                        {
                            //Console.WriteLine("[WARNING] 'SiteSettings' or 'Sitename' field not found in JSON.");
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine($"[ERROR] Error reading file '{file}': {innerEx.Message}");
                    }
                }

                //Console.WriteLine("[DEBUG] Finished LoadPreBots successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PreBots: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Console.WriteLine($"[ERROR] Exception in LoadPreBots: {ex}");
            }
        }

        private void LoadCurrentSite()
        {
            try
            {
                if (File.Exists(currentSiteFilePath))
                {
                    string jsonContent = File.ReadAllText(currentSiteFilePath);
                    currentSite = JsonConvert.DeserializeObject<SiteConfig>(jsonContent);

                    if (currentSite != null)
                    {
                        // Populate fields from currentSite object
                        textBox1.Text = currentSite.Server?.Host ?? string.Empty;
                        textBox2.Text = currentSite.Server?.Port.ToString() ?? string.Empty;
                        textBox3.Text = currentSite.Server?.Username ?? string.Empty;
                        textBox4.Text = SecureConfig.Decrypt(currentSite.Server?.Password ?? string.Empty);

                        textBox8.Text = currentSite.SiteSettings?.Sitename ?? string.Empty;
                        textBox5.Text = currentSite.SiteSettings?.BotName ?? string.Empty;
                        // âœ… NEW: Load channels into listbox
                        ListboxChannels.Items.Clear();

                        // Load all channels (chan1, chan2, chan3, etc.)
                        for (int i = 1; i <= 20; i++) // Support up to 20 channels
                        {
                            var channelProp = currentSite.SiteSettings?.GetType().GetProperty($"Chan{i}");
                            var keyProp = currentSite.SiteSettings?.GetType().GetProperty($"BlowfishKey{i}");

                            if (channelProp != null && keyProp != null)
                            {
                                var channel = channelProp.GetValue(currentSite.SiteSettings)?.ToString();
                                var key = keyProp.GetValue(currentSite.SiteSettings)?.ToString();

                                if (!string.IsNullOrWhiteSpace(channel))
                                {
                                    string plainKey = "";
                                    if (!string.IsNullOrWhiteSpace(key))
                                    {
                                        try
                                        {
                                            plainKey = SecureConfig.Decrypt(key);
                                        }
                                        catch (Exception ex)
                                        {
                                            // Key was encrypted by a different Windows user/machine (DPAPI is
                                            // per-user). Don't blow up the whole load - show it empty so it
                                            // can simply be re-entered.
                                            plainKey = "";
                                            LogManager.Error(
                                                $"Could not decrypt stored Blowfish key for {channel}. " +
                                                $"It was encrypted by another Windows user/machine - please re-enter it. ({ex.Message})");
                                        }
                                    }

                                    ListboxChannels.Items.Add(new ChannelInfo
                                    {
                                        Channel = channel,
                                        BlowfishKey = plainKey
                                    });
                                }
                            }
                        }

                        New_Field_regex.Text = currentSite.SiteSettings?.NewRegexPattern ?? string.Empty;
                        Ignore_Word_regex.Text = currentSite.SiteSettings?.IgnoreWords ?? string.Empty;
                        Name_Field_regex.Text = currentSite.SiteSettings?.ReleaseRegexPattern ?? string.Empty;
                        Section_Field_regex.Text = currentSite.SiteSettings?.SectionRegexPattern ?? string.Empty;
                        Section_Prefix.Text = currentSite.SiteSettings?.SectionPrefix ?? string.Empty;
                        Section_Suffix.Text = currentSite.SiteSettings?.SectionSuffix ?? string.Empty;
                        // PRE / affil capture fields
                        Pre_field_regex.Text = currentSite.SiteSettings?.PreRegexPattern ?? string.Empty;
                        Section_pre_field.Text = currentSite.SiteSettings?.PreSectionRegexPattern ?? string.Empty;
                        Section_pre_prefix_field.Text = currentSite.SiteSettings?.PreSectionPrefix ?? string.Empty;
                        Section_pre_suffix_field.Text = currentSite.SiteSettings?.PreSectionSuffix ?? string.Empty;
                        Release_Pre_field.Text = currentSite.SiteSettings?.PreReleaseRegexPattern ?? string.Empty;

                        Dl_Only_CheckBox.Checked = currentSite.SiteSettings?.DlOnlySite ?? false;
                        Disable_site_checkbox.Checked = currentSite.SiteSettings?.DisableSite ?? false;

                        int maxPreTime = currentSite.SiteSettings?.MaxPreTime ?? 0;
                        numericUpDown_pretime.Value = Math.Max(0, Math.Min((decimal)maxPreTime, numericUpDown_pretime.Maximum));

                        // ─────────────────────────────────────────────
                        // request auto-fill settings → UI controls
                        // ─────────────────────────────────────────────
                        chkRequestAutoFillEnabled.Checked =
                            currentSite.SiteSettings?.RequestAutoFillEnabled ?? false;

                        chkRequestCanFillSource.Checked =
                            currentSite.SiteSettings?.RequestCanFillSource ?? false;

                        txtRequestListCommand.Text =
                            currentSite.SiteSettings?.RequestListCommand ?? string.Empty;

                        txtRequestLinePattern.Text =
                            currentSite.SiteSettings?.RequestLinePattern ?? string.Empty;

                        txtRequestFillTemplate.Text =
                            currentSite.SiteSettings?.RequestFillTemplate ?? string.Empty;

                        txtRequestDstPathTemplate.Text =
                            currentSite.SiteSettings?.RequestDstPathTemplate ?? string.Empty;

                        txtRequestCompletePattern.Text =
                            currentSite.SiteSettings?.RequestCompletePattern ?? string.Empty;

                        // Poll seconds – default to 300 if not set / zero
                        int pollSeconds = currentSite.SiteSettings?.RequestPollSeconds ?? 0;
                        if (pollSeconds <= 0)
                        {
                            pollSeconds = 300;
                        }
                        numRequestPollSeconds.Value = Math.Max(
                            numRequestPollSeconds.Minimum,
                            Math.Min(numRequestPollSeconds.Maximum, pollSeconds)
                        );

                        LoadAffils();



                        // Get the value from the JSON
                        string preOrSiteValue = currentSite.SiteSettings?.PreOrSite ?? "SiteBot";
                        //Console.WriteLine($"[DEBUG] preOrSiteValue from JSON: {preOrSiteValue}");


                        // Log all items in the dropdown
                        //Console.WriteLine("[DEBUG] Items in comboBox_announce:");
                        foreach (var item in comboBox_announce.Items)
                        {
                            //Console.WriteLine($"[DEBUG] Dropdown item: {item}");
                        }

                        // Match exact item in comboBox_announce
                        bool foundMatch = false;
                        foreach (var item in comboBox_announce.Items)
                        {
                            if (item.ToString().Equals(preOrSiteValue, StringComparison.OrdinalIgnoreCase))
                            {
                                comboBox_announce.SelectedItem = item;
                                foundMatch = true;
                                //Console.WriteLine($"[DEBUG] Selected item in comboBox_announce: {item}");
                                break;
                            }
                        }

                        if (!foundMatch)
                        {
                            //Console.WriteLine($"[WARNING] No match found for '{preOrSiteValue}' in comboBox_announce items.");
                            comboBox_announce.SelectedIndex = 0; // Default to the first item (e.g., SiteBot)
                        }

                        CurrentSiteName = currentSite.SiteSettings?.Sitename ?? "Unknown Site";
                        this.Text = $"Edit/Add Site - {CurrentSiteName}"; 
                        //Console.WriteLine($"[DEBUG] Current Site: {CurrentSiteName}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading site: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        private void Add_channel_button_Click(object sender, EventArgs e)
        {
            var input = textBox_Channel_Key.Text.Trim();

            // Skip if it's the placeholder text
            if (input == "Format: #channel:blowfishkey" || string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please enter a channel and blowfish key.\n\nFormat: #channel:blowfishkey",
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Parse input: #channel:blowfishkey
            var parts = input.Split(new[] { ':' }, 2);

            if (parts.Length != 2)
            {
                MessageBox.Show("Invalid format!\n\nUse: #channel:blowfishkey\nExample: #announce:mykey123",
                    "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string channel = parts[0].Trim();
            string blowfishKey = parts[1].Trim();

            // Validate channel starts with #
            if (!channel.StartsWith("#"))
            {
                MessageBox.Show("Channel must start with #\n\nExample: #announce:mykey123",
                    "Invalid Channel", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If the channel already exists, update its key in place (edit support)
            for (int i = 0; i < ListboxChannels.Items.Count; i++)
            {
                if (ListboxChannels.Items[i] is ChannelInfo existingChannel &&
                    existingChannel.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase))
                {
                    existingChannel.Channel = channel;
                    existingChannel.BlowfishKey = blowfishKey;
                    ListboxChannels.Items[i] = existingChannel; // refresh display

                    textBox_Channel_Key.Clear();
                    textBox_Channel_Key.Focus();

                    LogManager.Success($"Updated key for channel: {channel}");
                    return;
                }
            }

            // Add to listbox
            ListboxChannels.Items.Add(new ChannelInfo
            {
                Channel = channel,
                BlowfishKey = blowfishKey
            });

            // Clear textbox
            textBox_Channel_Key.Clear();
            textBox_Channel_Key.Focus();

            LogManager.Success($"Added channel: {channel}");
        }

        private void Remove_channel_button_Click(object sender, EventArgs e)
        {
            if (ListboxChannels.SelectedItem == null)
            {
                MessageBox.Show("Please select a channel to remove.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedChannel = (ChannelInfo)ListboxChannels.SelectedItem;
            ListboxChannels.Items.Remove(selectedChannel);

            LogManager.Success($"Removed channel: {selectedChannel.Channel}");
        }





        private void comboBox_announce_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selectedAnnounce = comboBox_announce.SelectedItem?.ToString();
                //Console.WriteLine($"[DEBUG] Selected Announce: {selectedAnnounce}");

                if (string.IsNullOrWhiteSpace(selectedAnnounce))
                    return;

                // If the selected announce is a dynamically loaded PreBot
                if (preBots.ContainsKey(selectedAnnounce))
                {
                    //Console.WriteLine($"[DEBUG] Found PreBot: {selectedAnnounce}");
                    textBox5.Text = currentSite.SiteSettings?.BotName ?? string.Empty; // Retain original BotName
                    //textBox5.Enabled = false; // Disable editing
                }
                else
                {
                    //Console.WriteLine($"[DEBUG] Selected item is not a PreBot. Allowing BotName editing.");
                    textBox5.Text = currentSite.SiteSettings?.BotName ?? string.Empty; // Retain original BotName
                    textBox5.Enabled = true; // Enable editing
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[ERROR] Exception in comboBox_announce_SelectedIndexChanged: {ex.Message}");
                MessageBox.Show($"Error handling announce selection: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        private void LoadCbftpSections()
        {
            try
            {
                if (!File.Exists(cbftpSectionsFilePath))
                {
                    throw new FileNotFoundException($"The CBFTP sections file '{cbftpSectionsFilePath}' does not exist.");
                }

                // Deserialize the JSON file into a dynamic object
                var jsonData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(File.ReadAllText(cbftpSectionsFilePath));

                // Check if cbftp_sections exists in the JSON
                if (jsonData.ContainsKey("cbftp_sections"))
                {
                    var cbftpSections = jsonData["cbftp_sections"].ToObject<Dictionary<string, string>>();
                    lstCbftpSections.Items.Clear();

                    // Add the values of cbftp_sections to the ListBox
                    foreach (var section in cbftpSections.Values)
                    {
                        lstCbftpSections.Items.Add(section);
                    }
                }
                else
                {
                    MessageBox.Show($"The 'cbftp_sections' field is missing in the JSON file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CBFTP sections: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void DebugLog(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        private void ListBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var sectionItem = (SectionItem)ListBox2.Items[e.Index];

            // Determine colors
            Color textColor;
            Color backgroundColor;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                // Selected item
                backgroundColor = Color.FromArgb(0, 168, 255); // Blue
                textColor = Color.White;
            }
            else if (sectionItem.IsEnabled)
            {
                // Enabled (normal)
                backgroundColor = Color.FromArgb(22, 26, 36); // Dark background
                textColor = Color.White;
            }
            else
            {
                // Disabled (greyed out)
                backgroundColor = Color.FromArgb(22, 26, 36);
                textColor = Color.Gray;
            }

            // Draw background
            e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);

            // Draw text
            e.Graphics.DrawString(
                sectionItem.SectionName,
                e.Font,
                new SolidBrush(textColor),
                e.Bounds.Left + 2,
                e.Bounds.Top + 2
            );

            e.DrawFocusRectangle();
        }



        private void LoadRaceSections()
        {
            try
            {
                ListBox2.Items.Clear();

                if (currentSite?.Sections != null && currentSite.Sections.Any())
                {
                    foreach (var section in currentSite.Sections)
                    {
                        var sectionItem = new SectionItem
                        {
                            SectionName = section.IrcName,
                            IsEnabled = currentSite.RaceSectionsEnabled?.Contains(section.IrcName) ?? false
                        };

                        ListBox2.Items.Add(sectionItem);
                    }
                }

                // Initial button state
                UpdateEnableDisableButtons();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading race sections: {ex.Message}");
            }
        }


        private void LoadMappings()
        {
            try
            {
                raceMappings.Clear();

                if (currentSite?.Sections != null)
                {
                    foreach (var section in currentSite.Sections)
                    {
                        var raceMapping = new RaceMapping
                        {
                            IrcName = section.IrcName,
                            Rules = section.Rules ?? new List<string>(),
                            Skiplists = section.Skiplists ?? new List<string>(),
                            DupeRules = section.DupeRules,
                            Mappings = section.Tags?.Select(tag => new Mapping
                            {
                                MapCbftpSection = tag.MapCbftpSection,
                                TriggerRegex = string.IsNullOrEmpty(tag.TriggerRegex) ? ".*" : tag.TriggerRegex,
                                Rules = tag.Rules ?? new List<string>()
                            }).ToList() ?? new List<Mapping>()
                        };

                        raceMappings.Add(raceMapping);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading mappings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveMappings()
        {
            try
            {
                if (raceMappings == null || !raceMappings.Any())
                {
                    MessageBox.Show("No mappings to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Load the existing JSON file to preserve unmapped sections and other data
                var siteData = File.Exists(currentSiteFilePath)
                    ? JsonConvert.DeserializeObject<SiteConfig>(File.ReadAllText(currentSiteFilePath))
                    : new SiteConfig();

                if (siteData.Sections == null)
                {
                    siteData.Sections = new List<Section>();
                }

                foreach (var mapping in raceMappings)
                {
                    // Find or create the corresponding IRC section
                    var section = siteData.Sections.FirstOrDefault(s => s.IrcName == mapping.IrcName);
                    if (section == null)
                    {
                        section = new Section
                        {
                            IrcName = mapping.IrcName,
                            Tags = new List<Tag>(),
                            Rules = new List<string>(),
                            Skiplists = new List<string>(),
                            DupeRules = new DupeRules()
                        };
                        siteData.Sections.Add(section);
                    }

                    // Update tags (Mapped CBFTP sections)
                    section.Tags = mapping.Mappings.Select(m => new Tag
                    {
                        MapCbftpSection = m.MapCbftpSection,
                        TriggerRegex = string.IsNullOrEmpty(m.TriggerRegex) ? ".*" : m.TriggerRegex, // Default regex
                        Rules = m.Rules ?? new List<string>()
                    }).ToList();

                    // Update global rules, skiplists, and dupe rules
                    section.Rules = mapping.Rules ?? section.Rules;
                    section.Skiplists = mapping.Skiplists ?? section.Skiplists;
                    section.DupeRules = mapping.DupeRules ?? section.DupeRules;
                }

                // Save the updated configuration back to the JSON file
                File.WriteAllText(currentSiteFilePath, JsonConvert.SerializeObject(siteData, Formatting.Indented));
                MessageBox.Show("Mappings saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving mappings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }







        private void LoadAvailableSections()
        {
            try
            {
                ListBox1.Items.Clear();

                if (currentSite?.Sections != null && currentSite.Sections.Any())
                {
                    foreach (var section in currentSite.Sections)
                    {
                        //Console.WriteLine($"[DEBUG] Adding section to ListBox1: {section.IrcName}");
                        ListBox1.Items.Add(section.IrcName);
                    }
                }
                else
                {
                    //Console.WriteLine("[DEBUG] No sections found in the configuration.");
                }
            }
            catch (Exception)
            {
                //Console.WriteLine($"[ERROR] Error loading available sections: {ex.Message}");
            }
        }
        private void Save_site_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate sitename
                string siteName = textBox8.Text.Trim();
                if (string.IsNullOrWhiteSpace(siteName))
                {
                    MessageBox.Show("Please enter a site name.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Build the correct file path based on sitename
                string siteFileName = siteName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                    ? siteName
                    : siteName + ".json";

                string sitePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", siteFileName);

                // Update currentSiteFilePath to the correct path
                currentSiteFilePath = sitePath;

                // Create or update the site config
                if (currentSite == null)
                {
                    currentSite = new SiteConfig
                    {
                        Server = new ServerSettings(),
                        SiteSettings = new SiteSettings(),
                        Sections = new List<Section>(),
                        RaceSectionsEnabled = new List<string>(),
                        GlobalBlacklist = new List<string>()
                    };
                }

                // Save server settings
                currentSite.Server = new ServerSettings
                {
                    Host = textBox1.Text.Trim(),
                    Port = int.TryParse(textBox2.Text.Trim(), out var port) ? port : 0,
                    Username = textBox3.Text.Trim(),
                    Password = SecureConfig.Encrypt(textBox4.Text.Trim())
                };

                // Preserve existing chat keys so we don't lose chat-only channels
                var existingChatKeys = currentSite?.SiteSettings?.ChatKeys
                                       ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                // Save site settings
                currentSite.SiteSettings = new SiteSettings
                {
                    Sitename = siteName,
                    BotName = textBox5.Text.Trim(),
                    NewRegexPattern = New_Field_regex.Text.Trim(),
                    IgnoreWords = Ignore_Word_regex.Text.Trim(),
                    ReleaseRegexPattern = Name_Field_regex.Text.Trim(),
                    SectionRegexPattern = Section_Field_regex.Text.Trim(),
                    SectionPrefix = Section_Prefix.Text.Trim(),
                    SectionSuffix = Section_Suffix.Text.Trim(),

                    // PRE / affil specific regexes
                    PreRegexPattern = Pre_field_regex.Text.Trim(),
                    PreSectionRegexPattern = Section_pre_field.Text.Trim(),
                    PreSectionPrefix = Section_pre_prefix_field.Text.Trim(),
                    PreSectionSuffix = Section_pre_suffix_field.Text.Trim(),
                    PreReleaseRegexPattern = Release_Pre_field.Text.Trim(),

                    DlOnlySite = Dl_Only_CheckBox.Checked,
                    DisableSite = Disable_site_checkbox.Checked,
                    PreOrSite = comboBox_announce.SelectedItem?.ToString() ?? "SiteBot",
                    ChatKeys = existingChatKeys,

                    MaxPreTime = (int)numericUpDown_pretime.Value,

                    // ─────────────────────────────────────────────
                    // request auto-fill settings from controls
                    // ─────────────────────────────────────────────
                    RequestAutoFillEnabled = chkRequestAutoFillEnabled.Checked,
                    RequestListCommand = txtRequestListCommand.Text.Trim(),
                    RequestLinePattern = txtRequestLinePattern.Text.Trim(),
                    RequestFillTemplate = txtRequestFillTemplate.Text.Trim(),
                    RequestDstPathTemplate = txtRequestDstPathTemplate.Text.Trim(),
                    RequestCompletePattern = txtRequestCompletePattern.Text.Trim(),
                    RequestPollSeconds = (int)numRequestPollSeconds.Value,
                    RequestCanFillSource = chkRequestCanFillSource.Checked
                };



                // Save channels from listbox (also pushes the raw keys into the running
                // IRC clients, same as the chatbox does).
                ApplyChannelListToSiteSettings();

                // Ensure the sites directory exists
                string sitesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites");
                if (!Directory.Exists(sitesDirectory))
                {
                    Directory.CreateDirectory(sitesDirectory);
                }

                // Write to JSON file
                string jsonContent = JsonConvert.SerializeObject(currentSite, Formatting.Indented);
                File.WriteAllText(currentSiteFilePath, jsonContent);
                LogManager.Success($"Site saved! Passwords encrypted automatically");
                //MessageBox.Show("Site saved! Passwords encrypted automatically.", "Success");

                this.DialogResult = DialogResult.OK;

                // Update form title to show saved sitename
                this.Text = $"Edit/Add Site - {siteName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving site configuration: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Delete_site_button_Click(object sender, EventArgs e)
        {
            if (!File.Exists(currentSiteFilePath))
            {
                MessageBox.Show("Site file not found.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete the site '{textBox8.Text}'?\n\n" +
                "This action cannot be undone!",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                string deletedSiteName = textBox8.Text;
                File.Delete(currentSiteFilePath);
                LogManager.Success($"Site '{deletedSiteName}' deleted successfully");

                if (isEditMode)
                {
                    // Refresh the dropdown
                    PopulateSiteDropdown();

                    // Select first available site, or close if none left
                    if (LoadEditSites_Combobox.Items.Count > 0)
                    {
                        LoadEditSites_Combobox.SelectedIndex = 0;
                        MessageBox.Show($"Site '{deletedSiteName}' deleted. Loaded next site.",
                            "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Site '{deletedSiteName}' deleted. No more sites available.",
                            "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.Retry;
                        this.Close();
                    }
                }
                else
                {
                    this.DialogResult = DialogResult.Retry;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting site: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnAddIrcSection_Click(object sender, EventArgs e)
        {
            string newSectionName = AddIrcTextbox.Text.Trim();

            if (string.IsNullOrEmpty(newSectionName))
            {
                MessageBox.Show("Please enter a section name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check for duplicates
            if (currentSite.Sections.Any(s => s.IrcName.Equals(newSectionName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show($"Section '{newSectionName}' already exists.", "Duplicate Section",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create new section
                var newSection = new Section
                {
                    IrcName = newSectionName,
                    Tags = new List<Tag>(),
                    Rules = new List<string>(),
                    Skiplists = new List<string>(),
                    DupeRules = new DupeRules
                    {
                        FirstWins = false,
                        Priority = string.Empty
                    }
                };

                currentSite.Sections.Add(newSection);

                // Add to raceMappings
                var raceMapping = new RaceMapping
                {
                    IrcName = newSectionName,
                    Mappings = new List<Mapping>(),
                    Rules = new List<string>(),
                    Skiplists = new List<string>(),
                    DupeRules = newSection.DupeRules
                };
                raceMappings.Add(raceMapping);

                // Save configuration
                SaveSiteConfiguration();

                // Reload both listboxes
                LoadAvailableSections();
                LoadRaceSections();

                // Clear textbox
                AddIrcTextbox.Clear();

                LogManager.Success($"Added IRC section: {newSectionName}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error adding IRC section: {ex.Message}");
                MessageBox.Show($"Error adding section:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnRemoveIrcSection_Click(object sender, EventArgs e)
        {
            if (ListBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a section to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedSection = ListBox1.SelectedItem.ToString();

            // Confirmation dialog
            var result = MessageBox.Show(
                $"Are you sure you want to permanently delete the IRC section '{selectedSection}'?\n\n" +
                "This will remove:\n" +
                "• The section configuration\n" +
                "• All mappings to CBFTP sections\n" +
                "• All rules and settings\n\n" +
                "This action cannot be undone!",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                // Temporarily unhook event handlers to prevent re-triggering
                ListBox1.SelectedIndexChanged -= ListBox1_SelectedIndexChanged;
                ListBox2.SelectedIndexChanged -= ListBox2_SelectedIndexChanged;

                // Remove from currentSite.RaceSectionsEnabled
                currentSite.RaceSectionsEnabled?.Remove(selectedSection);

                // Remove from currentSite.Sections
                var sectionToRemove = currentSite.Sections?
                    .FirstOrDefault(s => s.IrcName == selectedSection);
                if (sectionToRemove != null)
                {
                    currentSite.Sections.Remove(sectionToRemove);
                }

                // Remove from raceMappings
                var mappingToRemove = raceMappings
                    .FirstOrDefault(m => m.IrcName == selectedSection);
                if (mappingToRemove != null)
                {
                    raceMappings.Remove(mappingToRemove);
                }

                // Save configuration
                SaveSiteConfiguration();

                // Reload both listboxes
                LoadAvailableSections();
                LoadRaceSections();

                LogManager.Success($"Deleted IRC section: {selectedSection}");

                // Clear UI
                txtRegex_Trigger.Clear();
                TagRulelistBox.Items.Clear();
                RuleTxtField.Clear();
                txtGlobalRule.Clear();
                ListBox5.Items.Clear();
                ListBox6.Items.Clear();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error removing IRC section: {ex.Message}");
                MessageBox.Show($"Error removing section:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-attach event handlers
                ListBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
                ListBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
            }
        }



        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnableDisableButtons();
        }

        private void UpdateEnableDisableButtons()
        {
            if (ListBox2.SelectedItem == null)
            {
                btnEnableRaceSection.Enabled = false;
                btnDisableRaceSection.Enabled = false;
            }
            else
            {
                var selectedItem = (SectionItem)ListBox2.SelectedItem;
                btnEnableRaceSection.Enabled = !selectedItem.IsEnabled;
                btnDisableRaceSection.Enabled = selectedItem.IsEnabled;
            }
        }



        private void btnDisableRaceSection_Click(object sender, EventArgs e)
        {
            if (ListBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a section to disable.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = (SectionItem)ListBox2.SelectedItem;

            if (!selectedItem.IsEnabled)
            {
                MessageBox.Show($"Section '{selectedItem.SectionName}' is already disabled.",
                    "Already Disabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Disable the section
            selectedItem.IsEnabled = false;

            // Remove from RaceSectionsEnabled
            currentSite.RaceSectionsEnabled?.Remove(selectedItem.SectionName);

            // Use SaveSiteConfiguration instead of SaveRaceSectionsToJson
            SaveSiteConfiguration();
            ListBox2.Invalidate(); // Redraw to update colors
            UpdateEnableDisableButtons();

            LogManager.Success($"Disabled section from racing: {selectedItem.SectionName}");
        }


        private void btnEnableRaceSection_Click(object sender, EventArgs e)
        {
            if (ListBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a section to enable.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = (SectionItem)ListBox2.SelectedItem;

            if (selectedItem.IsEnabled)
            {
                MessageBox.Show($"Section '{selectedItem.SectionName}' is already enabled.",
                    "Already Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Enable the section
            selectedItem.IsEnabled = true;

            // Add to RaceSectionsEnabled
            if (currentSite.RaceSectionsEnabled == null)
                currentSite.RaceSectionsEnabled = new List<string>();

            if (!currentSite.RaceSectionsEnabled.Contains(selectedItem.SectionName))
            {
                currentSite.RaceSectionsEnabled.Add(selectedItem.SectionName);
            }

            // Use SaveSiteConfiguration instead of SaveRaceSectionsToJson
            SaveSiteConfiguration();
            ListBox2.Invalidate(); // Redraw to update colors
            UpdateEnableDisableButtons();

            LogManager.Success($"Enabled section for racing: {selectedItem.SectionName}");
        }




























        private void Exit_button_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }



        private void UpdateListBox5(string ircSection)
        {
            if (string.IsNullOrWhiteSpace(ircSection))
                return;

            // Clear ListBox5
            ListBox5.Items.Clear();

            // Find the mapping for the selected IRC Section
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == ircSection);
            if (mapping != null && mapping.Mappings != null)
            {
                foreach (var map in mapping.Mappings)
                {
                    ListBox5.Items.Add(map.MapCbftpSection); // Add each mapped CBFTP section
                }
            }

            // Automatically select the first item in ListBox5
            if (ListBox5.Items.Count > 0)
            {
                ListBox5.SelectedIndex = 0; // Select the first item

                // Let ListBox5_SelectedIndexChanged populate trigger + tag rules UI
                ListBox5_SelectedIndexChanged(ListBox5, EventArgs.Empty);
            }
            else
            {
                // Clear trigger + tag rules if there are no mappings
                txtRegex_Trigger.Text = string.Empty;
                TagRulelistBox.Items.Clear();
                RuleTxtField.Clear();
            }
        }



        private void btnMap_Click(object sender, EventArgs e)
        {
            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = lstCbftpSections.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) || string.IsNullOrWhiteSpace(selectedCbftpSection))
            {
                MessageBox.Show("Please select both an IRC Section and a CBFTP Section.");
                return;
            }

            // -----------------------------
            // 1) Update in-memory RaceMapping list
            // -----------------------------
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping == null)
            {
                mapping = new RaceMapping
                {
                    IrcName = selectedIrcSection,
                    Mappings = new List<Mapping>()
                };
                raceMappings.Add(mapping);
            }

            var existingMapping = mapping.Mappings
                .FirstOrDefault(m => m.MapCbftpSection.Equals(selectedCbftpSection, StringComparison.OrdinalIgnoreCase));

            if (existingMapping != null)
            {
                MessageBox.Show(
                    $"CBFTP Section '{selectedCbftpSection}' is already mapped to IRC Section '{selectedIrcSection}'.",
                    "Already Mapped",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            mapping.Mappings.Add(new Mapping
            {
                MapCbftpSection = selectedCbftpSection,
                TriggerRegex = ".*",          // default trigger regex
                Rules = new List<string>()
            });

            // -----------------------------
            // 2) Keep currentSite.Sections in sync
            // -----------------------------
            if (currentSite.Sections == null)
                currentSite.Sections = new List<Section>();

            var section = currentSite.Sections
                .FirstOrDefault(s => s.IrcName.Equals(selectedIrcSection, StringComparison.OrdinalIgnoreCase));

            if (section == null)
            {
                section = new Section
                {
                    IrcName = selectedIrcSection,
                    Tags = new List<Tag>(),
                    Rules = new List<string>(),
                    Skiplists = new List<string>(),
                    DupeRules = new DupeRules()
                };
                currentSite.Sections.Add(section);
            }

            if (section.Tags == null)
                section.Tags = new List<Tag>();

            if (!section.Tags.Any(t => t.MapCbftpSection.Equals(selectedCbftpSection, StringComparison.OrdinalIgnoreCase)))
            {
                section.Tags.Add(new Tag
                {
                    MapCbftpSection = selectedCbftpSection,
                    TriggerRegex = ".*",
                    Rules = new List<string>()
                });
            }

            // -----------------------------
            // 3) Save + refresh UI
            // -----------------------------
            SaveSiteConfiguration(); // same method you already have
            LogManager.Success($"Mapped CBFTP Section '{selectedCbftpSection}' to IRC Section '{selectedIrcSection}'.");

            UpdateListBox5(selectedIrcSection);
        }

        private void btnUnmap_Click(object sender, EventArgs e)
        {
            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) || string.IsNullOrWhiteSpace(selectedCbftpSection))
            {
                MessageBox.Show(
                    "Please select both an IRC Section and a mapped CBFTP Section to unmap.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            bool removedSomething = false;

            // -----------------------------
            // 1) Remove from in-memory RaceMapping list
            // -----------------------------
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping != null)
            {
                var cbftpMapping = mapping.Mappings
                    .FirstOrDefault(m => m.MapCbftpSection.Equals(selectedCbftpSection, StringComparison.OrdinalIgnoreCase));

                if (cbftpMapping != null)
                {
                    mapping.Mappings.Remove(cbftpMapping);
                    removedSomething = true;
                }
            }

            // -----------------------------
            // 2) Remove from currentSite.Sections[*].Tags
            // -----------------------------
            var section = currentSite.Sections?
                .FirstOrDefault(s => s.IrcName.Equals(selectedIrcSection, StringComparison.OrdinalIgnoreCase));

            if (section?.Tags != null)
            {
                var tagToRemove = section.Tags
                    .FirstOrDefault(t => t.MapCbftpSection.Equals(selectedCbftpSection, StringComparison.OrdinalIgnoreCase));

                if (tagToRemove != null)
                {
                    section.Tags.Remove(tagToRemove);
                    removedSomething = true;
                }
            }

            if (!removedSomething)
            {
                MessageBox.Show(
                    $"CBFTP Section '{selectedCbftpSection}' is not mapped to IRC Section '{selectedIrcSection}'.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // -----------------------------
            // 3) Save + refresh UI
            // -----------------------------
            SaveSiteConfiguration(); // same method you already have
            LogManager.Success($"Unmapped CBFTP Section '{selectedCbftpSection}' from IRC Section '{selectedIrcSection}'.");

            UpdateListBox5(selectedIrcSection); // Refresh mapped section list
        }

        /// <summary>
        /// Persists the channel/blowfish-key listbox into currentSite.SiteSettings
        /// (encrypting each key). Called by EVERY save path so channel keys are never
        /// dropped by a save that didn't come from the main "Save site" button.
        /// </summary>
        private void ApplyChannelListToSiteSettings()
        {
            if (currentSite?.SiteSettings == null || ListboxChannels == null)
                return;

            int channelIndex = 1;
            foreach (ChannelInfo channelInfo in ListboxChannels.Items)
            {
                var channelProp = currentSite.SiteSettings.GetType().GetProperty($"Chan{channelIndex}");
                var keyProp = currentSite.SiteSettings.GetType().GetProperty($"BlowfishKey{channelIndex}");
                if (channelProp != null && keyProp != null)
                {
                    channelProp.SetValue(currentSite.SiteSettings, channelInfo.Channel);
                    keyProp.SetValue(currentSite.SiteSettings, SecureConfig.Encrypt(channelInfo.BlowfishKey));
                }
                channelIndex++;
                if (channelIndex > 20) break;
            }

            for (int i = channelIndex; i <= 20; i++)
            {
                var channelProp = currentSite.SiteSettings.GetType().GetProperty($"Chan{i}");
                var keyProp = currentSite.SiteSettings.GetType().GetProperty($"BlowfishKey{i}");
                if (channelProp != null && keyProp != null)
                {
                    channelProp.SetValue(currentSite.SiteSettings, "");
                    keyProp.SetValue(currentSite.SiteSettings, "");
                }
            }

            // Chatbox parity: push the RAW keys straight into the running IRC clients so
            // they take effect immediately - no restart and no DPAPI round-trip involved.
            var siteName = currentSite.SiteSettings.Sitename;
            if (!string.IsNullOrWhiteSpace(siteName))
            {
                var liveKeys = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (ChannelInfo ci in ListboxChannels.Items)
                {
                    if (!string.IsNullOrWhiteSpace(ci.Channel) && !string.IsNullOrWhiteSpace(ci.BlowfishKey))
                        liveKeys[ci.Channel] = ci.BlowfishKey;
                }

                MainApp.ApplyChannelKeysToRunningClients(siteName, liveKeys);
            }
        }

        private void SaveSiteConfiguration()
        {
            try
            {
                if (currentSite == null)
                {
                    MessageBox.Show("No site configuration loaded to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Always fold the current channel/blowfish-key list into the config
                // so no save path drops the keys.
                ApplyChannelListToSiteSettings();

                if (File.Exists(currentSiteFilePath))
                {
                    var existingJsonContent = File.ReadAllText(currentSiteFilePath);
                    var existingSiteConfig = JsonConvert.DeserializeObject<SiteConfig>(existingJsonContent);

                    if (existingSiteConfig != null)
                    {

                        existingSiteConfig.Sections = currentSite.Sections;

                        // Update other fields
                        existingSiteConfig.Server = currentSite.Server;
                        existingSiteConfig.SiteSettings = currentSite.SiteSettings;

                        // Save race_sections_enabled properly from ListBox2
                        existingSiteConfig.RaceSectionsEnabled = ListBox2.Items
                            .Cast<SectionItem>()
                            .Where(s => s.IsEnabled)
                            .Select(s => s.SectionName)
                            .Distinct()
                            .ToList();

                        existingSiteConfig.GlobalBlacklist = currentSite.GlobalBlacklist;
                        existingSiteConfig.Affils = currentSite.Affils;
                        // Serialize and save
                        var updatedJsonContent = JsonConvert.SerializeObject(existingSiteConfig, Formatting.Indented);
                        File.WriteAllText(currentSiteFilePath, updatedJsonContent);

                        LogManager.Success("Site configuration saved");
                    }
                    else
                    {
                        MessageBox.Show("Failed to load existing site configuration for merging.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // If no existing file, save the currentSite as new
                    var newJsonContent = JsonConvert.SerializeObject(currentSite, Formatting.Indented);
                    File.WriteAllText(currentSiteFilePath, newJsonContent);
                    MessageBox.Show("Site configuration saved as new!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving site configuration: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }















        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected IRC Section
            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedIrcSection))
                return;

            // Update ListBox5 (mapped CBFTP sections) for this IRC section
            UpdateListBox5(selectedIrcSection);

            // Clear all selections in lstCbftpSections
            lstCbftpSections.ClearSelected();

            // Ensure multi-select mode is enabled
            lstCbftpSections.SelectionMode = SelectionMode.MultiExtended;

            // Find the mapping for the selected IRC Section
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping != null && mapping.Mappings != null)
            {
                // Loop through all mappings and select corresponding CBFTP sections in lstCbftpSections
                foreach (var map in mapping.Mappings)
                {
                    for (int i = 0; i < lstCbftpSections.Items.Count; i++)
                    {
                        if (lstCbftpSections.Items[i].ToString()
                                .Equals(map.MapCbftpSection, StringComparison.OrdinalIgnoreCase))
                        {
                            lstCbftpSections.SetSelected(i, true); // Set the item as selected
                        }
                    }
                }
            }

            // Update ListBox6 (Global Rules for the selected IRC Section)
            ListBox6.Items.Clear();
            var selectedSection = currentSite.Sections.FirstOrDefault(s => s.IrcName == selectedIrcSection);

            if (selectedSection != null && selectedSection.Rules != null)
            {
                foreach (var rule in selectedSection.Rules)
                {
                    ListBox6.Items.Add(rule);
                }
            }
        }


        private void ListBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected rule in the ListBox
            var selectedRule = ListBox6.SelectedItem?.ToString();

            // Check if the selected item is a section or a specific rule
            if (string.IsNullOrWhiteSpace(selectedRule))
            {
                txtGlobalRule.Clear(); // Clear the text box if nothing is selected
                return;
            }

            // Check if the selected rule matches an enabled section (old functionality)
            var section = currentSite.Sections.FirstOrDefault(s => s.IrcName == selectedRule);

            if (section != null)
            {
                // Populate the global rules as a newline-separated string (old functionality)
                txtGlobalRule.Text = section.Rules != null && section.Rules.Any()
                    ? string.Join(Environment.NewLine, section.Rules)
                    : string.Empty;

                LogManager.Success($"Loaded rules for section '{selectedRule}'.");
            }
            else if (ListBox6.Items.Contains(selectedRule))
            {
                // Populate the text box with the specific rule for editing
                txtGlobalRule.Text = selectedRule;
            }
            else
            {
                // If no match is found, clear the text box and inform the user
                txtGlobalRule.Clear();
                MessageBox.Show($"No match found for '{selectedRule}'.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }






        private void ListBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) ||
                string.IsNullOrWhiteSpace(selectedCbftpSection))
            {
                txtRegex_Trigger.Clear();
                TagRulelistBox.Items.Clear();
                RuleTxtField.Clear();
                return;
            }

            // Find the mapping for the selected IRC Section
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping == null)
            {
                txtRegex_Trigger.Clear();
                TagRulelistBox.Items.Clear();
                RuleTxtField.Clear();
                return;
            }

            // Find the mapping for the selected CBFTP Section
            var cbftpMapping = mapping.Mappings
                .FirstOrDefault(m => m.MapCbftpSection == selectedCbftpSection);

            if (cbftpMapping == null)
            {
                txtRegex_Trigger.Clear();
                TagRulelistBox.Items.Clear();
                RuleTxtField.Clear();
                return;
            }

            // Populate the Trigger Regex
            txtRegex_Trigger.Text = cbftpMapping.TriggerRegex;

            // Populate Tag rules list
            TagRulelistBox.Items.Clear();
            if (cbftpMapping.Rules != null && cbftpMapping.Rules.Any())
            {
                foreach (var rule in cbftpMapping.Rules)
                {
                    TagRulelistBox.Items.Add(rule);
                }
            }

            // Clear the edit box
            RuleTxtField.Clear();
        }


        private void TagRulelistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected rule in the ListBox
            var selectedRule = TagRulelistBox.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedRule))
            {
                RuleTxtField.Clear();
                return;
            }

            // Populate the text field with the selected rule for editing
            RuleTxtField.Text = selectedRule;
        }



        private void AddTagRuleButton_Click(object sender, EventArgs e)
        {
            var newRule = RuleTxtField.Text.Trim();

            if (string.IsNullOrEmpty(newRule))
            {
                MessageBox.Show("Please enter a valid tag rule.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) ||
                string.IsNullOrWhiteSpace(selectedCbftpSection))
            {
                MessageBox.Show("Please select an IRC Section and a CBFTP Section first.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TagRulelistBox.Items.Contains(newRule))
            {
                MessageBox.Show("This tag rule already exists.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1) Update UI list
            TagRulelistBox.Items.Add(newRule);
            RuleTxtField.Clear();

            // 2) Update raceMappings in memory
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping != null)
            {
                var cbftpMapping = mapping.Mappings
                    .FirstOrDefault(m => m.MapCbftpSection == selectedCbftpSection);

                if (cbftpMapping != null)
                {
                    if (cbftpMapping.Rules == null)
                        cbftpMapping.Rules = new List<string>();

                    cbftpMapping.Rules.Add(newRule);

                    // 3) Keep SiteConfig in sync
                    SyncTagRulesToCurrentSite(selectedIrcSection, selectedCbftpSection, cbftpMapping);

                    // 4) Persist to JSON
                    SaveSiteConfiguration();
                    LogManager.Success("Tag rule added");
                }
            }
        }

        private void RemoveTagRuleButton_Click(object sender, EventArgs e)
        {
            if (TagRulelistBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tag rule to remove.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var ruleToRemove = TagRulelistBox.SelectedItem.ToString();
            TagRulelistBox.Items.Remove(ruleToRemove);

            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) ||
                string.IsNullOrWhiteSpace(selectedCbftpSection))
                return;

            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping == null) return;

            var cbftpMapping = mapping.Mappings
                .FirstOrDefault(m => m.MapCbftpSection == selectedCbftpSection);

            if (cbftpMapping?.Rules != null)
            {
                cbftpMapping.Rules.Remove(ruleToRemove);

                // Keep SiteConfig in sync
                SyncTagRulesToCurrentSite(selectedIrcSection, selectedCbftpSection, cbftpMapping);

                SaveSiteConfiguration();
                LogManager.Success("Tag rule removed");
            }
        }


        private void SaveTagButton_Click(object sender, EventArgs e)
        {
            var updatedRule = RuleTxtField.Text.Trim();

            if (string.IsNullOrEmpty(updatedRule))
            {
                MessageBox.Show("Please enter a valid tag rule.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TagRulelistBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tag rule to edit.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedIndex = TagRulelistBox.SelectedIndex;

            // 1) Update ListBox
            TagRulelistBox.Items[selectedIndex] = updatedRule;

            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) ||
                string.IsNullOrWhiteSpace(selectedCbftpSection))
                return;

            // 2) Update raceMappings
            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping == null) return;

            var cbftpMapping = mapping.Mappings
                .FirstOrDefault(m => m.MapCbftpSection == selectedCbftpSection);

            if (cbftpMapping?.Rules != null && selectedIndex < cbftpMapping.Rules.Count)
            {
                cbftpMapping.Rules[selectedIndex] = updatedRule;

                // 3) Sync to SiteConfig
                SyncTagRulesToCurrentSite(selectedIrcSection, selectedCbftpSection, cbftpMapping);

                // 4) Persist
                SaveSiteConfiguration();
                LogManager.Success("Tag rule updated");
            }

            RuleTxtField.Clear();
        }






        private void btnAddGlobalRule_Click(object sender, EventArgs e)
        {
            var newRule = txtGlobalRule.Text.Trim();

            if (string.IsNullOrEmpty(newRule))
            {
                MessageBox.Show("Please enter a valid rule.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ListBox6.Items.Contains(newRule))
            {
                ListBox6.Items.Add(newRule); // Add the rule to the ListBox
                txtGlobalRule.Clear(); // Clear the textbox

                // Update the corresponding section rules
                var selectedSectionName = ListBox1.SelectedItem?.ToString();
                if (!string.IsNullOrWhiteSpace(selectedSectionName))
                {
                    var selectedSection = currentSite.Sections.FirstOrDefault(s => s.IrcName == selectedSectionName);
                    if (selectedSection != null)
                    {
                        selectedSection.Rules.Add(newRule);
                        SaveSiteConfiguration(); // Save changes immediately
                        LogManager.Success("Section rule added");
                    }
                }
            }
            else
            {
                MessageBox.Show("This rule already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void btnRemoveGlobalRule_Click(object sender, EventArgs e)
        {
            if (ListBox6.SelectedItem != null)
            {
                var ruleToRemove = ListBox6.SelectedItem.ToString();
                ListBox6.Items.Remove(ruleToRemove);

                var selectedSectionName = ListBox1.SelectedItem?.ToString();
                if (!string.IsNullOrWhiteSpace(selectedSectionName))
                {
                    var selectedSection = currentSite.Sections.FirstOrDefault(s => s.IrcName == selectedSectionName);
                    if (selectedSection != null)
                    {
                        selectedSection.Rules.Remove(ruleToRemove);
                        SaveSiteConfiguration(); // Save the updated configuration
                        LogManager.Success("Section rule removed");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a rule to remove.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void btnSaveGlobalRules_Click(object sender, EventArgs e)
        {
            var updatedRule = txtGlobalRule.Text.Trim();

            if (string.IsNullOrEmpty(updatedRule))
            {
                MessageBox.Show("Please enter a valid rule.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ListBox6.SelectedItem != null)
            {
                int selectedIndex = ListBox6.SelectedIndex;

                // Update the rule in the ListBox
                ListBox6.Items[selectedIndex] = updatedRule;

                // Update the rule in the configuration
                var selectedSectionName = ListBox1.SelectedItem?.ToString();
                if (!string.IsNullOrWhiteSpace(selectedSectionName))
                {
                    var selectedSection = currentSite.Sections.FirstOrDefault(s => s.IrcName == selectedSectionName);
                    if (selectedSection != null && selectedIndex < selectedSection.Rules.Count)
                    {
                        selectedSection.Rules[selectedIndex] = updatedRule;
                        SaveSiteConfiguration(); // Save the updated configuration
                        LogManager.Success("Section rule updated");
                    }
                }

                txtGlobalRule.Clear(); // Clear the text box after saving

            }
            else
            {
                MessageBox.Show("Please select a rule to edit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }






        private void btnSaveRegexRules_Click(object sender, EventArgs e)
        {
            var selectedIrcSection = ListBox1.SelectedItem?.ToString();
            var selectedCbftpSection = ListBox5.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedIrcSection) || string.IsNullOrWhiteSpace(selectedCbftpSection))
            {
                MessageBox.Show(
                    "Please select both an IRC Section and a CBFTP Section to save the trigger regex.",
                    "Validation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var mapping = raceMappings.FirstOrDefault(m => m.IrcName == selectedIrcSection);
            if (mapping == null)
            {
                MessageBox.Show(
                    $"No mapping found for the IRC Section '{selectedIrcSection}'.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var cbftpMapping = mapping.Mappings
                .FirstOrDefault(m => m.MapCbftpSection == selectedCbftpSection);
            if (cbftpMapping == null)
            {
                MessageBox.Show(
                    $"No mapping found for the CBFTP Section '{selectedCbftpSection}'.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Save ONLY the trigger regex (default to .* when empty)
            cbftpMapping.TriggerRegex = string.IsNullOrWhiteSpace(txtRegex_Trigger.Text)
                ? ".*"
                : txtRegex_Trigger.Text.Trim();

            // Keep currentSite.Sections in sync (so JSON follows)
            var section = currentSite.Sections?
                .FirstOrDefault(s => s.IrcName.Equals(selectedIrcSection, StringComparison.OrdinalIgnoreCase));

            if (section?.Tags != null)
            {
                var tag = section.Tags
                    .FirstOrDefault(t => t.MapCbftpSection.Equals(selectedCbftpSection, StringComparison.OrdinalIgnoreCase));

                if (tag != null)
                {
                    tag.TriggerRegex = cbftpMapping.TriggerRegex;
                }
            }

            SaveSiteConfiguration();
            LogManager.Success("Trigger regex saved");
        }


        private void SaveOnlyRegexAndRules(RaceMapping mapping, Mapping cbftpMapping)
        {
            try
            {
                // Load the existing JSON file
                var siteData = File.Exists(currentSiteFilePath)
                    ? JsonConvert.DeserializeObject<SiteConfig>(File.ReadAllText(currentSiteFilePath))
                    : new SiteConfig();

                if (siteData.Sections == null)
                    siteData.Sections = new List<Section>();

                var section = siteData.Sections
                    .FirstOrDefault(s => s.IrcName == mapping.IrcName);

                if (section != null && section.Tags != null)
                {
                    var tag = section.Tags
                        .FirstOrDefault(t => t.MapCbftpSection == cbftpMapping.MapCbftpSection);

                    if (tag != null)
                    {
                        // Only update trigger regex
                        tag.TriggerRegex = cbftpMapping.TriggerRegex;
                        // DO NOT touch tag.Rules here – Tag rules are edited elsewhere
                    }
                }

                // Save the updated JSON
                File.WriteAllText(
                    currentSiteFilePath,
                    JsonConvert.SerializeObject(siteData, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving trigger regex: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }









        private void Test_regex_button_Click(object sender, EventArgs e)
        {
            string input = txtBox_test_regex.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please paste a valid line to test.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Retrieve regex patterns from the form fields
                string newRegexPattern = New_Field_regex.Text;
                string ignoreWordsText = Ignore_Word_regex.Text;
                string sectionRegexPattern = Section_Field_regex.Text;
                string releaseRegexPattern = Name_Field_regex.Text;

                // Prefix and suffix from the form
                string sectionPrefix = Section_Prefix.Text ?? string.Empty;
                string sectionSuffix = Section_Suffix.Text ?? string.Empty;

                // Parse ignore words into a list
                List<string> ignoreWords = (ignoreWordsText ?? string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => word.Trim())
                    .ToList();

                // Match "NEW"
                var newMatch = Regex.IsMatch(input, newRegexPattern, RegexOptions.IgnoreCase);
                if (!newMatch)
                {
                    MessageBox.Show("No 'NEW' found in the input.", "Regex Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Match Section
                var sectionMatch = Regex.Match(input, sectionRegexPattern, RegexOptions.IgnoreCase);
                string section = sectionMatch.Success ? sectionMatch.Groups[1].Value : "N/A";

                // Apply prefix and suffix trimming if applicable
                if (!string.IsNullOrEmpty(sectionPrefix) && section.StartsWith(sectionPrefix))
                {
                    section = section.Substring(sectionPrefix.Length);
                }

                if (!string.IsNullOrEmpty(sectionSuffix) && section.EndsWith(sectionSuffix))
                {
                    section = section.Substring(0, section.Length - sectionSuffix.Length);
                }

                // Match Release Name
                var releaseMatch = Regex.Match(input, releaseRegexPattern, RegexOptions.IgnoreCase);
                string releaseName = releaseMatch.Success ? releaseMatch.Groups[1].Value : "N/A";

                // Check ignored words
                string ignoredWordFound = ignoreWords.FirstOrDefault(word =>
                    input.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0);

                // Prepare results
                string resultMessage = $"NEW Detected!\n" +
                                       $"Section: {section}\n" +
                                       $"Release: {releaseName}\n" +
                                       $"Ignored Word Found: {(ignoredWordFound ?? "None")}";

                // Show results in a popup
                MessageBox.Show(resultMessage, "Regex Test Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Regex Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void TVMazeSettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                var siteName = textBox8.Text?.Trim();
                if (string.IsNullOrEmpty(siteName))
                {
                    MessageBox.Show("Please enter or select a site name first.", "No Site Name",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var siteFile = Path.Combine("sites", $"{siteName}.json");
                if (!File.Exists(siteFile))
                {
                    MessageBox.Show($"Site configuration '{siteName}' does not exist.\n\nPlease save the site first.",
                        "Site Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // CLEAN VERSION - no debug messages
                var sectionSettingsForm = new SectionSettingsForm(siteName);
                sectionSettingsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening section settings");
                MessageBox.Show($"Error opening section settings:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }








        private async void site_rules_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Sitename from textbox (you already use this as the site name)
                string siteName = textBox8.Text?.Trim();

                if (string.IsNullOrWhiteSpace(siteName))
                {
                    MessageBox.Show(
                        "Please enter/save a site first (Sitename).",
                        "No Site",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // 🔸 Call cbftp
                var rulesText = await CbftpRacer.GetSiteRulesTextAsync(siteName);

                // Show in non-blocking rules form
                var frm = new SiteRulesForm
                {
                    Text = $"Site Rules - {siteName}"
                };
                frm.SetRulesText(string.IsNullOrWhiteSpace(rulesText)
                    ? "No rules returned from CBFTP."
                    : rulesText);

                // modeless
                frm.Show(this);
                frm.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error loading site rules:\r\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }




        /// <summary>
        /// Keep currentSite.Sections.Tags.Rules in sync with a Mapping from raceMappings.
        /// </summary>
        private void SyncTagRulesToCurrentSite(string ircSection, string cbftpSection, Mapping cbftpMapping)
        {
            if (currentSite?.Sections == null)
                return;

            var section = currentSite.Sections
                .FirstOrDefault(s => s.IrcName.Equals(ircSection, StringComparison.OrdinalIgnoreCase));

            if (section?.Tags == null)
                return;

            var tag = section.Tags
                .FirstOrDefault(t => t.MapCbftpSection.Equals(cbftpSection, StringComparison.OrdinalIgnoreCase));

            if (tag == null)
                return;

            // Copy the rules from the Mapping to the Tag in the SiteConfig
            tag.Rules = cbftpMapping.Rules != null
                ? new List<string>(cbftpMapping.Rules)
                : new List<string>();
        }

        private void Test_pre_regex_button_Click(object sender, EventArgs e)
        {
            string input = txtBox_test_pre_regex.Text;
            if (string.IsNullOrWhiteSpace(input))
            {
                MessageBox.Show("Please paste a valid PRE line to test.",
                    "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Retrieve PRE regex patterns from the form fields
                string preRegexPattern = Pre_field_regex.Text;
                string preSectionRegexPattern = Section_pre_field.Text;
                string preReleaseRegexPattern = Release_Pre_field.Text;

                // PRE section prefix/suffix
                string preSectionPrefix = Section_pre_prefix_field.Text ?? string.Empty;
                string preSectionSuffix = Section_pre_suffix_field.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(preRegexPattern))
                {
                    MessageBox.Show("Please enter a PRE regex pattern in 'Pre_field_regex'.",
                        "Regex Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Case-sensitive PRE detection (no IgnoreCase, no @)
                var preMatch = Regex.IsMatch(input, preRegexPattern);
                if (!preMatch)
                {
                    MessageBox.Show("No 'PRE' match found in the input using 'Pre_field_regex'.",
                        "Regex Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ----- SECTION -----
                string section = "N/A";
                if (!string.IsNullOrWhiteSpace(preSectionRegexPattern))
                {
                    var sectionMatch = Regex.Match(input, preSectionRegexPattern);
                    if (sectionMatch.Success && sectionMatch.Groups.Count > 1)
                    {
                        section = sectionMatch.Groups[1].Value;
                    }

                    // Apply PRE section prefix/suffix trimming if applicable
                    if (!string.IsNullOrEmpty(preSectionPrefix) && section.StartsWith(preSectionPrefix))
                    {
                        section = section.Substring(preSectionPrefix.Length);
                    }

                    if (!string.IsNullOrEmpty(preSectionSuffix) && section.EndsWith(preSectionSuffix))
                    {
                        section = section.Substring(0, section.Length - preSectionSuffix.Length);
                    }
                }

                // ----- RELEASE -----
                string releaseName = "N/A";
                if (!string.IsNullOrWhiteSpace(preReleaseRegexPattern))
                {
                    var releaseMatch = Regex.Match(input, preReleaseRegexPattern);
                    if (releaseMatch.Success && releaseMatch.Groups.Count > 1)
                    {
                        releaseName = releaseMatch.Groups[1].Value;
                    }
                }

                // Result
                string resultMessage = $"PRE Detected!\n" +
                                       $"Section: {section}\n" +
                                       $"Release: {releaseName}";

                MessageBox.Show(resultMessage, "PRE Regex Test Results",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "PRE Regex Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Edit_Cbftp_Site_Click(object sender, EventArgs e)
        {
            try
            {
                var siteName = textBox8.Text?.Trim();
                if (string.IsNullOrEmpty(siteName))
                {
                    MessageBox.Show(
                        "Please enter or select a site name first.",
                        "No Site Name",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var frm = new CbftpAddSiteForm(
                    CbftpAddSiteForm.CbftpSiteFormMode.Edit,
                    null,
                    siteName);

                frm.StartPosition = FormStartPosition.CenterParent;
                frm.Show(this);       // 👈 modeless, main window stays usable
                frm.BringToFront();
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening CBFTP edit site form");
                MessageBox.Show(
                    $"Error opening CBFTP site editor:\r\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

    }

    public class RaceMapping
    {
        public string IrcName { get; set; } // IRC Announce Section
        public List<Mapping> Mappings { get; set; } = new List<Mapping>(); // Associated CBFTP Sections (Tags)
        public List<string> Rules { get; set; } = new List<string>(); // Rules for the IRC section
        public List<string> Skiplists { get; set; } = new List<string>(); // Skip lists for the IRC section
        public DupeRules DupeRules { get; set; } // Dupe rules for the section
    }

    public class ChannelInfo
    {
        public string Channel { get; set; }
        public string BlowfishKey { get; set; }

        // Display only channel name in listbox
        public override string ToString()
        {
            return Channel;
        }
    }

    public class SectionItem
    {
        public string SectionName { get; set; }
        public bool IsEnabled { get; set; }

        public override string ToString()
        {
            return SectionName;
        }
    }




}
