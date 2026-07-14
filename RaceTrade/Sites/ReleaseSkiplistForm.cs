using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RaceTrade
{
    public sealed class ReleaseSkiplistForm : AntdUI.Window
    {
        private readonly ListBox patternListBox;
        private readonly TextBox patternTextBox;
        private readonly Button addButton;
        private readonly Button removeButton;
        private readonly Button saveButton;
        private readonly Button closeButton;

        public List<string> Patterns { get; private set; }

        public ReleaseSkiplistForm(string sectionName, IEnumerable<string> patterns)
        {
            Patterns = patterns?
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList() ?? new List<string>();

            Text = $"Release Skiplist - {sectionName}";
            Size = new Size(560, 430);
            MinimumSize = new Size(520, 360);
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;
            MinimizeBox = false;
            MaximizeBox = false;
            Dark = true;
            Mode = AntdUI.TAMode.Dark;
            BackColor = ThemeManager.Colors.Background;
            ForeColor = ThemeManager.Colors.Foreground;
            Font = ThemeManager.Fonts.Default;

            var titleLabel = new Label
            {
                Text = sectionName,
                AutoSize = false,
                Location = new Point(16, 14),
                Size = new Size(512, 24),
                ForeColor = ThemeManager.Colors.AccentCyan,
                BackColor = Color.Transparent,
                Font = new Font(ThemeManager.Fonts.UiFamily, 10.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(titleLabel);

            var hintLabel = new Label
            {
                Text = "Checks the announce release name before racing. Case-insensitive.\r\n" +
                       "Plain text contains-match: FRENCH   Wildcards: *FRENCH* or TV-?080P*\r\n" +
                       "This is not the CBFTP file/dir skiplist.",
                AutoSize = false,
                Location = new Point(16, 38),
                Size = new Size(512, 50),
                ForeColor = ThemeManager.Colors.ForegroundMuted,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(hintLabel);

            patternListBox = new ListBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(16, 96),
                Size = new Size(512, 180),
                BackColor = ThemeManager.Colors.BackgroundDark,
                ForeColor = ThemeManager.Colors.Foreground,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false
            };
            patternListBox.DoubleClick += PatternListBox_DoubleClick;
            Controls.Add(patternListBox);

            var patternLabel = new Label
            {
                Text = "Pattern:",
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                AutoSize = true,
                Location = new Point(16, 292),
                ForeColor = ThemeManager.Colors.Foreground,
                BackColor = Color.Transparent
            };
            Controls.Add(patternLabel);

            patternTextBox = new TextBox
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(86, 288),
                Size = new Size(276, 24),
                BackColor = ThemeManager.Colors.BackgroundDark,
                ForeColor = ThemeManager.Colors.Foreground,
                BorderStyle = BorderStyle.FixedSingle
            };
            patternTextBox.KeyDown += PatternTextBox_KeyDown;
            Controls.Add(patternTextBox);

            addButton = CreateButton("Add", 374, 286, 70, 28);
            addButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            addButton.Click += AddButton_Click;
            Controls.Add(addButton);

            removeButton = CreateButton("Remove", 452, 286, 76, 28);
            removeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            removeButton.Click += RemoveButton_Click;
            Controls.Add(removeButton);

            saveButton = CreateButton("Save", 330, 342, 96, 32);
            saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            saveButton.Click += SaveButton_Click;
            Controls.Add(saveButton);

            closeButton = CreateButton("Close", 432, 342, 96, 32);
            closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            closeButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            Controls.Add(closeButton);

            AcceptButton = addButton;
            CancelButton = closeButton;

            LoadPatterns();
        }

        private static Button CreateButton(string text, int x, int y, int width, int height)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                UseVisualStyleBackColor = false
            };
            ThemeManager.StyleActionButton(button, 0);
            return button;
        }

        private void LoadPatterns()
        {
            patternListBox.Items.Clear();
            foreach (var pattern in Patterns)
                patternListBox.Items.Add(pattern);
        }

        private void PatternTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddPatternFromTextBox();
            }
        }

        private void PatternListBox_DoubleClick(object sender, EventArgs e)
        {
            if (patternListBox.SelectedItem != null)
            {
                patternTextBox.Text = patternListBox.SelectedItem.ToString();
                patternTextBox.Focus();
                patternTextBox.SelectAll();
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddPatternFromTextBox();
        }

        private void AddPatternFromTextBox()
        {
            var pattern = patternTextBox.Text.Trim();
            if (pattern.Length == 0)
                return;

            for (int i = 0; i < patternListBox.Items.Count; i++)
            {
                if (string.Equals(patternListBox.Items[i].ToString(), pattern, StringComparison.OrdinalIgnoreCase))
                {
                    patternListBox.SelectedIndex = i;
                    patternTextBox.SelectAll();
                    return;
                }
            }

            patternListBox.Items.Add(pattern);
            patternListBox.SelectedIndex = patternListBox.Items.Count - 1;
            patternTextBox.Clear();
            patternTextBox.Focus();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            var index = patternListBox.SelectedIndex;
            if (index < 0)
                return;

            patternListBox.Items.RemoveAt(index);
            if (patternListBox.Items.Count > 0)
                patternListBox.SelectedIndex = Math.Min(index, patternListBox.Items.Count - 1);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Patterns = patternListBox.Items
                .Cast<object>()
                .Select(item => item.ToString().Trim())
                .Where(item => item.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
