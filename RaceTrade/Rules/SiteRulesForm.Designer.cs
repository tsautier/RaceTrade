using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    partial class SiteRulesForm
    {
        private System.ComponentModel.IContainer components = null;
        private RichTextBox rulesRichTextBox;
        private Button btnClose;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rulesRichTextBox = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SiteRulesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ClientSize = new System.Drawing.Size(700, 450);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = Color.White;
            this.Name = "SiteRulesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Site Rules";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.MinimumSize = new Size(500, 300);
            // 
            // rulesRichTextBox
            // 
            this.rulesRichTextBox.BackColor = Color.FromArgb(25, 25, 25);
            this.rulesRichTextBox.ForeColor = Color.White;
            this.rulesRichTextBox.BorderStyle = BorderStyle.None;
            this.rulesRichTextBox.Font = new Font("Consolas", 9F);
            this.rulesRichTextBox.Location = new Point(10, 10);
            this.rulesRichTextBox.Name = "rulesRichTextBox";
            this.rulesRichTextBox.ReadOnly = true;
            this.rulesRichTextBox.WordWrap = false;
            this.rulesRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.rulesRichTextBox.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 60);
            this.rulesRichTextBox.TabIndex = 0;
            this.rulesRichTextBox.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Text = "Close";
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(80, 28);
            this.btnClose.Location = new Point(this.ClientSize.Width - 90, this.ClientSize.Height - 36);
            this.btnClose.BackColor = Color.FromArgb(62, 62, 66);
            this.btnClose.ForeColor = Color.White;
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.FlatAppearance.BorderColor = Color.FromArgb(63, 63, 70);
            this.btnClose.TabIndex = 1;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // 
            // add controls
            // 
            this.Controls.Add(this.rulesRichTextBox);
            this.Controls.Add(this.btnClose);

            // handle resize to keep layout clean
            this.Resize += (s, e) =>
            {
                if (rulesRichTextBox != null)
                {
                    rulesRichTextBox.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 60);
                }
                if (btnClose != null)
                {
                    btnClose.Location = new Point(this.ClientSize.Width - 90, this.ClientSize.Height - 36);
                }
            };

            this.ResumeLayout(false);
        }

        #endregion
    }
}
