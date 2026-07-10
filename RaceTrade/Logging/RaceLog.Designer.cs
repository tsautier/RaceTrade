using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    partial class RaceLog
    {
        private System.ComponentModel.IContainer components = null;

        private Panel toolbar;
        private TextBox searchBox;
        private ComboBox filterComboBox;
        private CheckBox autoScrollCheckBox;
        private Button clearButton;
        private Button exportButton;
        private RichTextBox raceLogRichTextBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.toolbar = new System.Windows.Forms.Panel();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.filterComboBox = new System.Windows.Forms.ComboBox();
            this.autoScrollCheckBox = new System.Windows.Forms.CheckBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.exportButton = new System.Windows.Forms.Button();
            this.raceLogRichTextBox = new System.Windows.Forms.RichTextBox();
            this.toolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbar
            // 
            this.toolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(26)))), ((int)(((byte)(36)))));
            this.toolbar.Controls.Add(this.searchBox);
            this.toolbar.Controls.Add(this.filterComboBox);
            this.toolbar.Controls.Add(this.autoScrollCheckBox);
            this.toolbar.Controls.Add(this.clearButton);
            this.toolbar.Controls.Add(this.exportButton);
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(964, 40);
            this.toolbar.TabIndex = 0;
            // 
            // searchBox
            // 
            this.searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.searchBox.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.searchBox.ForeColor = System.Drawing.Color.Gray;
            this.searchBox.Location = new System.Drawing.Point(10, 10);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(200, 25);
            this.searchBox.TabIndex = 0;
            this.searchBox.Text = "Search logs...";
            this.searchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            this.searchBox.GotFocus += new System.EventHandler(this.SearchBox_GotFocus);
            this.searchBox.LostFocus += new System.EventHandler(this.SearchBox_LostFocus);
            // 
            // filterComboBox
            // 
            this.filterComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.filterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.filterComboBox.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.filterComboBox.ForeColor = System.Drawing.Color.White;
            this.filterComboBox.FormattingEnabled = true;
            this.filterComboBox.Items.AddRange(new object[] {
            "All",
            "Detected",
            "Racing",
            "Complete",
            "Filtered",
            "Failed"});
            this.filterComboBox.Location = new System.Drawing.Point(216, 9);
            this.filterComboBox.Name = "filterComboBox";
            this.filterComboBox.Size = new System.Drawing.Size(132, 26);
            this.filterComboBox.TabIndex = 1;
            this.filterComboBox.SelectedIndexChanged += new System.EventHandler(this.FilterComboBox_SelectedIndexChanged);
            // 
            // autoScrollCheckBox
            // 
            this.autoScrollCheckBox.AutoSize = true;
            this.autoScrollCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.autoScrollCheckBox.Checked = true;
            this.autoScrollCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoScrollCheckBox.Font = new System.Drawing.Font("Cascadia Mono", 8F);
            this.autoScrollCheckBox.ForeColor = System.Drawing.Color.White;
            this.autoScrollCheckBox.Location = new System.Drawing.Point(358, 12);
            this.autoScrollCheckBox.Name = "autoScrollCheckBox";
            this.autoScrollCheckBox.Size = new System.Drawing.Size(118, 21);
            this.autoScrollCheckBox.TabIndex = 2;
            this.autoScrollCheckBox.Text = "Auto-scroll";
            this.autoScrollCheckBox.UseVisualStyleBackColor = false;
            this.autoScrollCheckBox.CheckedChanged += new System.EventHandler(this.autoScrollCheckBox_CheckedChanged);
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(42)))), ((int)(((byte)(54)))));
            this.clearButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(48)))), ((int)(((byte)(62)))));
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Font = new System.Drawing.Font("Cascadia Mono", 8F);
            this.clearButton.ForeColor = System.Drawing.Color.White;
            this.clearButton.Location = new System.Drawing.Point(486, 8);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(84, 26);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // exportButton
            // 
            this.exportButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(42)))), ((int)(((byte)(54)))));
            this.exportButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(48)))), ((int)(((byte)(62)))));
            this.exportButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exportButton.Font = new System.Drawing.Font("Cascadia Mono", 8F);
            this.exportButton.ForeColor = System.Drawing.Color.White;
            this.exportButton.Location = new System.Drawing.Point(578, 8);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(84, 26);
            this.exportButton.TabIndex = 4;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = false;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // raceLogRichTextBox
            // 
            this.raceLogRichTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.raceLogRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.raceLogRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.raceLogRichTextBox.Font = new System.Drawing.Font("Cascadia Mono", 8F);
            this.raceLogRichTextBox.ForeColor = System.Drawing.Color.White;
            this.raceLogRichTextBox.Location = new System.Drawing.Point(0, 40);
            this.raceLogRichTextBox.Name = "raceLogRichTextBox";
            this.raceLogRichTextBox.ReadOnly = true;
            this.raceLogRichTextBox.Size = new System.Drawing.Size(964, 560);
            this.raceLogRichTextBox.TabIndex = 1;
            this.raceLogRichTextBox.Text = "";
            // 
            // RaceLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(964, 600);
            this.Controls.Add(this.raceLogRichTextBox);
            this.Controls.Add(this.toolbar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "RaceLog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Race Log";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
