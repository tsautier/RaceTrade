using System;
using System.Windows.Forms;
using RaceTrade;

namespace RaceTrader
{
    public partial class GlobalBlacklistForm : Form
    {
        private MainApp mainApp;

        // Parameterless constructor so Visual Studio Designer can instantiate the form.
        public GlobalBlacklistForm()
        {
            InitializeComponent();

            // Do NOT call mainApp-related logic here.
            // Designer will use this constructor.
        }

        // Runtime constructor
        public GlobalBlacklistForm(MainApp parentForm) : this()
        {
            mainApp = parentForm;

            // Only run app logic when we have the dependency
            if (mainApp != null)
            {
                LoadPatterns();
                enabledCheckBox.Checked = mainApp.IsGlobalBlacklistEnabled();
            }
        }

        private void LoadPatterns()
        {
            try
            {
                patternsListBox.Items.Clear();

                if (mainApp == null)
                    return;

                var patterns = mainApp.GetGlobalBlacklist();

                if (patterns != null && patterns.Count > 0)
                {
                    foreach (var pattern in patterns)
                        patternsListBox.Items.Add(pattern);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading blacklist patterns: {ex.Message}");
                MessageBox.Show(
                    $"Error loading patterns:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var pattern = patternTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(pattern))
            {
                MessageBox.Show(
                    "Please enter a pattern.",
                    "Empty Pattern",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (mainApp == null)
            {
                // designer safety / mis-use safety
                patternsListBox.Items.Add(pattern);
                patternTextBox.Clear();
                return;
            }

            if (mainApp.AddGlobalBlacklistPattern(pattern))
            {
                patternsListBox.Items.Add(pattern);
                patternTextBox.Clear();
                patternTextBox.Focus();
                LogManager.Success($"Added blacklist pattern: {pattern}");
            }
            else
            {
                MessageBox.Show(
                    "Pattern already exists in the blacklist.",
                    "Duplicate Pattern",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (patternsListBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a pattern to remove.",
                    "No Selection",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var pattern = patternsListBox.SelectedItem.ToString();

            var result = MessageBox.Show(
                $"Remove pattern '{pattern}' from blacklist?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            if (mainApp == null)
            {
                patternsListBox.Items.Remove(patternsListBox.SelectedItem);
                return;
            }

            if (mainApp.RemoveGlobalBlacklistPattern(pattern))
            {
                patternsListBox.Items.Remove(patternsListBox.SelectedItem);
                LogManager.Success($"Removed blacklist pattern: {pattern}");
            }
            else
            {
                MessageBox.Show(
                    "Failed to remove pattern.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (patternsListBox.Items.Count == 0)
            {
                MessageBox.Show(
                    "Blacklist is already empty.",
                    "Nothing to Clear",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Remove all {patternsListBox.Items.Count} pattern(s) from the blacklist?",
                "Confirm Clear All",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            if (mainApp != null)
                mainApp.ClearGlobalBlacklist();

            patternsListBox.Items.Clear();
            LogManager.Success("Cleared all blacklist patterns");
        }

        private void enabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (mainApp == null)
                    return;

                mainApp.SetGlobalBlacklistEnabled(enabledCheckBox.Checked);

                if (enabledCheckBox.Checked)
                    LogManager.Success("Global blacklist enabled");
                else
                    LogManager.Warning("Global blacklist disabled");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error changing blacklist state: {ex.Message}");
                MessageBox.Show(
                    $"Error:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void patternTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                addButton_Click(sender, e);
            }
        }
    }
}
