using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    partial class CbftpSyncForm
    {
        private System.ComponentModel.IContainer components = null;

        private ComboBox cbftpServerCombo;
        private Button syncButton;
        private Button cancelButton;
        private Button importButton;
        private CheckedListBox sitesListBox;
        private Label statusLabel;
        private ProgressBar progressBar;
        private Label serverLabel;
        private Label sitesLabel;
        private Button selectAllButton;
        private Button deselectAllButton;

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
            this.cbftpServerCombo = new System.Windows.Forms.ComboBox();
            this.syncButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.sitesListBox = new System.Windows.Forms.CheckedListBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.serverLabel = new System.Windows.Forms.Label();
            this.sitesLabel = new System.Windows.Forms.Label();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.deselectAllButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cbftpServerCombo
            // 
            this.cbftpServerCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(50)))));
            this.cbftpServerCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbftpServerCombo.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.cbftpServerCombo.ForeColor = System.Drawing.Color.White;
            this.cbftpServerCombo.FormattingEnabled = true;
            this.cbftpServerCombo.Location = new System.Drawing.Point(150, 18);
            this.cbftpServerCombo.Name = "cbftpServerCombo";
            this.cbftpServerCombo.Size = new System.Drawing.Size(420, 25);
            this.cbftpServerCombo.TabIndex = 1;
            // 
            // syncButton
            // 
            this.syncButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.syncButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.syncButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.syncButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.syncButton.ForeColor = System.Drawing.Color.White;
            this.syncButton.Location = new System.Drawing.Point(150, 55);
            this.syncButton.Name = "syncButton";
            this.syncButton.Size = new System.Drawing.Size(120, 30);
            this.syncButton.TabIndex = 2;
            this.syncButton.Text = "Fetch Sites";
            this.syncButton.UseVisualStyleBackColor = false;
            this.syncButton.Click += new System.EventHandler(this.SyncButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.cancelButton.ForeColor = System.Drawing.Color.White;
            this.cancelButton.Location = new System.Drawing.Point(440, 373);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(130, 30);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // importButton
            // 
            this.importButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.importButton.Enabled = false;
            this.importButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.importButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.importButton.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.importButton.ForeColor = System.Drawing.Color.White;
            this.importButton.Location = new System.Drawing.Point(244, 373);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(190, 30);
            this.importButton.TabIndex = 9;
            this.importButton.Text = "Import Selected Sites";
            this.importButton.UseVisualStyleBackColor = false;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // sitesListBox
            // 
            this.sitesListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(38)))), ((int)(((byte)(50)))));
            this.sitesListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sitesListBox.CheckOnClick = true;
            this.sitesListBox.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.sitesListBox.ForeColor = System.Drawing.Color.White;
            this.sitesListBox.Location = new System.Drawing.Point(20, 175);
            this.sitesListBox.Name = "sitesListBox";
            this.sitesListBox.Size = new System.Drawing.Size(550, 192);
            this.sitesListBox.TabIndex = 8;
            this.sitesListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.sitesListBox_ItemCheck);
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.Color.Transparent;
            this.statusLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(20, 95);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(550, 20);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Select a CBFTP server and click \'Fetch Sites\'";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 120);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(550, 20);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 4;
            this.progressBar.Visible = false;
            // 
            // serverLabel
            // 
            this.serverLabel.BackColor = System.Drawing.Color.Transparent;
            this.serverLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.serverLabel.ForeColor = System.Drawing.Color.White;
            this.serverLabel.Location = new System.Drawing.Point(20, 20);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(120, 20);
            this.serverLabel.TabIndex = 0;
            this.serverLabel.Text = "CBFTP Server:";
            // 
            // sitesLabel
            // 
            this.sitesLabel.BackColor = System.Drawing.Color.Transparent;
            this.sitesLabel.Font = new System.Drawing.Font("Cascadia Mono", 8.25F);
            this.sitesLabel.ForeColor = System.Drawing.Color.White;
            this.sitesLabel.Location = new System.Drawing.Point(20, 150);
            this.sitesLabel.Name = "sitesLabel";
            this.sitesLabel.Size = new System.Drawing.Size(390, 20);
            this.sitesLabel.TabIndex = 5;
            this.sitesLabel.Text = "Sites to Import (check to import):";
            // 
            // selectAllButton
            // 
            this.selectAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.selectAllButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.selectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectAllButton.Font = new System.Drawing.Font("Cascadia Mono", 7.25F);
            this.selectAllButton.ForeColor = System.Drawing.Color.White;
            this.selectAllButton.Location = new System.Drawing.Point(415, 147);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(70, 25);
            this.selectAllButton.TabIndex = 6;
            this.selectAllButton.Text = "Select All";
            this.selectAllButton.UseVisualStyleBackColor = false;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // deselectAllButton
            // 
            this.deselectAllButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.deselectAllButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.deselectAllButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deselectAllButton.Font = new System.Drawing.Font("Cascadia Mono", 7.25F);
            this.deselectAllButton.ForeColor = System.Drawing.Color.White;
            this.deselectAllButton.Location = new System.Drawing.Point(490, 147);
            this.deselectAllButton.Name = "deselectAllButton";
            this.deselectAllButton.Size = new System.Drawing.Size(80, 25);
            this.deselectAllButton.TabIndex = 7;
            this.deselectAllButton.Text = "Deselect All";
            this.deselectAllButton.UseVisualStyleBackColor = false;
            this.deselectAllButton.Click += new System.EventHandler(this.deselectAllButton_Click);
            // 
            // CbftpSyncForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(26)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(590, 418);
            this.Controls.Add(this.serverLabel);
            this.Controls.Add(this.cbftpServerCombo);
            this.Controls.Add(this.syncButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.sitesLabel);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.deselectAllButton);
            this.Controls.Add(this.sitesListBox);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.cancelButton);
            this.Font = new System.Drawing.Font("Cascadia Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CbftpSyncForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sync Sites from CBFTP";
            this.ResumeLayout(false);

        }
    }
}
