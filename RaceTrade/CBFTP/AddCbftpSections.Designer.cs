namespace RaceTrade
{
    partial class AddCbftpSection
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
            this.List_cbftp_sections = new System.Windows.Forms.ListBox();
            this.Delete_cbftp_Section = new System.Windows.Forms.Button();
            this.Add_cbftp_Section = new System.Windows.Forms.Button();
            this.cbftp_Section_name_field = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // List_cbftp_sections
            // 
            this.List_cbftp_sections.FormattingEnabled = true;
            this.List_cbftp_sections.ItemHeight = 17;
            this.List_cbftp_sections.Location = new System.Drawing.Point(12, 12);
            this.List_cbftp_sections.Name = "List_cbftp_sections";
            this.List_cbftp_sections.Size = new System.Drawing.Size(160, 276);
            this.List_cbftp_sections.TabIndex = 8;
            // 
            // Delete_cbftp_Section
            // 
            this.Delete_cbftp_Section.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(75)))), ((int)(((byte)(76)))));
            this.Delete_cbftp_Section.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Delete_cbftp_Section.Location = new System.Drawing.Point(12, 324);
            this.Delete_cbftp_Section.Name = "Delete_cbftp_Section";
            this.Delete_cbftp_Section.Size = new System.Drawing.Size(82, 25);
            this.Delete_cbftp_Section.TabIndex = 7;
            this.Delete_cbftp_Section.Text = "Delete";
            this.Delete_cbftp_Section.UseVisualStyleBackColor = false;
            this.Delete_cbftp_Section.Click += new System.EventHandler(this.Delete_cbftp_Section_Click);
            // 
            // Add_cbftp_Section
            // 
            this.Add_cbftp_Section.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(166)))), ((int)(((byte)(112)))));
            this.Add_cbftp_Section.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Add_cbftp_Section.Location = new System.Drawing.Point(99, 324);
            this.Add_cbftp_Section.Name = "Add_cbftp_Section";
            this.Add_cbftp_Section.Size = new System.Drawing.Size(73, 25);
            this.Add_cbftp_Section.TabIndex = 6;
            this.Add_cbftp_Section.Text = "Add";
            this.Add_cbftp_Section.UseVisualStyleBackColor = false;
            this.Add_cbftp_Section.Click += new System.EventHandler(this.Add_Section_cbftp_Click);
            // 
            // cbftp_Section_name_field
            // 
            this.cbftp_Section_name_field.Location = new System.Drawing.Point(12, 294);
            this.cbftp_Section_name_field.Name = "cbftp_Section_name_field";
            this.cbftp_Section_name_field.Size = new System.Drawing.Size(160, 24);
            this.cbftp_Section_name_field.TabIndex = 5;
            // 
            // AddCbftpSection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(185, 359);
            this.Controls.Add(this.List_cbftp_sections);
            this.Controls.Add(this.Delete_cbftp_Section);
            this.Controls.Add(this.Add_cbftp_Section);
            this.Controls.Add(this.cbftp_Section_name_field);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddCbftpSection";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add Cbftp Sections";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox List_cbftp_sections;
        private System.Windows.Forms.Button Delete_cbftp_Section;
        private System.Windows.Forms.Button Add_cbftp_Section;
        private System.Windows.Forms.TextBox cbftp_Section_name_field;
    }
}