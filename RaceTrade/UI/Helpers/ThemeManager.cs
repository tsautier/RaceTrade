using System;
using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    /// <summary>
    /// Provides centralized theming and styling for the entire application.
    /// Ensures consistent look and feel across all forms and controls.
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// Color palette for the dark theme.
        /// </summary>
        public static class Colors
        {
            // Backgrounds
            public static readonly Color Background = Color.FromArgb(45, 45, 48);
            public static readonly Color BackgroundDark = Color.FromArgb(30, 30, 30);
            public static readonly Color BackgroundLight = Color.FromArgb(62, 62, 66);

            // Text
            public static readonly Color Foreground = Color.White;
            public static readonly Color ForegroundDisabled = Color.FromArgb(128, 128, 128);

            // Borders
            public static readonly Color Border = Color.FromArgb(63, 63, 70);
            public static readonly Color BorderHighlight = Color.FromArgb(0, 122, 204);

            // Buttons
            public static readonly Color ButtonBackground = Color.FromArgb(62, 62, 66);
            public static readonly Color ButtonHover = Color.FromArgb(82, 82, 86);
            public static readonly Color ButtonPressed = Color.FromArgb(72, 72, 76);
            public static readonly Color ButtonPrimary = Color.FromArgb(0, 122, 204);
            public static readonly Color ButtonPrimaryHover = Color.FromArgb(28, 151, 234);

            // Status colors
            public static readonly Color Success = Color.FromArgb(16, 124, 16);
            public static readonly Color SuccessLight = Color.LimeGreen;
            public static readonly Color Danger = Color.FromArgb(232, 17, 35);
            public static readonly Color DangerLight = Color.FromArgb(255, 69, 58);
            public static readonly Color Warning = Color.FromArgb(255, 185, 0);
            public static readonly Color WarningLight = Color.Orange;
            public static readonly Color Info = Color.FromArgb(0, 120, 215);
            public static readonly Color InfoLight = Color.Cyan;

            // Highlights
            public static readonly Color Selection = Color.FromArgb(51, 153, 255);
            public static readonly Color Hover = Color.FromArgb(82, 82, 86);
        }

        /// <summary>
        /// Common fonts used throughout the application.
        /// </summary>
        public static class Fonts
        {
            public static readonly Font Default = new Font("Segoe UI", 9F);
            public static readonly Font DefaultBold = new Font("Segoe UI", 9F, FontStyle.Bold);
            public static readonly Font Large = new Font("Segoe UI", 11F);
            public static readonly Font LargeBold = new Font("Segoe UI", 11F, FontStyle.Bold);
            public static readonly Font Header = new Font("Segoe UI", 14F, FontStyle.Bold);
            public static readonly Font Monospace = new Font("Consolas", 9F);
            public static readonly Font MonospaceLarge = new Font("Consolas", 10F);
        }

        /// <summary>
        /// Applies the dark theme to an entire form and all its controls.
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            form.BackColor = Colors.Background;
            form.ForeColor = Colors.Foreground;

            foreach (Control control in form.Controls)
            {
                ApplyThemeToControl(control);
            }
        }

        /// <summary>
        /// Applies the theme to a control and all its children recursively.
        /// </summary>
        private static void ApplyThemeToControl(Control control)
        {
            // Skip if control is disposed
            if (control.IsDisposed) return;

            switch (control)
            {
                case Button button:
                    StyleButton(button);
                    break;

                case TextBox textBox:
                    StyleTextBox(textBox);
                    break;

                case ComboBox comboBox:
                    StyleComboBox(comboBox);
                    break;

                case CheckBox checkBox:
                    StyleCheckBox(checkBox);
                    break;

                case RadioButton radioButton:
                    StyleRadioButton(radioButton);
                    break;

                case Label label:
                    StyleLabel(label);
                    break;

                case GroupBox groupBox:
                    StyleGroupBox(groupBox);
                    break;

                case Panel panel:
                    StylePanel(panel);
                    break;

                case ListBox listBox:
                    StyleListBox(listBox);
                    break;

                case DataGridView dataGridView:
                    StyleDataGridView(dataGridView);
                    break;

                case TabControl tabControl:
                    StyleTabControl(tabControl);
                    break;

                case RichTextBox richTextBox:
                    StyleRichTextBox(richTextBox);
                    break;
            }

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child);
            }
        }

        private static void StyleButton(Button button)
        {
            button.BackColor = Colors.ButtonBackground;
            button.ForeColor = Colors.Foreground;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Colors.Border;
            button.FlatAppearance.BorderSize = 1;
            button.Font = Fonts.Default;

            // Add hover effect
            button.MouseEnter += (s, e) => button.BackColor = Colors.ButtonHover;
            button.MouseLeave += (s, e) => button.BackColor = Colors.ButtonBackground;
            button.MouseDown += (s, e) => button.BackColor = Colors.ButtonPressed;
            button.MouseUp += (s, e) => button.BackColor = Colors.ButtonHover;
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Colors.BackgroundDark;
            textBox.ForeColor = Colors.Foreground;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = Fonts.Default;
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Colors.BackgroundDark;
            comboBox.ForeColor = Colors.Foreground;
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.Font = Fonts.Default;
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.ForeColor = Colors.Foreground;
            checkBox.Font = Fonts.Default;
        }

        private static void StyleRadioButton(RadioButton radioButton)
        {
            radioButton.ForeColor = Colors.Foreground;
            radioButton.Font = Fonts.Default;
        }

        private static void StyleLabel(Label label)
        {
            label.ForeColor = Colors.Foreground;
            label.Font = Fonts.Default;
        }

        private static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = Colors.Foreground;
            groupBox.Font = Fonts.DefaultBold;
        }

        private static void StylePanel(Panel panel)
        {
            // Only style if not already customized
            if (panel.BackColor == SystemColors.Control)
            {
                panel.BackColor = Colors.Background;
            }
        }

        private static void StyleListBox(ListBox listBox)
        {
            listBox.BackColor = Colors.BackgroundDark;
            listBox.ForeColor = Colors.Foreground;
            listBox.BorderStyle = BorderStyle.FixedSingle;
            listBox.Font = Fonts.Default;
        }

        private static void StyleDataGridView(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = Colors.BackgroundDark;
            dataGridView.ForeColor = Colors.Foreground;
            dataGridView.GridColor = Colors.Border;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Colors.BackgroundLight;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = Colors.BackgroundLight;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.DefaultCellStyle.BackColor = Colors.BackgroundDark;
            dataGridView.DefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.DefaultCellStyle.SelectionBackColor = Colors.Selection;
            dataGridView.DefaultCellStyle.SelectionForeColor = Colors.Foreground;
            dataGridView.Font = Fonts.Default;
        }

        private static void StyleTabControl(TabControl tabControl)
        {
            tabControl.Font = Fonts.Default;
            // TabControl styling is limited in WinForms without custom painting
        }

        private static void StyleRichTextBox(RichTextBox richTextBox)
        {
            richTextBox.BackColor = Colors.BackgroundDark;
            richTextBox.ForeColor = Colors.Foreground;
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
            richTextBox.Font = Fonts.Monospace;
        }

        /// <summary>
        /// Styles a button as a primary action button (blue/highlighted).
        /// </summary>
        public static void StylePrimaryButton(Button button)
        {
            button.BackColor = Colors.ButtonPrimary;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = Fonts.DefaultBold;

            button.MouseEnter += (s, e) => button.BackColor = Colors.ButtonPrimaryHover;
            button.MouseLeave += (s, e) => button.BackColor = Colors.ButtonPrimary;
        }

        /// <summary>
        /// Styles a button as a danger button (red, for deletions).
        /// </summary>
        public static void StyleDangerButton(Button button)
        {
            button.BackColor = Colors.Danger;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = Fonts.DefaultBold;

            button.MouseEnter += (s, e) => button.BackColor = Colors.DangerLight;
            button.MouseLeave += (s, e) => button.BackColor = Colors.Danger;
        }

        /// <summary>
        /// Styles a button as a success button (green).
        /// </summary>
        public static void StyleSuccessButton(Button button)
        {
            button.BackColor = Colors.Success;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = Fonts.DefaultBold;

            button.MouseEnter += (s, e) => button.BackColor = Colors.SuccessLight;
            button.MouseLeave += (s, e) => button.BackColor = Colors.Success;
        }

        /// <summary>
        /// Creates a styled section header label.
        /// </summary>
        public static Label CreateHeaderLabel(string text, int x, int y, int width = 400)
        {
            return new Label
            {
                Text = text,
                Font = Fonts.Header,
                ForeColor = Colors.Foreground,
                Location = new Point(x, y),
                Size = new Size(width, 30),
                AutoSize = false
            };
        }

        /// <summary>
        /// Creates a styled section divider (horizontal line).
        /// </summary>
        public static Panel CreateDivider(int x, int y, int width)
        {
            return new Panel
            {
                BackColor = Colors.Border,
                Location = new Point(x, y),
                Size = new Size(width, 1)
            };
        }

        /// <summary>
        /// Creates a tooltip with consistent styling.
        /// </summary>
        public static ToolTip CreateTooltip()
        {
            return new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100,
                ShowAlways = true,
                BackColor = Colors.BackgroundLight,
                ForeColor = Colors.Foreground
            };
        }
    }
}