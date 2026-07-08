using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    partial class ChannelTab
    {
        private System.ComponentModel.IContainer components = null;

        private RichTextBox outputBox;
        private ListBox userListBox;
        private TextBox inputBox;
        private Button sendButton;
        private Panel mainPanel;
        private Panel userListPanel;
        private Panel inputPanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.outputBox = new System.Windows.Forms.RichTextBox();
            this.userListBox = new System.Windows.Forms.ListBox();
            this.inputBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.userListPanel = new System.Windows.Forms.Panel();
            this.inputPanel = new System.Windows.Forms.Panel();
            this.mainPanel.SuspendLayout();
            this.userListPanel.SuspendLayout();
            this.inputPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // outputBox
            // 
            this.outputBox.BackColor = System.Drawing.Color.White;
            this.outputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.outputBox.ForeColor = System.Drawing.Color.Black;
            this.outputBox.Location = new System.Drawing.Point(0, 0);
            this.outputBox.Margin = new System.Windows.Forms.Padding(0);
            this.outputBox.Name = "outputBox";
            this.outputBox.ReadOnly = true;
            this.outputBox.Size = new System.Drawing.Size(840, 560);
            this.outputBox.TabIndex = 0;
            this.outputBox.Text = "";
            this.outputBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OutputBox_MouseUp);
            // 
            // userListBox
            // 
            this.userListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(20)))), ((int)(((byte)(28)))));
            this.userListBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.userListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userListBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.userListBox.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.userListBox.ForeColor = System.Drawing.Color.White;
            this.userListBox.FormattingEnabled = true;
            this.userListBox.IntegralHeight = false;
            this.userListBox.ItemHeight = 18;
            this.userListBox.Location = new System.Drawing.Point(0, 0);
            this.userListBox.Name = "userListBox";
            this.userListBox.Size = new System.Drawing.Size(160, 560);
            this.userListBox.TabIndex = 1;
            this.userListBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.UserListBox_MouseClick);
            this.userListBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.UserListBox_DrawItem);
            this.userListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.UserListBox_MouseDoubleClick);
            // 
            // inputBox
            // 
            this.inputBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(20)))), ((int)(((byte)(28)))));
            this.inputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inputBox.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.inputBox.ForeColor = System.Drawing.Color.White;
            this.inputBox.Location = new System.Drawing.Point(5, 5);
            this.inputBox.Name = "inputBox";
            this.inputBox.Size = new System.Drawing.Size(915, 25);
            this.inputBox.TabIndex = 0;
            this.inputBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputBox_KeyDown);
            // 
            // sendButton
            // 
            this.sendButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(50)))), ((int)(((byte)(64)))));
            this.sendButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.sendButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(48)))), ((int)(((byte)(62)))));
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Font = new System.Drawing.Font("Cascadia Mono", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sendButton.ForeColor = System.Drawing.Color.White;
            this.sendButton.Location = new System.Drawing.Point(920, 5);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 30);
            this.sendButton.TabIndex = 1;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = false;
            this.sendButton.Click += new System.EventHandler(this.SendButton_Click);
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(46)))));
            this.mainPanel.Controls.Add(this.outputBox);
            this.mainPanel.Controls.Add(this.userListPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1000, 560);
            this.mainPanel.TabIndex = 3;
            // 
            // userListPanel
            // 
            this.userListPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(20)))), ((int)(((byte)(28)))));
            this.userListPanel.Controls.Add(this.userListBox);
            this.userListPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.userListPanel.Location = new System.Drawing.Point(840, 0);
            this.userListPanel.Name = "userListPanel";
            this.userListPanel.Size = new System.Drawing.Size(160, 560);
            this.userListPanel.TabIndex = 2;
            // 
            // inputPanel
            // 
            this.inputPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(35)))), ((int)(((byte)(46)))));
            this.inputPanel.Controls.Add(this.inputBox);
            this.inputPanel.Controls.Add(this.sendButton);
            this.inputPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inputPanel.Location = new System.Drawing.Point(0, 560);
            this.inputPanel.Margin = new System.Windows.Forms.Padding(0);
            this.inputPanel.Name = "inputPanel";
            this.inputPanel.Padding = new System.Windows.Forms.Padding(5);
            this.inputPanel.Size = new System.Drawing.Size(1000, 40);
            this.inputPanel.TabIndex = 4;
            // 
            // ChannelTab
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.inputPanel);
            this.Font = new System.Drawing.Font("Cascadia Mono", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ChannelTab";
            this.Size = new System.Drawing.Size(1000, 600);
            this.mainPanel.ResumeLayout(false);
            this.userListPanel.ResumeLayout(false);
            this.inputPanel.ResumeLayout(false);
            this.inputPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
