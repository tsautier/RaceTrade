using System.Windows.Forms;
using System.Drawing;

namespace RaceTrader
{
    partial class GlobalBlacklistForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label titleLabel;
        private GroupBox patternsGroupBox;
        private Button clearButton;
        private Button removeButton;
        private ListBox patternsListBox;
        private Label patternLabel;
        private TextBox patternTextBox;
        private Button addButton;
        private Label hintLabel;
        private CheckBox enabledCheckBox;
        private Button closeButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.patternsGroupBox = new System.Windows.Forms.GroupBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.patternsListBox = new System.Windows.Forms.ListBox();
            this.patternLabel = new System.Windows.Forms.Label();
            this.patternTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.hintLabel = new System.Windows.Forms.Label();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.patternsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(18, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(480, 48);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Global Blacklist";
            // 
            // patternsGroupBox
            // 
            this.patternsGroupBox.Controls.Add(this.clearButton);
            this.patternsGroupBox.Controls.Add(this.removeButton);
            this.patternsGroupBox.Controls.Add(this.patternsListBox);
            this.patternsGroupBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold);
            this.patternsGroupBox.ForeColor = System.Drawing.Color.White;
            this.patternsGroupBox.Location = new System.Drawing.Point(20, 60);
            this.patternsGroupBox.Name = "patternsGroupBox";
            this.patternsGroupBox.Size = new System.Drawing.Size(560, 280);
            this.patternsGroupBox.TabIndex = 1;
            this.patternsGroupBox.TabStop = false;
            this.patternsGroupBox.Text = "Blacklist Patterns";
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.clearButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.clearButton.ForeColor = System.Drawing.Color.White;
            this.clearButton.Location = new System.Drawing.Point(175, 230);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(150, 30);
            this.clearButton.TabIndex = 2;
            this.clearButton.Text = "Clear All";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.removeButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.removeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeButton.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.removeButton.ForeColor = System.Drawing.Color.White;
            this.removeButton.Location = new System.Drawing.Point(15, 230);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(150, 30);
            this.removeButton.TabIndex = 1;
            this.removeButton.Text = "Remove Selected";
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // patternsListBox
            // 
            this.patternsListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.patternsListBox.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.patternsListBox.ForeColor = System.Drawing.Color.White;
            this.patternsListBox.FormattingEnabled = true;
            this.patternsListBox.ItemHeight = 17;
            this.patternsListBox.Location = new System.Drawing.Point(15, 25);
            this.patternsListBox.Name = "patternsListBox";
            this.patternsListBox.Size = new System.Drawing.Size(530, 174);
            this.patternsListBox.TabIndex = 0;
            // 
            // patternLabel
            // 
            this.patternLabel.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.patternLabel.ForeColor = System.Drawing.Color.White;
            this.patternLabel.Location = new System.Drawing.Point(20, 355);
            this.patternLabel.Name = "patternLabel";
            this.patternLabel.Size = new System.Drawing.Size(350, 20);
            this.patternLabel.TabIndex = 2;
            this.patternLabel.Text = "Add Pattern (wildcards: * = any, ? = single):";
            // 
            // patternTextBox
            // 
            this.patternTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.patternTextBox.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.patternTextBox.ForeColor = System.Drawing.Color.White;
            this.patternTextBox.Location = new System.Drawing.Point(20, 378);
            this.patternTextBox.Name = "patternTextBox";
            this.patternTextBox.Size = new System.Drawing.Size(400, 24);
            this.patternTextBox.TabIndex = 3;
            this.patternTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.patternTextBox_KeyDown);
            // 
            // addButton
            // 
            this.addButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.addButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.addButton.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.addButton.ForeColor = System.Drawing.Color.White;
            this.addButton.Location = new System.Drawing.Point(430, 375);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(150, 30);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "Add Pattern";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // hintLabel
            // 
            this.hintLabel.Font = new System.Drawing.Font("Consolas", 7F, System.Drawing.FontStyle.Italic);
            this.hintLabel.ForeColor = System.Drawing.Color.Gray;
            this.hintLabel.Location = new System.Drawing.Point(20, 407);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(560, 35);
            this.hintLabel.TabIndex = 5;
            this.hintLabel.Text = "Examples: *INTERNAL*, *-NUKE*, *.XXX.*, or regex: ^.*REPACK.*$\r\nPatterns are case" +
    "-insensitive and match against full release names.";
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.enabledCheckBox.ForeColor = System.Drawing.Color.White;
            this.enabledCheckBox.Location = new System.Drawing.Point(20, 450);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.Size = new System.Drawing.Size(300, 20);
            this.enabledCheckBox.TabIndex = 6;
            this.enabledCheckBox.Text = "Enable Global Blacklist";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.enabledCheckBox_CheckedChanged);
            // 
            // closeButton
            // 
            this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.closeButton.ForeColor = System.Drawing.Color.White;
            this.closeButton.Location = new System.Drawing.Point(480, 445);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 30);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // GlobalBlacklistForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(600, 490);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.enabledCheckBox);
            this.Controls.Add(this.hintLabel);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.patternTextBox);
            this.Controls.Add(this.patternLabel);
            this.Controls.Add(this.patternsGroupBox);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalBlacklistForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Global Blacklist Manager";
            this.patternsGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
