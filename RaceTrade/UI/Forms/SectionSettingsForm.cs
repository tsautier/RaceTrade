using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using RaceTrade;

namespace RaceTrader
{
    /// <summary>
    /// Section-specific settings form for IMDB and TVMaze filtering
    /// Each section can have its own IMDB (movies) or TVMaze (TV) configuration
    /// </summary>
    public class SectionSettingsForm : AntdUI.Window
    {
        private ComboBox siteComboBox;
        private ComboBox sectionComboBox;
        private TabControl tabControl;
        private string currentSiteName;

        // IMDB tab controls
        private CheckBox imdbEnabledCheckBox;
        private NumericUpDown imdbMinRatingNumeric;
        private NumericUpDown imdbMinVotesNumeric;
        private CheckedListBox imdbAllowedGenresListBox;
        private CheckedListBox imdbBlockedGenresListBox;
        private CheckBox onlyEnglishCheckBox;
        private CheckBox onlyUSCountryCheckBox;
        private CheckBox noDocumentaryCheckBox;
        private CheckBox noMusicCheckBox;
        private CheckBox noComedyCheckBox;
        private CheckBox noShowCheckBox;
        private CheckBox imdbFallbackCheckBox;

        // TVMaze tab controls
        private CheckBox tvmazeEnabledCheckBox;
        private CheckBox skipEndedShowsCheckBox;
        private NumericUpDown tvmazeMinRatingNumeric;
        private NumericUpDown cacheDaysNumeric;
        private CheckedListBox tvmazeAllowedGenresListBox;
        private CheckedListBox tvmazeBlockedGenresListBox;
        private CheckedListBox allowedNetworksListBox;
        private CheckBox tvmazeFallbackCheckBox;
        private CheckedListBox allowedShowTypesListBox;
        private Button saveButton;
        private Button cancelButton;
        private Button testIMDBButton;
        private Button testTVMazeButton;

        private string currentSiteFile;
        private JObject currentSiteConfig;

        // IMDB genres (movies)
        private readonly List<string> imdbGenres = new List<string>
        {
            "Action", "Adventure", "Animation", "Biography", "Comedy",
            "Crime", "Documentary", "Drama", "Family", "Fantasy",
            "Film-Noir", "History", "Horror", "Music", "Musical",
            "Mystery", "Romance", "Sci-Fi", "Sport", "Thriller",
            "War", "Western"
        };

        // TVMaze genres (TV shows)
        private readonly List<string> tvmazeGenres = new List<string>
        {
            "Action", "Adventure", "Anime", "Children", "Comedy", "Crime",
            "Documentary", "Drama", "Fantasy", "Food", "Game Show", "History",
            "Horror", "Legal", "Medical", "Music", "Mystery", "Nature",
            "Reality", "Romance", "Science-Fiction", "Sports", "Supernatural",
            "Talk Show", "Thriller", "Travel", "War", "Western"
        };

        // Popular networks
        private readonly List<string> popularNetworks = new List<string>
        {
            "HBO", "Netflix", "Amazon", "Apple TV+", "Disney+", "Hulu",
            "Showtime", "AMC", "FX", "CBS", "NBC", "ABC", "FOX", "The CW",
            "Peacock", "Paramount+", "Max", "BBC One", "BBC Two", "Channel 4",
            "Sky Atlantic", "ITV", "Starz", "Epix", "USA Network", "TNT",
            "Syfy", "Comedy Central", "Adult Swim", "Cartoon Network"
        };
        // TVMaze show types
        private readonly List<string> tvmazeShowTypes = new List<string>
        {
        "Scripted", "Animation", "Reality", "Talk Show", "Documentary",
        "Game Show", "News", "Sports", "Variety", "Award Show", "Panel Show"
        };



        public SectionSettingsForm(string siteName)  // ← Add parameter
        {
            this.currentSiteName = siteName;
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);
            LoadSites();
        }

