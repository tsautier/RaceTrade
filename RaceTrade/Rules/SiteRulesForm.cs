using System;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class SiteRulesForm : Form
    {
        public SiteRulesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the rules text to display.
        /// </summary>
        public void SetRulesText(string rules)
        {
            rulesRichTextBox.Clear();
            rulesRichTextBox.Text = rules ?? string.Empty;
            rulesRichTextBox.SelectionStart = 0;
            rulesRichTextBox.SelectionLength = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
