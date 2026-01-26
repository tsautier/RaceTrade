using System.Drawing;

namespace RaceTrade
{
    partial class MainApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainApp));
            this.groupBox1_cbftp = new System.Windows.Forms.GroupBox();
            this.Ftp_button = new System.Windows.Forms.Button();
            this.Cbftp_Edit_Site = new System.Windows.Forms.Button();
            this.Add_cbftp_site = new System.Windows.Forms.Button();
            this.Add_Ccbftp_Sections = new System.Windows.Forms.Button();
            this.EditDropdown_cbftp_comboBox = new System.Windows.Forms.ComboBox();
            this.Add_Cbftp_button = new System.Windows.Forms.Button();
            this.add_sites_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Edit_sites_button = new System.Windows.Forms.Button();
            this.Enable_Disable_Racer_button = new System.Windows.Forms.Button();
            this.ToggleIRCLog = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.Add_PreBot_button = new System.Windows.Forms.Button();
            this.ToggleCBFTPLog = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ToggleRaceLog = new System.Windows.Forms.Button();
            this.ToggleApplicationLog = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonImportPredb = new System.Windows.Forms.Button();
            this.Prebot_edit_button = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.blacklist_add = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Help_button = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.Sync_From_Cbftp_Button = new System.Windows.Forms.Button();
            this.Functions_groupbox = new System.Windows.Forms.GroupBox();
            this.Pre_button = new System.Windows.Forms.Button();
            this.OpenTabbedIRC = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDockLogs = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1_cbftp.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.Functions_groupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1_cbftp
            // 
            this.groupBox1_cbftp.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1_cbftp.Controls.Add(this.Ftp_button);
            this.groupBox1_cbftp.Controls.Add(this.Cbftp_Edit_Site);
            this.groupBox1_cbftp.Controls.Add(this.Add_cbftp_site);
            this.groupBox1_cbftp.Controls.Add(this.Add_Ccbftp_Sections);
            this.groupBox1_cbftp.Controls.Add(this.EditDropdown_cbftp_comboBox);
            this.groupBox1_cbftp.Controls.Add(this.Add_Cbftp_button);
            this.groupBox1_cbftp.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1_cbftp.Location = new System.Drawing.Point(10, 30);
            this.groupBox1_cbftp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1_cbftp.Name = "groupBox1_cbftp";
            this.groupBox1_cbftp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1_cbftp.Size = new System.Drawing.Size(672, 51);
            this.groupBox1_cbftp.TabIndex = 0;
            this.groupBox1_cbftp.TabStop = false;
            this.groupBox1_cbftp.Text = "Cbftp Server";
            // 
            // Ftp_button
            // 
            this.Ftp_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Ftp_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Ftp_button.Location = new System.Drawing.Point(588, 19);
            this.Ftp_button.Name = "Ftp_button";
            this.Ftp_button.Size = new System.Drawing.Size(75, 25);
            this.Ftp_button.TabIndex = 10;
            this.Ftp_button.Text = "Ftp";
            this.Ftp_button.UseVisualStyleBackColor = false;
            this.Ftp_button.Click += new System.EventHandler(this.Ftp_button_Click);
            // 
            // Cbftp_Edit_Site
            // 
            this.Cbftp_Edit_Site.BackColor = System.Drawing.Color.Coral;
            this.Cbftp_Edit_Site.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cbftp_Edit_Site.Location = new System.Drawing.Point(441, 19);
            this.Cbftp_Edit_Site.Name = "Cbftp_Edit_Site";
            this.Cbftp_Edit_Site.Size = new System.Drawing.Size(141, 25);
            this.Cbftp_Edit_Site.TabIndex = 9;
            this.Cbftp_Edit_Site.Text = "Edit CBFTP Site";
            this.Cbftp_Edit_Site.UseVisualStyleBackColor = false;
            this.Cbftp_Edit_Site.Click += new System.EventHandler(this.Cbftp_Edit_Site_Click);
            // 
            // Add_cbftp_site
            // 
            this.Add_cbftp_site.BackColor = System.Drawing.Color.Chocolate;
            this.Add_cbftp_site.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_cbftp_site.Location = new System.Drawing.Point(305, 19);
            this.Add_cbftp_site.Name = "Add_cbftp_site";
            this.Add_cbftp_site.Size = new System.Drawing.Size(130, 25);
            this.Add_cbftp_site.TabIndex = 8;
            this.Add_cbftp_site.Text = "Add CBFTP Site";
            this.Add_cbftp_site.UseVisualStyleBackColor = false;
            this.Add_cbftp_site.Click += new System.EventHandler(this.Add_cbftp_site_Click);
            // 
            // Add_Ccbftp_Sections
            // 
            this.Add_Ccbftp_Sections.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Add_Ccbftp_Sections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_Ccbftp_Sections.Location = new System.Drawing.Point(215, 19);
            this.Add_Ccbftp_Sections.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Add_Ccbftp_Sections.Name = "Add_Ccbftp_Sections";
            this.Add_Ccbftp_Sections.Size = new System.Drawing.Size(84, 25);
            this.Add_Ccbftp_Sections.TabIndex = 7;
            this.Add_Ccbftp_Sections.Text = "Sections";
            this.Add_Ccbftp_Sections.UseVisualStyleBackColor = false;
            this.Add_Ccbftp_Sections.Click += new System.EventHandler(this.button2_Click);
            // 
            // EditDropdown_cbftp_comboBox
            // 
            this.EditDropdown_cbftp_comboBox.FormattingEnabled = true;
            this.EditDropdown_cbftp_comboBox.Location = new System.Drawing.Point(6, 19);
            this.EditDropdown_cbftp_comboBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.EditDropdown_cbftp_comboBox.Name = "EditDropdown_cbftp_comboBox";
            this.EditDropdown_cbftp_comboBox.Size = new System.Drawing.Size(99, 25);
            this.EditDropdown_cbftp_comboBox.TabIndex = 1;
            this.EditDropdown_cbftp_comboBox.SelectedIndexChanged += new System.EventHandler(this.EditDropdown_cbftp_comboBox_SelectedIndexChanged);
            // 
            // Add_Cbftp_button
            // 
            this.Add_Cbftp_button.BackColor = System.Drawing.Color.LightSlateGray;
            this.Add_Cbftp_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_Cbftp_button.Location = new System.Drawing.Point(111, 19);
            this.Add_Cbftp_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Add_Cbftp_button.Name = "Add_Cbftp_button";
            this.Add_Cbftp_button.Size = new System.Drawing.Size(98, 25);
            this.Add_Cbftp_button.TabIndex = 0;
            this.Add_Cbftp_button.Text = "Add Server";
            this.Add_Cbftp_button.UseVisualStyleBackColor = false;
            this.Add_Cbftp_button.Click += new System.EventHandler(this.Add_Cbftp_button1_Click);
            // 
            // add_sites_button
            // 
            this.add_sites_button.BackColor = System.Drawing.Color.SteelBlue;
            this.add_sites_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.add_sites_button.Location = new System.Drawing.Point(6, 19);
            this.add_sites_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.add_sites_button.Name = "add_sites_button";
            this.add_sites_button.Size = new System.Drawing.Size(68, 25);
            this.add_sites_button.TabIndex = 0;
            this.add_sites_button.Text = "Add";
            this.add_sites_button.UseVisualStyleBackColor = false;
            this.add_sites_button.Click += new System.EventHandler(this.add_sites_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.Edit_sites_button);
            this.groupBox2.Controls.Add(this.add_sites_button);
            this.groupBox2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox2.Location = new System.Drawing.Point(688, 30);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(152, 51);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sites";
            // 
            // Edit_sites_button
            // 
            this.Edit_sites_button.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Edit_sites_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Edit_sites_button.Location = new System.Drawing.Point(80, 19);
            this.Edit_sites_button.Name = "Edit_sites_button";
            this.Edit_sites_button.Size = new System.Drawing.Size(66, 25);
            this.Edit_sites_button.TabIndex = 2;
            this.Edit_sites_button.Text = "Edit";
            this.Edit_sites_button.UseVisualStyleBackColor = false;
            this.Edit_sites_button.Click += new System.EventHandler(this.Edit_sites_button_Click_1);
            // 
            // Enable_Disable_Racer_button
            // 
            this.Enable_Disable_Racer_button.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.Enable_Disable_Racer_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Enable_Disable_Racer_button.Location = new System.Drawing.Point(6, 21);
            this.Enable_Disable_Racer_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Enable_Disable_Racer_button.Name = "Enable_Disable_Racer_button";
            this.Enable_Disable_Racer_button.Size = new System.Drawing.Size(74, 27);
            this.Enable_Disable_Racer_button.TabIndex = 2;
            this.Enable_Disable_Racer_button.Text = "Start Trader";
            this.Enable_Disable_Racer_button.UseVisualStyleBackColor = true;
            this.Enable_Disable_Racer_button.Click += new System.EventHandler(this.button3_Click);
            // 
            // ToggleIRCLog
            // 
            this.ToggleIRCLog.BackColor = System.Drawing.Color.LightGray;
            this.ToggleIRCLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ToggleIRCLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToggleIRCLog.Location = new System.Drawing.Point(70, 21);
            this.ToggleIRCLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ToggleIRCLog.Name = "ToggleIRCLog";
            this.ToggleIRCLog.Size = new System.Drawing.Size(72, 27);
            this.ToggleIRCLog.TabIndex = 3;
            this.ToggleIRCLog.Text = "IRC Log";
            this.ToggleIRCLog.UseVisualStyleBackColor = false;
            this.ToggleIRCLog.Click += new System.EventHandler(this.ToggleIRCLog_Click);
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.exitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exitButton.Location = new System.Drawing.Point(396, 21);
            this.exitButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(66, 27);
            this.exitButton.TabIndex = 7;
            this.exitButton.Text = "Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click_1);
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.Enable_Disable_Racer_button);
            this.groupBox4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox4.Location = new System.Drawing.Point(10, 84);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Size = new System.Drawing.Size(88, 58);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Trade";
            // 
            // Add_PreBot_button
            // 
            this.Add_PreBot_button.BackColor = System.Drawing.Color.SteelBlue;
            this.Add_PreBot_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_PreBot_button.Location = new System.Drawing.Point(10, 19);
            this.Add_PreBot_button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Add_PreBot_button.Name = "Add_PreBot_button";
            this.Add_PreBot_button.Size = new System.Drawing.Size(60, 25);
            this.Add_PreBot_button.TabIndex = 8;
            this.Add_PreBot_button.Text = "Add";
            this.Add_PreBot_button.UseVisualStyleBackColor = false;
            this.Add_PreBot_button.Click += new System.EventHandler(this.Add_PreBot_button_Click);
            // 
            // ToggleCBFTPLog
            // 
            this.ToggleCBFTPLog.BackColor = System.Drawing.Color.LightGray;
            this.ToggleCBFTPLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ToggleCBFTPLog.Location = new System.Drawing.Point(6, 21);
            this.ToggleCBFTPLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ToggleCBFTPLog.Name = "ToggleCBFTPLog";
            this.ToggleCBFTPLog.Size = new System.Drawing.Size(58, 27);
            this.ToggleCBFTPLog.TabIndex = 8;
            this.ToggleCBFTPLog.Text = "Cbftp Log";
            this.ToggleCBFTPLog.UseVisualStyleBackColor = false;
            this.ToggleCBFTPLog.Click += new System.EventHandler(this.ToggleCBFTPLog_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.ToggleRaceLog);
            this.groupBox1.Controls.Add(this.ToggleApplicationLog);
            this.groupBox1.Controls.Add(this.ToggleCBFTPLog);
            this.groupBox1.Controls.Add(this.ToggleIRCLog);
            this.groupBox1.Location = new System.Drawing.Point(104, 84);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(295, 58);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logs";
            // 
            // ToggleRaceLog
            // 
            this.ToggleRaceLog.BackColor = System.Drawing.Color.LightGray;
            this.ToggleRaceLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ToggleRaceLog.Location = new System.Drawing.Point(219, 21);
            this.ToggleRaceLog.Name = "ToggleRaceLog";
            this.ToggleRaceLog.Size = new System.Drawing.Size(70, 27);
            this.ToggleRaceLog.TabIndex = 10;
            this.ToggleRaceLog.Text = "Race";
            this.ToggleRaceLog.UseVisualStyleBackColor = false;
            this.ToggleRaceLog.Click += new System.EventHandler(this.ToggleRaceLog_Click);
            // 
            // ToggleApplicationLog
            // 
            this.ToggleApplicationLog.BackColor = System.Drawing.Color.LightGray;
            this.ToggleApplicationLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ToggleApplicationLog.Location = new System.Drawing.Point(148, 21);
            this.ToggleApplicationLog.Name = "ToggleApplicationLog";
            this.ToggleApplicationLog.Size = new System.Drawing.Size(65, 27);
            this.ToggleApplicationLog.TabIndex = 9;
            this.ToggleApplicationLog.Text = "App";
            this.ToggleApplicationLog.UseVisualStyleBackColor = false;
            this.ToggleApplicationLog.Click += new System.EventHandler(this.ToggleApplicationLog_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.buttonImportPredb);
            this.groupBox3.Controls.Add(this.Prebot_edit_button);
            this.groupBox3.Controls.Add(this.Add_PreBot_button);
            this.groupBox3.Location = new System.Drawing.Point(846, 30);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(232, 51);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PreBots";
            // 
            // buttonImportPredb
            // 
            this.buttonImportPredb.BackColor = System.Drawing.Color.Gray;
            this.buttonImportPredb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonImportPredb.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.buttonImportPredb.Location = new System.Drawing.Point(153, 19);
            this.buttonImportPredb.Margin = new System.Windows.Forms.Padding(4);
            this.buttonImportPredb.Name = "buttonImportPredb";
            this.buttonImportPredb.Size = new System.Drawing.Size(70, 25);
            this.buttonImportPredb.TabIndex = 54;
            this.buttonImportPredb.Text = "Import";
            this.buttonImportPredb.UseVisualStyleBackColor = false;
            this.buttonImportPredb.Click += new System.EventHandler(this.buttonImportPredb_Click);
            // 
            // Prebot_edit_button
            // 
            this.Prebot_edit_button.BackColor = System.Drawing.Color.LightSteelBlue;
            this.Prebot_edit_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Prebot_edit_button.Location = new System.Drawing.Point(76, 19);
            this.Prebot_edit_button.Name = "Prebot_edit_button";
            this.Prebot_edit_button.Size = new System.Drawing.Size(70, 25);
            this.Prebot_edit_button.TabIndex = 11;
            this.Prebot_edit_button.Text = "Edit";
            this.Prebot_edit_button.UseVisualStyleBackColor = false;
            this.Prebot_edit_button.Click += new System.EventHandler(this.Prebot_edit_button_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.BackColor = System.Drawing.Color.Transparent;
            this.groupBox6.Controls.Add(this.blacklist_add);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Location = new System.Drawing.Point(405, 84);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox6.Size = new System.Drawing.Size(196, 58);
            this.groupBox6.TabIndex = 12;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Settings";
            // 
            // blacklist_add
            // 
            this.blacklist_add.BackColor = System.Drawing.Color.Violet;
            this.blacklist_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.blacklist_add.Location = new System.Drawing.Point(96, 21);
            this.blacklist_add.Name = "blacklist_add";
            this.blacklist_add.Size = new System.Drawing.Size(91, 27);
            this.blacklist_add.TabIndex = 12;
            this.blacklist_add.Text = "Blacklist";
            this.blacklist_add.UseVisualStyleBackColor = false;
            this.blacklist_add.Click += new System.EventHandler(this.blacklist_add_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Bisque;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(8, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 27);
            this.button1.TabIndex = 11;
            this.button1.Text = "Settings";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Help_button
            // 
            this.Help_button.BackColor = System.Drawing.Color.LightCoral;
            this.Help_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Help_button.Location = new System.Drawing.Point(315, 21);
            this.Help_button.Name = "Help_button";
            this.Help_button.Size = new System.Drawing.Size(75, 27);
            this.Help_button.TabIndex = 12;
            this.Help_button.Text = "Help";
            this.Help_button.UseVisualStyleBackColor = false;
            this.Help_button.Click += new System.EventHandler(this.Help_button_Click_1);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(185, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(56, 27);
            this.button2.TabIndex = 0;
            this.button2.Text = "Test Release";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // Sync_From_Cbftp_Button
            // 
            this.Sync_From_Cbftp_Button.BackColor = System.Drawing.Color.RosyBrown;
            this.Sync_From_Cbftp_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Sync_From_Cbftp_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Sync_From_Cbftp_Button.Location = new System.Drawing.Point(81, 21);
            this.Sync_From_Cbftp_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Sync_From_Cbftp_Button.Name = "Sync_From_Cbftp_Button";
            this.Sync_From_Cbftp_Button.Size = new System.Drawing.Size(98, 27);
            this.Sync_From_Cbftp_Button.TabIndex = 10;
            this.Sync_From_Cbftp_Button.Text = "Sync Sites";
            this.Sync_From_Cbftp_Button.UseVisualStyleBackColor = false;
            this.Sync_From_Cbftp_Button.Click += new System.EventHandler(this.Sync_From_Cbftp_Button_Click);
            // 
            // Functions_groupbox
            // 
            this.Functions_groupbox.Controls.Add(this.Pre_button);
            this.Functions_groupbox.Controls.Add(this.OpenTabbedIRC);
            this.Functions_groupbox.Controls.Add(this.Sync_From_Cbftp_Button);
            this.Functions_groupbox.Controls.Add(this.Help_button);
            this.Functions_groupbox.Controls.Add(this.exitButton);
            this.Functions_groupbox.Controls.Add(this.button2);
            this.Functions_groupbox.Location = new System.Drawing.Point(607, 84);
            this.Functions_groupbox.Name = "Functions_groupbox";
            this.Functions_groupbox.Size = new System.Drawing.Size(471, 58);
            this.Functions_groupbox.TabIndex = 13;
            this.Functions_groupbox.TabStop = false;
            this.Functions_groupbox.Text = "Functions";
            // 
            // Pre_button
            // 
            this.Pre_button.BackColor = System.Drawing.Color.MediumPurple;
            this.Pre_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Pre_button.Location = new System.Drawing.Point(247, 21);
            this.Pre_button.Name = "Pre_button";
            this.Pre_button.Size = new System.Drawing.Size(62, 27);
            this.Pre_button.TabIndex = 13;
            this.Pre_button.Text = "Pre";
            this.Pre_button.UseVisualStyleBackColor = false;
            this.Pre_button.Click += new System.EventHandler(this.Pre_button_Click);
            // 
            // OpenTabbedIRC
            // 
            this.OpenTabbedIRC.BackColor = System.Drawing.Color.SandyBrown;
            this.OpenTabbedIRC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenTabbedIRC.Location = new System.Drawing.Point(11, 21);
            this.OpenTabbedIRC.Name = "OpenTabbedIRC";
            this.OpenTabbedIRC.Size = new System.Drawing.Size(64, 27);
            this.OpenTabbedIRC.TabIndex = 10;
            this.OpenTabbedIRC.Text = "Chat";
            this.OpenTabbedIRC.UseVisualStyleBackColor = false;
            this.OpenTabbedIRC.Click += new System.EventHandler(this.OpenTabbedIRC_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "RaceTrade v1.0.7b";
            // 
            // lblDockLogs
            // 
            this.lblDockLogs.AutoSize = true;
            this.lblDockLogs.Location = new System.Drawing.Point(974, 9);
            this.lblDockLogs.Name = "lblDockLogs";
            this.lblDockLogs.Size = new System.Drawing.Size(104, 17);
            this.lblDockLogs.TabIndex = 15;
            this.lblDockLogs.Text = "Logs: Docked";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(465, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "linknet/#racetrade";
            // 
            // MainApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1092, 154);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDockLogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Functions_groupbox);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1_cbftp);
            this.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "MainApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RaceTrade (2025) v1.0.7beta  ▂▃▅▇█▓▒░۩۞۩ HiGH VOLTAGE ۩۞۩░▒▓█▇▅▃▂ ";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1_cbftp.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.Functions_groupbox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1_cbftp;
        private System.Windows.Forms.ComboBox EditDropdown_cbftp_comboBox;
        private System.Windows.Forms.Button Add_Cbftp_button;
        private System.Windows.Forms.Button add_sites_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button Enable_Disable_Racer_button;
        private System.Windows.Forms.Button ToggleIRCLog;
        private System.Windows.Forms.Button Add_Ccbftp_Sections;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button ToggleCBFTPLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Add_PreBot_button;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button Sync_From_Cbftp_Button;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button Help_button;
        private System.Windows.Forms.Button Edit_sites_button;
        private System.Windows.Forms.Button Prebot_edit_button;
        private System.Windows.Forms.GroupBox Functions_groupbox;
        private System.Windows.Forms.Button OpenTabbedIRC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ToggleApplicationLog;
        private System.Windows.Forms.Button ToggleRaceLog;
        private System.Windows.Forms.Button Add_cbftp_site;
        private System.Windows.Forms.Button Cbftp_Edit_Site;
        private System.Windows.Forms.Button blacklist_add;
        private System.Windows.Forms.Label lblDockLogs;
        private System.Windows.Forms.Button Pre_button;
        private System.Windows.Forms.Button buttonImportPredb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Ftp_button;
    }
}