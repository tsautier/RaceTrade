using System.Drawing;
using System.Windows.Forms;

namespace RaceTrader
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox appNameTextBox;
        private CheckBox debugCheckBox;
        private CheckBox insecureSslCheckBox;
        private Button saveButton;
        private Button cancelButton;
        private Button testButton;
        private Label statusLabel;
        private GroupBox generalGroupBox;
        private GroupBox apiGroupBox;
        private Label titleLabel;
        private Label appNameLabel;
        private Label omdbLabel;
        private Label movieProviderLabel;
        private ComboBox movieProviderComboBox;
        private Label tmdbApiKeyLabel;
        private TextBox tmdbApiKeyTextBox;
        private LinkLabel getLinkLabel;

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
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.insecureSslCheckBox = new System.Windows.Forms.CheckBox();
            this.debugCheckBox = new System.Windows.Forms.CheckBox();
            this.appNameTextBox = new System.Windows.Forms.TextBox();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.apiGroupBox = new System.Windows.Forms.GroupBox();
            this.testButton = new System.Windows.Forms.Button();
            this.getLinkLabel = new System.Windows.Forms.LinkLabel();
            this.omdbLabel = new System.Windows.Forms.Label();
            this.movieProviderLabel = new System.Windows.Forms.Label();
            this.movieProviderComboBox = new System.Windows.Forms.ComboBox();
            this.tmdbApiKeyLabel = new System.Windows.Forms.Label();
            this.tmdbApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.disableAppLogCheckBox = new System.Windows.Forms.CheckBox();
            this.disableCbftpLogCheckBox = new System.Windows.Forms.CheckBox();
            this.disableRaceLogCheckBox = new System.Windows.Forms.CheckBox();
            this.generalGroupBox.SuspendLayout();
            this.apiGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            //
            // generalGroupBox
            //
            this.generalGroupBox.Controls.Add(this.insecureSslCheckBox);
            this.generalGroupBox.Controls.Add(this.debugCheckBox);
            this.generalGroupBox.Controls.Add(this.appNameTextBox);
            this.generalGroupBox.Controls.Add(this.appNameLabel);
            this.generalGroupBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F, System.Drawing.FontStyle.Bold);
            this.generalGroupBox.ForeColor = System.Drawing.Color.White;
            this.generalGroupBox.Location = new System.Drawing.Point(20, 60);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(480, 140);
            this.generalGroupBox.TabIndex = 1;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            //
            // insecureSslCheckBox
            //
            this.insecureSslCheckBox.AutoSize = true;
            this.insecureSslCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.insecureSslCheckBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.insecureSslCheckBox.ForeColor = System.Drawing.Color.White;
            this.insecureSslCheckBox.Location = new System.Drawing.Point(15, 110);
            this.insecureSslCheckBox.Name = "insecureSslCheckBox";
            this.insecureSslCheckBox.Size = new System.Drawing.Size(398, 21);
            this.insecureSslCheckBox.TabIndex = 0;
            this.insecureSslCheckBox.Text = "Allow insecure SSL for Cbftp (NOT recommended)";
            this.insecureSslCheckBox.UseVisualStyleBackColor = true;
            // 
            // debugCheckBox
            // 
            this.debugCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.debugCheckBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.debugCheckBox.ForeColor = System.Drawing.Color.White;
            this.debugCheckBox.Location = new System.Drawing.Point(15, 85);
            this.debugCheckBox.Name = "debugCheckBox";
            this.debugCheckBox.Size = new System.Drawing.Size(200, 20);
            this.debugCheckBox.TabIndex = 1;
            this.debugCheckBox.Text = "Enable Debug Mode";
            this.debugCheckBox.UseVisualStyleBackColor = true;
            // 
            // appNameTextBox
            // 
            this.appNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(50)))));
            this.appNameTextBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.appNameTextBox.ForeColor = System.Drawing.Color.White;
            this.appNameTextBox.Location = new System.Drawing.Point(15, 50);
            this.appNameTextBox.Name = "appNameTextBox";
            this.appNameTextBox.Size = new System.Drawing.Size(440, 24);
            this.appNameTextBox.TabIndex = 2;
            this.appNameTextBox.Text = "RaceTrade (2025) ▂▃▅▇█▓▒░۩۞۩ HiGH VOLTAGE ۩۞۩░▒▓█▇▅▃▂ ";
            // 
            // appNameLabel
            // 
            this.appNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.appNameLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.appNameLabel.ForeColor = System.Drawing.Color.White;
            this.appNameLabel.Location = new System.Drawing.Point(15, 25);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(250, 20);
            this.appNameLabel.TabIndex = 3;
            this.appNameLabel.Text = "Application Name (Taskbar):";
            // 
            // apiGroupBox
            // 
            this.apiGroupBox.Controls.Add(this.testButton);
            this.apiGroupBox.Controls.Add(this.getLinkLabel);
            this.apiGroupBox.Controls.Add(this.omdbLabel);
            this.apiGroupBox.Controls.Add(this.movieProviderLabel);
            this.apiGroupBox.Controls.Add(this.movieProviderComboBox);
            this.apiGroupBox.Controls.Add(this.tmdbApiKeyLabel);
            this.apiGroupBox.Controls.Add(this.tmdbApiKeyTextBox);
            this.apiGroupBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F, System.Drawing.FontStyle.Bold);
            this.apiGroupBox.ForeColor = System.Drawing.Color.White;
            this.apiGroupBox.Location = new System.Drawing.Point(20, 215);
            this.apiGroupBox.Name = "apiGroupBox";
            this.apiGroupBox.Size = new System.Drawing.Size(480, 180);
            this.apiGroupBox.TabIndex = 2;
            this.apiGroupBox.TabStop = false;
            this.apiGroupBox.Text = "Movie APIs";
            // 
            // testButton
            // 
            this.testButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(80)))), ((int)(((byte)(98)))));
            this.testButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.testButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.testButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.testButton.ForeColor = System.Drawing.Color.White;
            this.testButton.Location = new System.Drawing.Point(26, 146);
            this.testButton.Name = "testButton";
            this.testButton.Size = new System.Drawing.Size(150, 28);
            this.testButton.TabIndex = 0;
            this.testButton.Text = "Test Connection";
            this.testButton.UseVisualStyleBackColor = false;
            this.testButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // getLinkLabel
            // 
            this.getLinkLabel.Location = new System.Drawing.Point(191, 151);
            this.getLinkLabel.Name = "getLinkLabel";
            this.getLinkLabel.Size = new System.Drawing.Size(264, 23);
            this.getLinkLabel.TabIndex = 2;
            this.getLinkLabel.Text = "Get TMDb key";
            this.getLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.getLinkLabel_LinkClicked);
            // 
            // omdbLabel
            // 
            this.omdbLabel.BackColor = System.Drawing.Color.Transparent;
            this.omdbLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.omdbLabel.ForeColor = System.Drawing.Color.White;
            this.omdbLabel.Location = new System.Drawing.Point(23, 20);
            this.omdbLabel.Name = "omdbLabel";
            this.omdbLabel.Size = new System.Drawing.Size(420, 20);
            this.omdbLabel.TabIndex = 4;
            this.omdbLabel.Text = "Movie lookup provider";
            //
            // movieProviderLabel
            //
            this.movieProviderLabel.BackColor = System.Drawing.Color.Transparent;
            this.movieProviderLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.movieProviderLabel.ForeColor = System.Drawing.Color.White;
            this.movieProviderLabel.Location = new System.Drawing.Point(23, 45);
            this.movieProviderLabel.Name = "movieProviderLabel";
            this.movieProviderLabel.Size = new System.Drawing.Size(180, 20);
            this.movieProviderLabel.TabIndex = 7;
            this.movieProviderLabel.Text = "Provider:";
            //
            // movieProviderComboBox
            //
            this.movieProviderComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(50)))));
            this.movieProviderComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.movieProviderComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.movieProviderComboBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.movieProviderComboBox.ForeColor = System.Drawing.Color.White;
            this.movieProviderComboBox.FormattingEnabled = true;
            this.movieProviderComboBox.Items.AddRange(new object[] {
            "Tiffara",
            "TMDb"});
            this.movieProviderComboBox.Location = new System.Drawing.Point(26, 66);
            this.movieProviderComboBox.Name = "movieProviderComboBox";
            this.movieProviderComboBox.Size = new System.Drawing.Size(429, 25);
            this.movieProviderComboBox.TabIndex = 8;
            //
            // tmdbApiKeyLabel
            //
            this.tmdbApiKeyLabel.BackColor = System.Drawing.Color.Transparent;
            this.tmdbApiKeyLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.tmdbApiKeyLabel.ForeColor = System.Drawing.Color.White;
            this.tmdbApiKeyLabel.Location = new System.Drawing.Point(23, 96);
            this.tmdbApiKeyLabel.Name = "tmdbApiKeyLabel";
            this.tmdbApiKeyLabel.Size = new System.Drawing.Size(180, 20);
            this.tmdbApiKeyLabel.TabIndex = 5;
            this.tmdbApiKeyLabel.Text = "TMDb API key:";
            //
            // tmdbApiKeyTextBox
            //
            this.tmdbApiKeyTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(50)))));
            this.tmdbApiKeyTextBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.tmdbApiKeyTextBox.ForeColor = System.Drawing.Color.White;
            this.tmdbApiKeyTextBox.Location = new System.Drawing.Point(26, 117);
            this.tmdbApiKeyTextBox.Name = "tmdbApiKeyTextBox";
            this.tmdbApiKeyTextBox.PasswordChar = '*';
            this.tmdbApiKeyTextBox.Size = new System.Drawing.Size(429, 24);
            this.tmdbApiKeyTextBox.TabIndex = 6;
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.statusLabel.ForeColor = System.Drawing.Color.Yellow;
            this.statusLabel.Location = new System.Drawing.Point(20, 400);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(480, 20);
            this.statusLabel.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(294, 605);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 30);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.saveButton.ForeColor = System.Drawing.Color.White;
            this.saveButton.Location = new System.Drawing.Point(400, 605);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(100, 30);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // titleLabel
            // 
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.Font = new System.Drawing.Font("Cascadia Mono", 15F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(18, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(480, 48);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Settings";
            this.titleLabel.Click += new System.EventHandler(this.titleLabel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.disableAppLogCheckBox);
            this.groupBox1.Controls.Add(this.disableCbftpLogCheckBox);
            this.groupBox1.Controls.Add(this.disableRaceLogCheckBox);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Location = new System.Drawing.Point(20, 425);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 174);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Logging";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(240, 34);
            this.label2.TabIndex = 6;
            this.label2.Text = "Disabling a log will disable \r\nthe buttons on the mainform";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(253, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "Clean Dupe Log in minutes";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(256, 45);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(205, 24);
            this.numericUpDown1.TabIndex = 4;
            // 
            // disableAppLogCheckBox
            // 
            this.disableAppLogCheckBox.AutoSize = true;
            this.disableAppLogCheckBox.Location = new System.Drawing.Point(10, 126);
            this.disableAppLogCheckBox.Name = "disableAppLogCheckBox";
            this.disableAppLogCheckBox.Size = new System.Drawing.Size(214, 21);
            this.disableAppLogCheckBox.TabIndex = 2;
            this.disableAppLogCheckBox.Text = "Disable Application Log";
            this.disableAppLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // disableCbftpLogCheckBox
            // 
            this.disableCbftpLogCheckBox.AutoSize = true;
            this.disableCbftpLogCheckBox.Location = new System.Drawing.Point(10, 99);
            this.disableCbftpLogCheckBox.Name = "disableCbftpLogCheckBox";
            this.disableCbftpLogCheckBox.Size = new System.Drawing.Size(166, 21);
            this.disableCbftpLogCheckBox.TabIndex = 1;
            this.disableCbftpLogCheckBox.Text = "Disable CBFTP Log";
            this.disableCbftpLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // disableRaceLogCheckBox
            // 
            this.disableRaceLogCheckBox.AutoSize = true;
            this.disableRaceLogCheckBox.Location = new System.Drawing.Point(10, 72);
            this.disableRaceLogCheckBox.Name = "disableRaceLogCheckBox";
            this.disableRaceLogCheckBox.Size = new System.Drawing.Size(158, 21);
            this.disableRaceLogCheckBox.TabIndex = 0;
            this.disableRaceLogCheckBox.Text = "Disable Race Log";
            this.disableRaceLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(26)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(520, 642);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.generalGroupBox);
            this.Controls.Add(this.apiGroupBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.saveButton);
            this.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.apiGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        private GroupBox groupBox1;
        private CheckBox disableRaceLogCheckBox;
        private CheckBox disableAppLogCheckBox;
        private CheckBox disableCbftpLogCheckBox;
        private Label label2;
        private Label label1;
        private NumericUpDown numericUpDown1;
    }
}
