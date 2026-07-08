using System.Windows.Forms;

namespace RaceTrade
{
    partial class AddCbftp
    {
        private System.ComponentModel.IContainer components = null;

        // Dispose of resources used by the form
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.DistProfile_CheckBox = new System.Windows.Forms.CheckBox();
            this.RaceProfile_CheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.delete_cbftp_server_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_ServerName = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(91, 197);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 25);
            this.button3.TabIndex = 4;
            this.button3.Text = "Save";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(352, 197);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 25);
            this.button4.TabIndex = 5;
            this.button4.Text = "Exit";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.textBox_ServerName);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.DistProfile_CheckBox);
            this.groupBox2.Controls.Add(this.RaceProfile_CheckBox);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBox2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Location = new System.Drawing.Point(10, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 179);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CBFTP Server";
            // 
            // DistProfile_CheckBox
            // 
            this.DistProfile_CheckBox.AutoSize = true;
            this.DistProfile_CheckBox.Location = new System.Drawing.Point(237, 139);
            this.DistProfile_CheckBox.Name = "DistProfile_CheckBox";
            this.DistProfile_CheckBox.Size = new System.Drawing.Size(174, 21);
            this.DistProfile_CheckBox.TabIndex = 10;
            this.DistProfile_CheckBox.Text = "Distribute Profile";
            this.DistProfile_CheckBox.UseVisualStyleBackColor = true;
            // 
            // RaceProfile_CheckBox
            // 
            this.RaceProfile_CheckBox.AutoSize = true;
            this.RaceProfile_CheckBox.Location = new System.Drawing.Point(91, 139);
            this.RaceProfile_CheckBox.Name = "RaceProfile_CheckBox";
            this.RaceProfile_CheckBox.Size = new System.Drawing.Size(126, 21);
            this.RaceProfile_CheckBox.TabIndex = 9;
            this.RaceProfile_CheckBox.Text = "Race Profile";
            this.RaceProfile_CheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Profile:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 17);
            this.label4.TabIndex = 7;
            this.label4.Text = "Password:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(91, 110);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(318, 24);
            this.textBox4.TabIndex = 6;
            this.textBox4.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Port:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(91, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(318, 24);
            this.textBox2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(91, 50);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(318, 24);
            this.textBox1.TabIndex = 0;
            // 
            // delete_cbftp_server_button
            // 
            this.delete_cbftp_server_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.delete_cbftp_server_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.delete_cbftp_server_button.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.delete_cbftp_server_button.Location = new System.Drawing.Point(10, 197);
            this.delete_cbftp_server_button.Name = "delete_cbftp_server_button";
            this.delete_cbftp_server_button.Size = new System.Drawing.Size(75, 25);
            this.delete_cbftp_server_button.TabIndex = 8;
            this.delete_cbftp_server_button.Text = "Delete";
            this.delete_cbftp_server_button.UseVisualStyleBackColor = false;
            this.delete_cbftp_server_button.Click += new System.EventHandler(this.delete_cbftp_server_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Name";
            // 
            // textBox_ServerName
            // 
            this.textBox_ServerName.Location = new System.Drawing.Point(91, 20);
            this.textBox_ServerName.Name = "textBox_ServerName";
            this.textBox_ServerName.Size = new System.Drawing.Size(318, 24);
            this.textBox_ServerName.TabIndex = 12;
            // 
            // AddCbftp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(437, 234);
            this.Controls.Add(this.delete_cbftp_server_button);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Font = new System.Drawing.Font("Cascadia Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddCbftp";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add/Edit Cbftp server";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private Button button3;
        private Button button4;
        private GroupBox groupBox2;
        private Label label4;
        private TextBox textBox4;
        private Label label2;
        private TextBox textBox2;
        private Label label1;
        private TextBox textBox1;
        private Button delete_cbftp_server_button;
        public CheckBox DistProfile_CheckBox;
        public CheckBox RaceProfile_CheckBox;
        private Label label3;
        private Label label5;
        private TextBox textBox_ServerName;
    }
}
