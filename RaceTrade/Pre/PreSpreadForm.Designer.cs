namespace RaceTrade
{
    partial class PreSpreadForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox groupBoxCbftpServers;
        private System.Windows.Forms.ListBox listBoxCbftpServers;
        private System.Windows.Forms.Button btnAddCbftpServer;
        private System.Windows.Forms.Button btnEditCbftpServer;
        private System.Windows.Forms.Button btnRemoveCbftpServer;
        private System.Windows.Forms.Button btnFetchAllSites;
        private System.Windows.Forms.GroupBox groupBoxSitesConfig;
        private System.Windows.Forms.CheckedListBox checkedListBoxSites;
        private System.Windows.Forms.Panel panelSiteConfig;
        private System.Windows.Forms.Label lblSiteName;
        private System.Windows.Forms.TextBox txtSiteName;
        private System.Windows.Forms.Label lblCbftpServer;
        private System.Windows.Forms.ComboBox comboCbftpServer;
        private System.Windows.Forms.Label lblAffilDirectory;
        private System.Windows.Forms.TextBox txtAffilDirectory;
        private System.Windows.Forms.Label lblSection;
        private System.Windows.Forms.TextBox txtSection;
        private System.Windows.Forms.Button btnSaveSiteConfig;
        private System.Windows.Forms.Button btnSendPre;
        private System.Windows.Forms.GroupBox groupBoxDistribution;
        private System.Windows.Forms.Label lblSourceSite;
        private System.Windows.Forms.ComboBox comboSourceSite;
        private System.Windows.Forms.Button btnRefreshReleases;
        private System.Windows.Forms.Label lblDestSites;
        private System.Windows.Forms.CheckedListBox checkedListBoxDestSites;
        private System.Windows.Forms.Label lblReleases;
        private System.Windows.Forms.ListBox listBoxReleases;
        private System.Windows.Forms.Button btnDistribute;
        private System.Windows.Forms.Button btnCheckCompletion;
        private System.Windows.Forms.Button btnDeleteRelease;
        private System.Windows.Forms.Label lblSelectedRelease;
        private System.Windows.Forms.Label lblSelectedGroup;
        private System.Windows.Forms.GroupBox groupBoxCompletionLog;
        private System.Windows.Forms.RichTextBox richTextBoxCompletionLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupBoxCbftpServers = new System.Windows.Forms.GroupBox();
            this.listBoxCbftpServers = new System.Windows.Forms.ListBox();
            this.btnAddCbftpServer = new System.Windows.Forms.Button();
            this.btnEditCbftpServer = new System.Windows.Forms.Button();
            this.btnRemoveCbftpServer = new System.Windows.Forms.Button();
            this.btnFetchAllSites = new System.Windows.Forms.Button();
            this.groupBoxSitesConfig = new System.Windows.Forms.GroupBox();
            this.checkedListBoxSites = new System.Windows.Forms.CheckedListBox();
            this.panelSiteConfig = new System.Windows.Forms.Panel();
            this.lblSiteName = new System.Windows.Forms.Label();
            this.txtSiteName = new System.Windows.Forms.TextBox();
            this.lblCbftpServer = new System.Windows.Forms.Label();
            this.comboCbftpServer = new System.Windows.Forms.ComboBox();
            this.lblAffilDirectory = new System.Windows.Forms.Label();
            this.txtAffilDirectory = new System.Windows.Forms.TextBox();
            this.lblSection = new System.Windows.Forms.Label();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.btnSaveSiteConfig = new System.Windows.Forms.Button();
            this.btnSendPre = new System.Windows.Forms.Button();
            this.groupBoxDistribution = new System.Windows.Forms.GroupBox();
            this.lblSourceSite = new System.Windows.Forms.Label();
            this.comboSourceSite = new System.Windows.Forms.ComboBox();
            this.btnRefreshReleases = new System.Windows.Forms.Button();
            this.lblDestSites = new System.Windows.Forms.Label();
            this.checkedListBoxDestSites = new System.Windows.Forms.CheckedListBox();
            this.lblReleases = new System.Windows.Forms.Label();
            this.listBoxReleases = new System.Windows.Forms.ListBox();
            this.btnDeleteRelease = new System.Windows.Forms.Button();
            this.btnDistribute = new System.Windows.Forms.Button();
            this.btnCheckCompletion = new System.Windows.Forms.Button();
            this.lblSelectedRelease = new System.Windows.Forms.Label();
            this.lblSelectedGroup = new System.Windows.Forms.Label();
            this.groupBoxCompletionLog = new System.Windows.Forms.GroupBox();
            this.richTextBoxCompletionLog = new System.Windows.Forms.RichTextBox();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBoxCbftpServers.SuspendLayout();
            this.groupBoxSitesConfig.SuspendLayout();
            this.panelSiteConfig.SuspendLayout();
            this.groupBoxDistribution.SuspendLayout();
            this.groupBoxCompletionLog.SuspendLayout();
            this.SuspendLayout();

            // groupBoxCbftpServers
            this.groupBoxCbftpServers.Controls.Add(this.listBoxCbftpServers);
            this.groupBoxCbftpServers.Controls.Add(this.btnAddCbftpServer);
            this.groupBoxCbftpServers.Controls.Add(this.btnEditCbftpServer);
            this.groupBoxCbftpServers.Controls.Add(this.btnRemoveCbftpServer);
            this.groupBoxCbftpServers.Controls.Add(this.btnFetchAllSites);
            this.groupBoxCbftpServers.ForeColor = System.Drawing.Color.White;
            this.groupBoxCbftpServers.Location = new System.Drawing.Point(16, 15);
            this.groupBoxCbftpServers.Name = "groupBoxCbftpServers";
            this.groupBoxCbftpServers.Size = new System.Drawing.Size(1196, 82);
            this.groupBoxCbftpServers.TabIndex = 0;
            this.groupBoxCbftpServers.TabStop = false;
            this.groupBoxCbftpServers.Text = "CBFTP Servers";

            // listBoxCbftpServers
            this.listBoxCbftpServers.BackColor = System.Drawing.Color.FromArgb(13, 16, 24);
            this.listBoxCbftpServers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxCbftpServers.ForeColor = System.Drawing.Color.White;
            this.listBoxCbftpServers.FormattingEnabled = true;
            this.listBoxCbftpServers.ItemHeight = 16;
            this.listBoxCbftpServers.Location = new System.Drawing.Point(13, 31);
            this.listBoxCbftpServers.Name = "listBoxCbftpServers";
            this.listBoxCbftpServers.Size = new System.Drawing.Size(666, 34);
            this.listBoxCbftpServers.TabIndex = 0;

            // btnAddCbftpServer
            this.btnAddCbftpServer.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnAddCbftpServer.FlatAppearance.BorderSize = 0;
            this.btnAddCbftpServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddCbftpServer.ForeColor = System.Drawing.Color.White;
            this.btnAddCbftpServer.Location = new System.Drawing.Point(693, 31);
            this.btnAddCbftpServer.Name = "btnAddCbftpServer";
            this.btnAddCbftpServer.Size = new System.Drawing.Size(107, 31);
            this.btnAddCbftpServer.TabIndex = 1;
            this.btnAddCbftpServer.Text = "Add";
            this.btnAddCbftpServer.UseVisualStyleBackColor = false;
            this.btnAddCbftpServer.Click += new System.EventHandler(this.BtnAddCbftpServer_Click);

            // btnEditCbftpServer
            this.btnEditCbftpServer.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.btnEditCbftpServer.FlatAppearance.BorderSize = 0;
            this.btnEditCbftpServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditCbftpServer.ForeColor = System.Drawing.Color.White;
            this.btnEditCbftpServer.Location = new System.Drawing.Point(808, 31);
            this.btnEditCbftpServer.Name = "btnEditCbftpServer";
            this.btnEditCbftpServer.Size = new System.Drawing.Size(107, 31);
            this.btnEditCbftpServer.TabIndex = 2;
            this.btnEditCbftpServer.Text = "Edit";
            this.btnEditCbftpServer.UseVisualStyleBackColor = false;
            this.btnEditCbftpServer.Click += new System.EventHandler(this.BtnEditCbftpServer_Click);

            // btnRemoveCbftpServer
            this.btnRemoveCbftpServer.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnRemoveCbftpServer.FlatAppearance.BorderSize = 0;
            this.btnRemoveCbftpServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveCbftpServer.ForeColor = System.Drawing.Color.White;
            this.btnRemoveCbftpServer.Location = new System.Drawing.Point(923, 31);
            this.btnRemoveCbftpServer.Name = "btnRemoveCbftpServer";
            this.btnRemoveCbftpServer.Size = new System.Drawing.Size(107, 31);
            this.btnRemoveCbftpServer.TabIndex = 3;
            this.btnRemoveCbftpServer.Text = "Remove";
            this.btnRemoveCbftpServer.UseVisualStyleBackColor = false;
            this.btnRemoveCbftpServer.Click += new System.EventHandler(this.BtnRemoveCbftpServer_Click);

            // btnFetchAllSites
            this.btnFetchAllSites.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnFetchAllSites.FlatAppearance.BorderSize = 0;
            this.btnFetchAllSites.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFetchAllSites.ForeColor = System.Drawing.Color.White;
            this.btnFetchAllSites.Location = new System.Drawing.Point(1038, 31);
            this.btnFetchAllSites.Name = "btnFetchAllSites";
            this.btnFetchAllSites.Size = new System.Drawing.Size(135, 31);
            this.btnFetchAllSites.TabIndex = 4;
            this.btnFetchAllSites.Text = "Fetch All Sites";
            this.btnFetchAllSites.UseVisualStyleBackColor = false;
            this.btnFetchAllSites.Click += new System.EventHandler(this.BtnFetchAllSites_Click);

            // groupBoxSitesConfig
            this.groupBoxSitesConfig.Controls.Add(this.checkedListBoxSites);
            this.groupBoxSitesConfig.Controls.Add(this.panelSiteConfig);
            this.groupBoxSitesConfig.Controls.Add(this.btnSendPre);
            this.groupBoxSitesConfig.ForeColor = System.Drawing.Color.White;
            this.groupBoxSitesConfig.Location = new System.Drawing.Point(16, 105);
            this.groupBoxSitesConfig.Name = "groupBoxSitesConfig";
            this.groupBoxSitesConfig.Size = new System.Drawing.Size(600, 591);
            this.groupBoxSitesConfig.TabIndex = 1;
            this.groupBoxSitesConfig.TabStop = false;
            this.groupBoxSitesConfig.Text = "Sites Configuration";

            // checkedListBoxSites
            this.checkedListBoxSites.BackColor = System.Drawing.Color.FromArgb(13, 16, 24);
            this.checkedListBoxSites.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxSites.ForeColor = System.Drawing.Color.White;
            this.checkedListBoxSites.FormattingEnabled = true;
            this.checkedListBoxSites.Location = new System.Drawing.Point(13, 31);
            this.checkedListBoxSites.Name = "checkedListBoxSites";
            this.checkedListBoxSites.Size = new System.Drawing.Size(573, 172);
            this.checkedListBoxSites.TabIndex = 0;
            this.checkedListBoxSites.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.CheckedListBoxSites_ItemCheck);
            this.checkedListBoxSites.SelectedIndexChanged += new System.EventHandler(this.CheckedListBoxSites_SelectedIndexChanged);

            // panelSiteConfig
            this.panelSiteConfig.BackColor = System.Drawing.Color.FromArgb(17, 21, 29);
            this.panelSiteConfig.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSiteConfig.Controls.Add(this.lblSiteName);
            this.panelSiteConfig.Controls.Add(this.txtSiteName);
            this.panelSiteConfig.Controls.Add(this.lblCbftpServer);
            this.panelSiteConfig.Controls.Add(this.comboCbftpServer);
            this.panelSiteConfig.Controls.Add(this.lblAffilDirectory);
            this.panelSiteConfig.Controls.Add(this.txtAffilDirectory);
            this.panelSiteConfig.Controls.Add(this.lblSection);
            this.panelSiteConfig.Controls.Add(this.txtSection);
            this.panelSiteConfig.Controls.Add(this.btnSaveSiteConfig);
            this.panelSiteConfig.Enabled = false;
            this.panelSiteConfig.Location = new System.Drawing.Point(13, 209);
            this.panelSiteConfig.Name = "panelSiteConfig";
            this.panelSiteConfig.Size = new System.Drawing.Size(573, 319);
            this.panelSiteConfig.TabIndex = 1;

            // lblSiteName
            this.lblSiteName.AutoSize = true;
            this.lblSiteName.ForeColor = System.Drawing.Color.White;
            this.lblSiteName.Location = new System.Drawing.Point(13, 18);
            this.lblSiteName.Name = "lblSiteName";
            this.lblSiteName.Size = new System.Drawing.Size(73, 16);
            this.lblSiteName.TabIndex = 0;
            this.lblSiteName.Text = "Site Name:";

            // txtSiteName
            this.txtSiteName.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.txtSiteName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSiteName.ForeColor = System.Drawing.Color.White;
            this.txtSiteName.Location = new System.Drawing.Point(160, 15);
            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.ReadOnly = true;
            this.txtSiteName.Size = new System.Drawing.Size(386, 22);
            this.txtSiteName.TabIndex = 1;

            // lblCbftpServer
            this.lblCbftpServer.AutoSize = true;
            this.lblCbftpServer.ForeColor = System.Drawing.Color.White;
            this.lblCbftpServer.Location = new System.Drawing.Point(13, 55);
            this.lblCbftpServer.Name = "lblCbftpServer";
            this.lblCbftpServer.Size = new System.Drawing.Size(97, 16);
            this.lblCbftpServer.TabIndex = 2;
            this.lblCbftpServer.Text = "CBFTP Server:";

            // comboCbftpServer
            this.comboCbftpServer.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.comboCbftpServer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCbftpServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboCbftpServer.ForeColor = System.Drawing.Color.White;
            this.comboCbftpServer.FormattingEnabled = true;
            this.comboCbftpServer.Location = new System.Drawing.Point(160, 52);
            this.comboCbftpServer.Name = "comboCbftpServer";
            this.comboCbftpServer.Size = new System.Drawing.Size(385, 24);
            this.comboCbftpServer.TabIndex = 3;

            // lblAffilDirectory
            this.lblAffilDirectory.AutoSize = true;
            this.lblAffilDirectory.ForeColor = System.Drawing.Color.White;
            this.lblAffilDirectory.Location = new System.Drawing.Point(13, 92);
            this.lblAffilDirectory.Name = "lblAffilDirectory";
            this.lblAffilDirectory.Size = new System.Drawing.Size(88, 16);
            this.lblAffilDirectory.TabIndex = 4;
            this.lblAffilDirectory.Text = "Affil Directory:";

            // txtAffilDirectory
            this.txtAffilDirectory.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.txtAffilDirectory.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAffilDirectory.ForeColor = System.Drawing.Color.White;
            this.txtAffilDirectory.Location = new System.Drawing.Point(160, 89);
            this.txtAffilDirectory.Name = "txtAffilDirectory";
            this.txtAffilDirectory.Size = new System.Drawing.Size(386, 22);
            this.txtAffilDirectory.TabIndex = 5;

            // lblSection
            this.lblSection.AutoSize = true;
            this.lblSection.ForeColor = System.Drawing.Color.White;
            this.lblSection.Location = new System.Drawing.Point(13, 129);
            this.lblSection.Name = "lblSection";
            this.lblSection.Size = new System.Drawing.Size(55, 16);
            this.lblSection.TabIndex = 6;
            this.lblSection.Text = "Section:";

            // txtSection
            this.txtSection.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.txtSection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSection.ForeColor = System.Drawing.Color.White;
            this.txtSection.Location = new System.Drawing.Point(160, 126);
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(386, 22);
            this.txtSection.TabIndex = 7;

            // btnSaveSiteConfig
            this.btnSaveSiteConfig.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnSaveSiteConfig.FlatAppearance.BorderSize = 0;
            this.btnSaveSiteConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveSiteConfig.ForeColor = System.Drawing.Color.White;
            this.btnSaveSiteConfig.Location = new System.Drawing.Point(434, 263);
            this.btnSaveSiteConfig.Name = "btnSaveSiteConfig";
            this.btnSaveSiteConfig.Size = new System.Drawing.Size(133, 37);
            this.btnSaveSiteConfig.TabIndex = 8;
            this.btnSaveSiteConfig.Text = "Save Config";
            this.btnSaveSiteConfig.UseVisualStyleBackColor = false;
            this.btnSaveSiteConfig.Click += new System.EventHandler(this.BtnSaveSiteConfig_Click);

            // btnSendPre
            this.btnSendPre.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnSendPre.Enabled = false;
            this.btnSendPre.FlatAppearance.BorderSize = 0;
            this.btnSendPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendPre.ForeColor = System.Drawing.Color.White;
            this.btnSendPre.Location = new System.Drawing.Point(13, 532);
            this.btnSendPre.Name = "btnSendPre";
            this.btnSendPre.Size = new System.Drawing.Size(573, 38);
            this.btnSendPre.TabIndex = 2;
            this.btnSendPre.Text = "SITE PRE (Checked Sites)";
            this.btnSendPre.UseVisualStyleBackColor = false;
            this.btnSendPre.Click += new System.EventHandler(this.BtnSendPre_Click);

            // groupBoxDistribution
            this.groupBoxDistribution.Controls.Add(this.lblSourceSite);
            this.groupBoxDistribution.Controls.Add(this.comboSourceSite);
            this.groupBoxDistribution.Controls.Add(this.btnRefreshReleases);
            this.groupBoxDistribution.Controls.Add(this.lblDestSites);
            this.groupBoxDistribution.Controls.Add(this.checkedListBoxDestSites);
            this.groupBoxDistribution.Controls.Add(this.lblReleases);
            this.groupBoxDistribution.Controls.Add(this.listBoxReleases);
            this.groupBoxDistribution.Controls.Add(this.btnDeleteRelease);
            this.groupBoxDistribution.Controls.Add(this.btnDistribute);
            this.groupBoxDistribution.Controls.Add(this.btnCheckCompletion);
            this.groupBoxDistribution.ForeColor = System.Drawing.Color.White;
            this.groupBoxDistribution.Location = new System.Drawing.Point(627, 105);
            this.groupBoxDistribution.Name = "groupBoxDistribution";
            this.groupBoxDistribution.Size = new System.Drawing.Size(586, 591);
            this.groupBoxDistribution.TabIndex = 2;
            this.groupBoxDistribution.TabStop = false;
            this.groupBoxDistribution.Text = "Distribution";

            // lblSourceSite
            this.lblSourceSite.AutoSize = true;
            this.lblSourceSite.ForeColor = System.Drawing.Color.White;
            this.lblSourceSite.Location = new System.Drawing.Point(13, 31);
            this.lblSourceSite.Name = "lblSourceSite";
            this.lblSourceSite.Size = new System.Drawing.Size(79, 16);
            this.lblSourceSite.TabIndex = 0;
            this.lblSourceSite.Text = "Source Site:";

            // comboSourceSite
            this.comboSourceSite.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.comboSourceSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSourceSite.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboSourceSite.ForeColor = System.Drawing.Color.White;
            this.comboSourceSite.FormattingEnabled = true;
            this.comboSourceSite.Location = new System.Drawing.Point(120, 27);
            this.comboSourceSite.Name = "comboSourceSite";
            this.comboSourceSite.Size = new System.Drawing.Size(332, 24);
            this.comboSourceSite.TabIndex = 1;

            // btnRefreshReleases
            this.btnRefreshReleases.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnRefreshReleases.FlatAppearance.BorderSize = 0;
            this.btnRefreshReleases.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshReleases.ForeColor = System.Drawing.Color.White;
            this.btnRefreshReleases.Location = new System.Drawing.Point(467, 25);
            this.btnRefreshReleases.Name = "btnRefreshReleases";
            this.btnRefreshReleases.Size = new System.Drawing.Size(107, 31);
            this.btnRefreshReleases.TabIndex = 2;
            this.btnRefreshReleases.Text = "Refresh";
            this.btnRefreshReleases.UseVisualStyleBackColor = false;
            this.btnRefreshReleases.Click += new System.EventHandler(this.BtnRefreshReleases_Click);

            // lblDestSites
            this.lblDestSites.AutoSize = true;
            this.lblDestSites.ForeColor = System.Drawing.Color.White;
            this.lblDestSites.Location = new System.Drawing.Point(13, 68);
            this.lblDestSites.Name = "lblDestSites";
            this.lblDestSites.Size = new System.Drawing.Size(110, 16);
            this.lblDestSites.TabIndex = 3;
            this.lblDestSites.Text = "Destination Sites:";

            // checkedListBoxDestSites
            this.checkedListBoxDestSites.BackColor = System.Drawing.Color.FromArgb(13, 16, 24);
            this.checkedListBoxDestSites.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxDestSites.ForeColor = System.Drawing.Color.White;
            this.checkedListBoxDestSites.FormattingEnabled = true;
            this.checkedListBoxDestSites.Location = new System.Drawing.Point(13, 92);
            this.checkedListBoxDestSites.Name = "checkedListBoxDestSites";
            this.checkedListBoxDestSites.Size = new System.Drawing.Size(559, 104);
            this.checkedListBoxDestSites.TabIndex = 4;

            // lblReleases
            this.lblReleases.AutoSize = true;
            this.lblReleases.ForeColor = System.Drawing.Color.White;
            this.lblReleases.Location = new System.Drawing.Point(13, 209);
            this.lblReleases.Name = "lblReleases";
            this.lblReleases.Size = new System.Drawing.Size(69, 16);
            this.lblReleases.TabIndex = 5;
            this.lblReleases.Text = "Releases:";

            // listBoxReleases
            this.listBoxReleases.BackColor = System.Drawing.Color.FromArgb(13, 16, 24);
            this.listBoxReleases.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxReleases.ForeColor = System.Drawing.Color.White;
            this.listBoxReleases.FormattingEnabled = true;
            this.listBoxReleases.ItemHeight = 16;
            this.listBoxReleases.Location = new System.Drawing.Point(13, 238);
            this.listBoxReleases.Name = "listBoxReleases";
            this.listBoxReleases.Size = new System.Drawing.Size(559, 274);
            this.listBoxReleases.TabIndex = 6;
            this.listBoxReleases.SelectedIndexChanged += new System.EventHandler(this.ListBoxReleases_SelectedIndexChanged);

            // btnDeleteRelease
            this.btnDeleteRelease.BackColor = System.Drawing.Color.FromArgb(192, 57, 43);
            this.btnDeleteRelease.Enabled = false;
            this.btnDeleteRelease.FlatAppearance.BorderSize = 0;
            this.btnDeleteRelease.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteRelease.ForeColor = System.Drawing.Color.White;
            this.btnDeleteRelease.Location = new System.Drawing.Point(13, 532);
            this.btnDeleteRelease.Name = "btnDeleteRelease";
            this.btnDeleteRelease.Size = new System.Drawing.Size(133, 38);
            this.btnDeleteRelease.TabIndex = 9;
            this.btnDeleteRelease.Text = "Delete Release";
            this.btnDeleteRelease.UseVisualStyleBackColor = false;
            this.btnDeleteRelease.Click += new System.EventHandler(this.BtnDeleteRelease_Click);

            // btnDistribute
            this.btnDistribute.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnDistribute.Enabled = false;
            this.btnDistribute.FlatAppearance.BorderSize = 0;
            this.btnDistribute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDistribute.ForeColor = System.Drawing.Color.White;
            this.btnDistribute.Location = new System.Drawing.Point(160, 532);
            this.btnDistribute.Name = "btnDistribute";
            this.btnDistribute.Size = new System.Drawing.Size(196, 38);
            this.btnDistribute.TabIndex = 7;
            this.btnDistribute.Text = "Distribute Release";
            this.btnDistribute.UseVisualStyleBackColor = false;
            this.btnDistribute.Click += new System.EventHandler(this.BtnDistribute_Click);

            // btnCheckCompletion
            this.btnCheckCompletion.BackColor = System.Drawing.Color.FromArgb(243, 156, 18);
            this.btnCheckCompletion.Enabled = false;
            this.btnCheckCompletion.FlatAppearance.BorderSize = 0;
            this.btnCheckCompletion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCheckCompletion.ForeColor = System.Drawing.Color.White;
            this.btnCheckCompletion.Location = new System.Drawing.Point(370, 532);
            this.btnCheckCompletion.Name = "btnCheckCompletion";
            this.btnCheckCompletion.Size = new System.Drawing.Size(202, 38);
            this.btnCheckCompletion.TabIndex = 8;
            this.btnCheckCompletion.Text = "Check Completion";
            this.btnCheckCompletion.UseVisualStyleBackColor = false;
            this.btnCheckCompletion.Click += new System.EventHandler(this.BtnCheckCompletion_Click);

            // lblSelectedRelease
            this.lblSelectedRelease.AutoSize = true;
            this.lblSelectedRelease.ForeColor = System.Drawing.Color.White;
            this.lblSelectedRelease.Location = new System.Drawing.Point(16, 708);
            this.lblSelectedRelease.Name = "lblSelectedRelease";
            this.lblSelectedRelease.Size = new System.Drawing.Size(100, 16);
            this.lblSelectedRelease.TabIndex = 3;
            this.lblSelectedRelease.Text = "Selected: None";

            // lblSelectedGroup
            this.lblSelectedGroup.AutoSize = true;
            this.lblSelectedGroup.ForeColor = System.Drawing.Color.White;
            this.lblSelectedGroup.Location = new System.Drawing.Point(16, 732);
            this.lblSelectedGroup.Name = "lblSelectedGroup";
            this.lblSelectedGroup.Size = new System.Drawing.Size(83, 16);
            this.lblSelectedGroup.TabIndex = 4;
            this.lblSelectedGroup.Text = "Group: None";

            // groupBoxCompletionLog
            this.groupBoxCompletionLog.Controls.Add(this.richTextBoxCompletionLog);
            this.groupBoxCompletionLog.Controls.Add(this.btnClearLog);
            this.groupBoxCompletionLog.ForeColor = System.Drawing.Color.White;
            this.groupBoxCompletionLog.Location = new System.Drawing.Point(16, 757);
            this.groupBoxCompletionLog.Name = "groupBoxCompletionLog";
            this.groupBoxCompletionLog.Size = new System.Drawing.Size(1197, 184);
            this.groupBoxCompletionLog.TabIndex = 5;
            this.groupBoxCompletionLog.TabStop = false;
            this.groupBoxCompletionLog.Text = "Completion Check Log";

            // richTextBoxCompletionLog
            this.richTextBoxCompletionLog.BackColor = System.Drawing.Color.FromArgb(13, 16, 24);
            this.richTextBoxCompletionLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxCompletionLog.ForeColor = System.Drawing.Color.White;
            this.richTextBoxCompletionLog.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.richTextBoxCompletionLog.Location = new System.Drawing.Point(13, 25);
            this.richTextBoxCompletionLog.Name = "richTextBoxCompletionLog";
            this.richTextBoxCompletionLog.ReadOnly = true;
            this.richTextBoxCompletionLog.Size = new System.Drawing.Size(1048, 146);
            this.richTextBoxCompletionLog.TabIndex = 0;
            this.richTextBoxCompletionLog.Text = "";

            // btnClearLog
            this.btnClearLog.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.btnClearLog.FlatAppearance.BorderSize = 0;
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.ForeColor = System.Drawing.Color.White;
            this.btnClearLog.Location = new System.Drawing.Point(1087, 25);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(98, 36);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = false;
            this.btnClearLog.Click += new System.EventHandler(this.BtnClearLog_Click);

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(16, 949);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1197, 31);
            this.progressBar.TabIndex = 6;
            this.progressBar.Visible = false;

            // lblStatus
            this.lblStatus.ForeColor = System.Drawing.Color.LightGray;
            this.lblStatus.Location = new System.Drawing.Point(16, 984);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(1200, 49);
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Ready";

            // btnSaveConfig
            this.btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(0, 168, 255);
            this.btnSaveConfig.FlatAppearance.BorderSize = 0;
            this.btnSaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveConfig.ForeColor = System.Drawing.Color.White;
            this.btnSaveConfig.Location = new System.Drawing.Point(933, 1022);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(133, 37);
            this.btnSaveConfig.TabIndex = 8;
            this.btnSaveConfig.Text = "Save Config";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.Click += new System.EventHandler(this.BtnSaveConfig_Click);

            // btnClose
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(37, 42, 54);
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1083, 1022);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(133, 37);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);

            // PreSpreadForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(22, 26, 36);
            this.ClientSize = new System.Drawing.Size(1229, 1070);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBoxCompletionLog);
            this.Controls.Add(this.lblSelectedGroup);
            this.Controls.Add(this.lblSelectedRelease);
            this.Controls.Add(this.groupBoxDistribution);
            this.Controls.Add(this.groupBoxSitesConfig);
            this.Controls.Add(this.groupBoxCbftpServers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PreSpreadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Affil Spread & Pre Manager";
            this.groupBoxCbftpServers.ResumeLayout(false);
            this.groupBoxSitesConfig.ResumeLayout(false);
            this.panelSiteConfig.ResumeLayout(false);
            this.panelSiteConfig.PerformLayout();
            this.groupBoxDistribution.ResumeLayout(false);
            this.groupBoxDistribution.PerformLayout();
            this.groupBoxCompletionLog.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