        public SectionSettingsForm()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);
            LoadSites();
        }

        private void InitializeComponent()
        {
            this.Text = "Section Settings - IMDB / TVMaze";
            this.Size = new Size(920, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(22, 26, 36);  // Dark grey background
            this.Font = new Font("Cascadia Mono", 8.25f);  // Default font

            int yPos = 20;

            // Site selection
            var siteLabel = new Label
            {
                Text = "Site:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            this.Controls.Add(siteLabel);

            siteComboBox = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            siteComboBox.SelectedIndexChanged += SiteComboBox_SelectedIndexChanged;
            this.Controls.Add(siteComboBox);

            yPos += 35;

            // Section selection
            var sectionLabel = new Label
            {
                Text = "Section:",
                Location = new Point(20, yPos),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            this.Controls.Add(sectionLabel);

            sectionComboBox = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            sectionComboBox.SelectedIndexChanged += SectionComboBox_SelectedIndexChanged;
            this.Controls.Add(sectionComboBox);

            var infoLabel = new Label
            {
                Text = "Configure IMDb (movies) or TVMaze (TV shows) for this section",
                Location = new Point(400, yPos + 3),
                Size = new Size(450, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7f, FontStyle.Italic),
                BackColor = Color.Transparent
            };
            this.Controls.Add(infoLabel);

            yPos += 40;

            // Tab control
            tabControl = new TabControl
            {
                Location = new Point(20, yPos),
                Size = new Size(860, 590),
                BackColor = Color.FromArgb(22, 26, 36),
                Font = new Font("Cascadia Mono", 8.25f)
            };
            this.Controls.Add(tabControl);

            // Create tabs
            var imdbTab = new TabPage("IMDb (Movies)");
            var tvmazeTab = new TabPage("TVMaze (TV Shows)");

            tabControl.TabPages.Add(imdbTab);
            tabControl.TabPages.Add(tvmazeTab);

            // Initialize IMDB tab
            InitializeIMDBTab(imdbTab);

            // Initialize TVMaze tab
            InitializeTVMazeTab(tvmazeTab);

            yPos += 600;

            // Bottom buttons with theme
            testIMDBButton = new Button
            {
                Text = "Test IMDb",
                Location = new Point(20, yPos),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(72, 80, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            testIMDBButton.FlatAppearance.BorderSize = 1;
            testIMDBButton.FlatAppearance.BorderColor = Color.Black;
            testIMDBButton.Click += TestIMDBButton_Click;
            this.Controls.Add(testIMDBButton);

            testTVMazeButton = new Button
            {
                Text = "Test TVMaze",
                Location = new Point(150, yPos),
                Size = new Size(120, 30),
                BackColor = Color.FromArgb(72, 80, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            testTVMazeButton.FlatAppearance.BorderSize = 1;
            testTVMazeButton.FlatAppearance.BorderColor = Color.Black;
            testTVMazeButton.Click += TestTVMazeButton_Click;
            this.Controls.Add(testTVMazeButton);

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(650, yPos),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(168, 75, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            cancelButton.FlatAppearance.BorderSize = 1;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);

            saveButton = new Button
            {
                Text = "Save Settings",
                Location = new Point(760, yPos),
                Size = new Size(110, 30),
                BackColor = Color.FromArgb(77, 166, 112),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            saveButton.FlatAppearance.BorderSize = 1;
            saveButton.FlatAppearance.BorderColor = Color.Black;
            saveButton.Click += SaveButton_Click;
            this.Controls.Add(saveButton);
        }

        private void InitializeIMDBTab(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(22, 26, 36);
            int yPos = 15;

            // Enable checkbox
            imdbEnabledCheckBox = new CheckBox
            {
                Text = "Enable IMDb Filtering for this section",
                Location = new Point(20, yPos),
                Size = new Size(360, 25),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(imdbEnabledCheckBox);

            yPos += 35;

            // Rating and Votes
            var ratingLabel = new Label
            {
                Text = "Minimum Rating:",
                Location = new Point(20, yPos + 3),
                Size = new Size(130, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(ratingLabel);

            imdbMinRatingNumeric = new NumericUpDown
            {
                Location = new Point(155, yPos),
                Size = new Size(70, 25),
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.1m,
                Value = 0,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(imdbMinRatingNumeric);

            var votesLabel = new Label
            {
                Text = "Minimum Votes:",
                Location = new Point(250, yPos + 3),
                Size = new Size(120, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(votesLabel);

            imdbMinVotesNumeric = new NumericUpDown
            {
                Location = new Point(380, yPos),
                Size = new Size(100, 25),
                Minimum = 0,
                Maximum = 1000000,
                Increment = 1000,
                Value = 0,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(imdbMinVotesNumeric);

            var votesHint = new Label
            {
                Text = "(e.g. 20000 for 20k votes)",
                Location = new Point(490, yPos + 3),
                Size = new Size(220, 20),
                ForeColor = Color.Gray,
                Font = new Font("Cascadia Mono", 8f)
            };
            tab.Controls.Add(votesHint);

            yPos += 40;

            // Quick filters
            var quickFiltersLabel = new Label
            {
                Text = "Quick Filters:",
                Location = new Point(20, yPos),
                Size = new Size(150, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(quickFiltersLabel);

            yPos += 25;

            int col1 = 30, col2 = 260, col3 = 460;

            onlyEnglishCheckBox = new CheckBox
            {
                Text = "Only English Language",
                Location = new Point(col1, yPos),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(onlyEnglishCheckBox);

            onlyUSCountryCheckBox = new CheckBox
            {
                Text = "Only US Country",
                Location = new Point(col2, yPos),
                Size = new Size(180, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(onlyUSCountryCheckBox);

            imdbFallbackCheckBox = new CheckBox
            {
                Text = "Allow Race on API Error",
                Location = new Point(col3, yPos),
                Size = new Size(220, 20),
                ForeColor = Color.White,
                Checked = true,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(imdbFallbackCheckBox);

            yPos += 30;

            noDocumentaryCheckBox = new CheckBox
            {
                Text = "No Documentary",
                Location = new Point(col1, yPos),
                Size = new Size(180, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(noDocumentaryCheckBox);

            noMusicCheckBox = new CheckBox
            {
                Text = "No Music",
                Location = new Point(col2, yPos),
                Size = new Size(180, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(noMusicCheckBox);

            noComedyCheckBox = new CheckBox
            {
                Text = "No Comedy",
                Location = new Point(col3, yPos),
                Size = new Size(180, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(noComedyCheckBox);

            yPos += 30;

            noShowCheckBox = new CheckBox
            {
                Text = "No Shows (Movies Only)",
                Location = new Point(col1, yPos),
                Size = new Size(220, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(noShowCheckBox);

            yPos += 40;

            // Genre filters
            var genresLabel = new Label
            {
                Text = "Genre Filters:",
                Location = new Point(20, yPos),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(genresLabel);

            yPos += 25;

            // Allowed genres
            var allowedLabel = new Label
            {
                Text = "Allowed Genres (empty = allow all):",
                Location = new Point(20, yPos),
                Size = new Size(300, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7.25f)
            };
            tab.Controls.Add(allowedLabel);

            imdbAllowedGenresListBox = new CheckedListBox
            {
                Location = new Point(20, yPos + 25),
                Size = new Size(380, 200),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(imdbAllowedGenresListBox, imdbGenres);
            tab.Controls.Add(imdbAllowedGenresListBox);

            var selectAllIMDBAllowed = CreateSmallButton("All", new Point(20, yPos + 230), 60);
            selectAllIMDBAllowed.Click += (s, e) => CheckAllItems(imdbAllowedGenresListBox, true);
            tab.Controls.Add(selectAllIMDBAllowed);

            var selectNoneIMDBAllowed = CreateSmallButton("None", new Point(90, yPos + 230), 60);
            selectNoneIMDBAllowed.Click += (s, e) => CheckAllItems(imdbAllowedGenresListBox, false);
            tab.Controls.Add(selectNoneIMDBAllowed);

            // Blocked genres
            var blockedLabel = new Label
            {
                Text = "Blocked Genres:",
                Location = new Point(430, yPos),
                Size = new Size(300, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7.25f)
            };
            tab.Controls.Add(blockedLabel);

            imdbBlockedGenresListBox = new CheckedListBox
            {
                Location = new Point(430, yPos + 25),
                Size = new Size(380, 200),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(imdbBlockedGenresListBox, imdbGenres);
            tab.Controls.Add(imdbBlockedGenresListBox);

            var selectAllIMDBBlocked = CreateSmallButton("All", new Point(430, yPos + 230), 60);
            selectAllIMDBBlocked.Click += (s, e) => CheckAllItems(imdbBlockedGenresListBox, true);
            tab.Controls.Add(selectAllIMDBBlocked);

            var selectNoneIMDBBlocked = CreateSmallButton("None", new Point(500, yPos + 230), 60);
            selectNoneIMDBBlocked.Click += (s, e) => CheckAllItems(imdbBlockedGenresListBox, false);
            tab.Controls.Add(selectNoneIMDBBlocked);
        }

        private void InitializeTVMazeTab(TabPage tab)
        {
            tab.BackColor = Color.FromArgb(22, 26, 36);
            int yPos = 15;

            // Enable checkbox
            tvmazeEnabledCheckBox = new CheckBox
            {
                Text = "Enable TVMaze Filtering for this section",
                Location = new Point(20, yPos),
                Size = new Size(380, 25),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(tvmazeEnabledCheckBox);

            yPos += 35;

            // Settings row 1
            skipEndedShowsCheckBox = new CheckBox
            {
                Text = "Skip Ended/Cancelled Shows",
                Location = new Point(20, yPos),
                Size = new Size(250, 20),
                ForeColor = Color.White,
                Checked = true,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(skipEndedShowsCheckBox);

            tvmazeFallbackCheckBox = new CheckBox
            {
                Text = "Allow Race on API Error",
                Location = new Point(300, yPos),
                Size = new Size(220, 20),
                ForeColor = Color.White,
                Checked = true,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(tvmazeFallbackCheckBox);

            yPos += 30;

            // Settings row 2
            var ratingLabel = new Label
            {
                Text = "Min Rating:",
                Location = new Point(20, yPos + 3),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(ratingLabel);

            tvmazeMinRatingNumeric = new NumericUpDown
            {
                Location = new Point(120, yPos),
                Size = new Size(70, 25),
                Minimum = 0,
                Maximum = 10,
                DecimalPlaces = 1,
                Increment = 0.5m,
                Value = 0,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(tvmazeMinRatingNumeric);

            var cacheLabel = new Label
            {
                Text = "Cache Days:",
                Location = new Point(200, yPos + 3),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(cacheLabel);

            cacheDaysNumeric = new NumericUpDown
            {
                Location = new Point(300, yPos),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 30,
                Value = 7,
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            tab.Controls.Add(cacheDaysNumeric);

            yPos += 40;

            // Show Types (horizontal checkboxes at top)
            var showTypesLabel = new Label
            {
                Text = "Show Type Filters (empty = allow all):",
                Location = new Point(20, yPos),
                Size = new Size(320, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(showTypesLabel);

            yPos += 25;

            allowedShowTypesListBox = new CheckedListBox
            {
                Location = new Point(20, yPos),
                Size = new Size(790, 65),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                MultiColumn = true,
                ColumnWidth = 130,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(allowedShowTypesListBox, tvmazeShowTypes);
            tab.Controls.Add(allowedShowTypesListBox);

            var selectAllShowTypes = CreateSmallButton("All", new Point(20, yPos + 70), 60);
            selectAllShowTypes.Click += (s, e) => CheckAllItems(allowedShowTypesListBox, true);
            tab.Controls.Add(selectAllShowTypes);

            var selectNoneShowTypes = CreateSmallButton("None", new Point(90, yPos + 70), 60);
            selectNoneShowTypes.Click += (s, e) => CheckAllItems(allowedShowTypesListBox, false);
            tab.Controls.Add(selectNoneShowTypes);

            yPos += 105;

            // Genre filters header
            var genresLabel = new Label
            {
                Text = "Genre Filters:",
                Location = new Point(20, yPos),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9f, FontStyle.Bold)
            };
            tab.Controls.Add(genresLabel);

            yPos += 25;

            // THREE COLUMNS: Allowed Genres | Blocked Genres | Networks
            int col1X = 30;
            int col2X = 295;
            int col3X = 560;
            int colWidth = 260;

            // Column 1: Allowed genres
            var allowedLabel = new Label
            {
                Text = "Allowed Genres (empty = allow all):",
                Location = new Point(col1X, yPos),
                Size = new Size(colWidth, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7.25f)
            };
            tab.Controls.Add(allowedLabel);

            tvmazeAllowedGenresListBox = new CheckedListBox
            {
                Location = new Point(col1X, yPos + 25),
                Size = new Size(colWidth, 200),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(tvmazeAllowedGenresListBox, tvmazeGenres);
            tab.Controls.Add(tvmazeAllowedGenresListBox);

            var selectAllTVAllowed = CreateSmallButton("All", new Point(col1X, yPos + 230), 60);
            selectAllTVAllowed.Click += (s, e) => CheckAllItems(tvmazeAllowedGenresListBox, true);
            tab.Controls.Add(selectAllTVAllowed);

            var selectNoneTVAllowed = CreateSmallButton("None", new Point(col1X + 70, yPos + 230), 60);
            selectNoneTVAllowed.Click += (s, e) => CheckAllItems(tvmazeAllowedGenresListBox, false);
            tab.Controls.Add(selectNoneTVAllowed);

            // Column 2: Blocked genres
            var blockedLabel = new Label
            {
                Text = "Blocked Genres:",
                Location = new Point(col2X, yPos),
                Size = new Size(colWidth, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7.25f)
            };
            tab.Controls.Add(blockedLabel);

            tvmazeBlockedGenresListBox = new CheckedListBox
            {
                Location = new Point(col2X, yPos + 25),
                Size = new Size(colWidth, 200),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(tvmazeBlockedGenresListBox, tvmazeGenres);
            tab.Controls.Add(tvmazeBlockedGenresListBox);

            var selectAllTVBlocked = CreateSmallButton("All", new Point(col2X, yPos + 230), 60);
            selectAllTVBlocked.Click += (s, e) => CheckAllItems(tvmazeBlockedGenresListBox, true);
            tab.Controls.Add(selectAllTVBlocked);

            var selectNoneTVBlocked = CreateSmallButton("None", new Point(col2X + 70, yPos + 230), 60);
            selectNoneTVBlocked.Click += (s, e) => CheckAllItems(tvmazeBlockedGenresListBox, false);
            tab.Controls.Add(selectNoneTVBlocked);

            // Column 3: Networks
            var networksLabel = new Label
            {
                Text = "Network Filters (empty = allow all):",
                Location = new Point(col3X, yPos),
                Size = new Size(colWidth, 20),
                ForeColor = Color.LightGray,
                Font = new Font("Cascadia Mono", 7.25f)
            };
            tab.Controls.Add(networksLabel);

            allowedNetworksListBox = new CheckedListBox
            {
                Location = new Point(col3X, yPos + 25),
                Size = new Size(colWidth, 200),
                BackColor = Color.FromArgb(33, 38, 50),
                ForeColor = Color.White,
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            PopulateListBox(allowedNetworksListBox, popularNetworks);
            tab.Controls.Add(allowedNetworksListBox);

            var selectAllNetworks = CreateSmallButton("All", new Point(col3X, yPos + 230), 60);
            selectAllNetworks.Click += (s, e) => CheckAllItems(allowedNetworksListBox, true);
            tab.Controls.Add(selectAllNetworks);

            var selectNoneNetworks = CreateSmallButton("None", new Point(col3X + 70, yPos + 230), 60);
            selectNoneNetworks.Click += (s, e) => CheckAllItems(allowedNetworksListBox, false);
            tab.Controls.Add(selectNoneNetworks);
        }

        private Button CreateSmallButton(string text, Point location, int width)
        {
            var button = new Button
            {
                Text = text,
                Location = location,
                Size = new Size(width, 25),
                BackColor = Color.FromArgb(72, 80, 98),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Cascadia Mono", 8.25f)
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Color.Black;
            return button;
        }

        private void PopulateListBox(CheckedListBox listBox, List<string> items)
        {
            listBox.Items.Clear();
            foreach (var item in items.OrderBy(i => i))
            {
                listBox.Items.Add(item);
            }
        }

        private void CheckAllItems(CheckedListBox listBox, bool check)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                listBox.SetItemChecked(i, check);
            }
        }

        private void LoadSites()
        {
            siteComboBox.Items.Clear();

            if (string.IsNullOrEmpty(currentSiteName))
            {
                MessageBox.Show("No site specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Only add the current site being edited
            siteComboBox.Items.Add(currentSiteName);
            siteComboBox.SelectedIndex = 0;
            siteComboBox.Enabled = false; // Lock to current site
        }

        private void SiteComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (siteComboBox.SelectedItem == null)
                return;

            var siteName = siteComboBox.SelectedItem.ToString();
            currentSiteFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", $"{siteName}.json");

            LoadSections();
        }

        private void LoadSections()
        {
            sectionComboBox.Items.Clear();

            if (string.IsNullOrEmpty(currentSiteFile) || !File.Exists(currentSiteFile))
                return;

            try
            {
                var json = File.ReadAllText(currentSiteFile);
                currentSiteConfig = JObject.Parse(json);

                var sections = currentSiteConfig["sections"] as JArray;
                if (sections != null)
                {
                    foreach (var section in sections)
                    {
                        var ircName = section["irc_name"]?.ToString();
                        if (!string.IsNullOrEmpty(ircName))
                        {
                            sectionComboBox.Items.Add(ircName);
                        }
                    }
                }

                if (sectionComboBox.Items.Count > 0)
                {
                    sectionComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading sections: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            // Just close - no DialogResult needed
            this.Close();
        }

        private void SectionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sectionComboBox.SelectedItem == null)
                return;

            LoadSectionSettings();
        }

        private void LoadSectionSettings()
        {
            if (currentSiteConfig == null || sectionComboBox.SelectedItem == null)
                return;

            try
            {
                var sectionName = sectionComboBox.SelectedItem.ToString();
                var sections = currentSiteConfig["sections"] as JArray;
                var section = sections?.FirstOrDefault(s => s["irc_name"]?.ToString() == sectionName) as JObject;

                if (section == null)
                {
                    SetDefaultSettings();
                    return;
                }

                // Load IMDB settings
                var imdb = section["imdb"] as JObject;
                if (imdb != null)
                {
                    imdbEnabledCheckBox.Checked = imdb["enabled"]?.Value<bool>() ?? false;
                    imdbMinRatingNumeric.Value = (decimal)(imdb["min_rating"]?.Value<double>() ?? 0);
                    imdbMinVotesNumeric.Value = imdb["min_votes"]?.Value<int>() ?? 0;
                    onlyEnglishCheckBox.Checked = imdb["only_english"]?.Value<bool>() ?? false;
                    onlyUSCountryCheckBox.Checked = imdb["only_us_country"]?.Value<bool>() ?? false;
                    noDocumentaryCheckBox.Checked = imdb["no_documentary"]?.Value<bool>() ?? false;
                    noMusicCheckBox.Checked = imdb["no_music"]?.Value<bool>() ?? false;
                    noComedyCheckBox.Checked = imdb["no_comedy"]?.Value<bool>() ?? false;
                    noShowCheckBox.Checked = imdb["no_show"]?.Value<bool>() ?? false;
                    imdbFallbackCheckBox.Checked = imdb["fallback_on_error"]?.Value<bool>() ?? true;

                    LoadGenres(imdb, imdbAllowedGenresListBox, imdbBlockedGenresListBox);
                }
                else
                {
                    SetDefaultIMDBSettings();
                }

                // Load TVMaze settings
                var tvmaze = section["tvmaze"] as JObject;
                if (tvmaze != null)
                {
                    tvmazeEnabledCheckBox.Checked = tvmaze["enabled"]?.Value<bool>() ?? false;
                    skipEndedShowsCheckBox.Checked = tvmaze["skip_ended_shows"]?.Value<bool>() ?? true;
                    tvmazeMinRatingNumeric.Value = (decimal)(tvmaze["min_rating"]?.Value<double>() ?? 0);
                    cacheDaysNumeric.Value = tvmaze["cache_duration_days"]?.Value<int>() ?? 7;
                    tvmazeFallbackCheckBox.Checked = tvmaze["fallback_on_error"]?.Value<bool>() ?? true;

                    LoadGenres(tvmaze, tvmazeAllowedGenresListBox, tvmazeBlockedGenresListBox);
                    LoadNetworks(tvmaze);
                    LoadShowTypes(tvmaze);
                }
                else
                {
                    SetDefaultTVMazeSettings();
                }

                LogManager.Info($"Loaded TVMaze/IMDB settings for section: {sectionName}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading section settings: {ex.Message}");
                SetDefaultSettings();
            }
        }

        private void LoadShowTypes(JObject tvmaze)
        {
            CheckAllItems(allowedShowTypesListBox, false);
            var allowedShowTypes = tvmaze["allowed_show_types"]?.ToObject<List<string>>() ?? new List<string>();
            foreach (var showType in allowedShowTypes)
            {
                int index = allowedShowTypesListBox.Items.IndexOf(showType);
                if (index >= 0)
                    allowedShowTypesListBox.SetItemChecked(index, true);
            }
        }


        private void LoadGenres(JObject config, CheckedListBox allowedList, CheckedListBox blockedList)
        {
            // Load allowed genres
            CheckAllItems(allowedList, false);
            var allowedGenres = config["allowed_genres"]?.ToObject<List<string>>() ?? new List<string>();
            foreach (var genre in allowedGenres)
            {
                int index = allowedList.Items.IndexOf(genre);
                if (index >= 0)
                    allowedList.SetItemChecked(index, true);
            }

            // Load blocked genres
            CheckAllItems(blockedList, false);
            var blockedGenres = config["blocked_genres"]?.ToObject<List<string>>() ?? new List<string>();
            foreach (var genre in blockedGenres)
            {
                int index = blockedList.Items.IndexOf(genre);
                if (index >= 0)
                    blockedList.SetItemChecked(index, true);
            }
        }

        private void LoadNetworks(JObject tvmaze)
        {
            CheckAllItems(allowedNetworksListBox, false);
            var allowedNetworks = tvmaze["allowed_networks"]?.ToObject<List<string>>() ?? new List<string>();
            foreach (var network in allowedNetworks)
            {
                int index = allowedNetworksListBox.Items.IndexOf(network);
                if (index >= 0)
                    allowedNetworksListBox.SetItemChecked(index, true);
            }
        }

        private void SetDefaultSettings()
        {
            SetDefaultIMDBSettings();
            SetDefaultTVMazeSettings();
        }

        private void SetDefaultIMDBSettings()
        {
            imdbEnabledCheckBox.Checked = false;
            imdbMinRatingNumeric.Value = 0;
            imdbMinVotesNumeric.Value = 0;
            onlyEnglishCheckBox.Checked = false;
            onlyUSCountryCheckBox.Checked = false;
            noDocumentaryCheckBox.Checked = false;
            noMusicCheckBox.Checked = false;
            noComedyCheckBox.Checked = false;
            noShowCheckBox.Checked = false;
            imdbFallbackCheckBox.Checked = true;
            CheckAllItems(imdbAllowedGenresListBox, false);
            CheckAllItems(imdbBlockedGenresListBox, false);
        }

        private void SetDefaultTVMazeSettings()
        {
            tvmazeEnabledCheckBox.Checked = false;
            skipEndedShowsCheckBox.Checked = true;
            tvmazeMinRatingNumeric.Value = 0;
            cacheDaysNumeric.Value = 7;
            tvmazeFallbackCheckBox.Checked = true;
            CheckAllItems(tvmazeAllowedGenresListBox, false);
            CheckAllItems(tvmazeBlockedGenresListBox, false);
            CheckAllItems(allowedNetworksListBox, false);
            CheckAllItems(allowedShowTypesListBox, false);
        }

        private async void TestIMDBButton_Click(object sender, EventArgs e)
        {
            testIMDBButton.Enabled = false;
            testIMDBButton.Text = "Testing...";
            try
            {
                // Race filtering uses title search, so test that live path instead
                // of proving only that a fixed IMDb ID exists in cache.
                var testMovie = await IMDBHelper.SearchMovie("Back to the Future", 1985, 0);

                if (testMovie != null)
                {
                    // Format rating to 1 decimal place
                    var ratingText = testMovie.ImdbRating.HasValue
                        ? testMovie.ImdbRating.Value.ToString("F1")
                        : "N/A";

                    var message = $"✓ IMDbAPI!\n\n" +
                                  $"Title-search test: {testMovie.Title} ({testMovie.Year})\n" +
                                  $"Rating: {ratingText}\n" +
                                  $"Votes: {testMovie.ImdbVotes:N0}\n" +
                                  $"Genres: {string.Join(", ", testMovie.Genres ?? new List<string>())}\n" +
                                  $"Language: {testMovie.Language}\n" +
                                  $"Country: {testMovie.Country}";

                    MessageBox.Show(message, "IMDbAPI Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogManager.Success("IMDbAPI test successful");
                }
                else
                {
                    MessageBox.Show("IMDb title-search failed.\n\nRace filters use title search, so cached IMDb ID tests are not enough. Check internet/DNS or imdbapi.dev availability.",
                        "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogManager.Warning("IMDbAPI title-search test failed - check internet/DNS or imdbapi.dev");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing IMDbAPI:\n{ex.Message}",
                    "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"IMDbAPI test error: {ex.Message}");
            }
            finally
            {
                testIMDBButton.Enabled = true;
                testIMDBButton.Text = "Test IMDb";
            }
        }

        private async void TestTVMazeButton_Click(object sender, EventArgs e)
        {
            testTVMazeButton.Enabled = false;
            testTVMazeButton.Text = "Testing...";

            try
            {
                var testShow = await TVMazeHelper.LookupByImdb("tt0944947");

                if (testShow != null)
                {
                    var message = $"✓ TVMaze API is working!\n\n" +
                                  $"Test Show: {testShow.Name}\n" +
                                  $"Type: {testShow.Type ?? "N/A"}\n" +
                                  $"Status: {testShow.Status}\n" +
                                  $"Genres: {string.Join(", ", testShow.Genres ?? new List<string>())}\n" +
                                  $"Rating: {testShow.Rating?.Average ?? 0}\n" +
                                  $"Network: {testShow.Network?.Name ?? testShow.WebChannel?.Name ?? "N/A"}";

                    MessageBox.Show(message, "TVMaze Test Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogManager.Success("TVMaze API test successful");
                }
                else
                {
                    MessageBox.Show("Could not connect to TVMaze API.",
                        "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogManager.Warning("TVMaze API test failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error testing TVMaze:\n{ex.Message}",
                    "Test Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogManager.Error($"TVMaze test error: {ex.Message}");
            }
            finally
            {
                testTVMazeButton.Enabled = true;
                testTVMazeButton.Text = "Test TVMaze";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (currentSiteConfig == null || sectionComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a site and section first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var sectionName = sectionComboBox.SelectedItem.ToString();
                var sections = currentSiteConfig["sections"] as JArray;
                var section = sections?.FirstOrDefault(s => s["irc_name"]?.ToString() == sectionName) as JObject;

                if (section == null)
                {
                    MessageBox.Show("Section not found.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Save IMDB config
                var imdbConfig = new JObject
                {
                    ["enabled"] = imdbEnabledCheckBox.Checked,
                    ["min_rating"] = (double)imdbMinRatingNumeric.Value,
                    ["min_votes"] = (int)imdbMinVotesNumeric.Value,
                    ["only_english"] = onlyEnglishCheckBox.Checked,
                    ["only_us_country"] = onlyUSCountryCheckBox.Checked,
                    ["no_documentary"] = noDocumentaryCheckBox.Checked,
                    ["no_music"] = noMusicCheckBox.Checked,
                    ["no_comedy"] = noComedyCheckBox.Checked,
                    ["no_show"] = noShowCheckBox.Checked,
                    ["fallback_on_error"] = imdbFallbackCheckBox.Checked,
                    ["allowed_genres"] = JArray.FromObject(GetCheckedItems(imdbAllowedGenresListBox)),
                    ["blocked_genres"] = JArray.FromObject(GetCheckedItems(imdbBlockedGenresListBox))
                };
                section["imdb"] = imdbConfig;

                // Save TVMaze config
                var tvmazeConfig = new JObject
                {
                    ["enabled"] = tvmazeEnabledCheckBox.Checked,
                    ["skip_ended_shows"] = skipEndedShowsCheckBox.Checked,
                    ["min_rating"] = (double)tvmazeMinRatingNumeric.Value,
                    ["cache_duration_days"] = (int)cacheDaysNumeric.Value,
                    ["fallback_on_error"] = tvmazeFallbackCheckBox.Checked,
                    ["allowed_genres"] = JArray.FromObject(GetCheckedItems(tvmazeAllowedGenresListBox)),
                    ["blocked_genres"] = JArray.FromObject(GetCheckedItems(tvmazeBlockedGenresListBox)),
                    ["allowed_networks"] = JArray.FromObject(GetCheckedItems(allowedNetworksListBox)),
                    ["allowed_show_types"] = JArray.FromObject(GetCheckedItems(allowedShowTypesListBox))
                };
                section["tvmaze"] = tvmazeConfig;

                // Save back to file
                RaceTrade.AtomicFile.WriteAllText(currentSiteFile, currentSiteConfig.ToString(Formatting.Indented));

                LogManager.Success($"Settings saved for section in tvmaze/imdb settings: {sectionName}");
                MessageBox.Show("Settings saved successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);


                this.Close();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error saving settings: {ex.Message}");
                MessageBox.Show($"Error saving settings:\n{ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetCheckedItems(CheckedListBox listBox)
        {
            var items = new List<string>();
            foreach (var item in listBox.CheckedItems)
            {
                items.Add(item.ToString());
            }
            return items;
        }
    }
}
