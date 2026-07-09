using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RaceTrade
{
    /// <summary>
    /// Provides centralized theming and styling for the entire application.
    /// Ensures consistent look and feel across all forms and controls.
    ///
    /// "Nebula" — a futuristic deep-space dark theme: cool navy/slate surfaces
    /// layered for depth, an electric-azure accent and a cyan highlight, with an
    /// Orbitron display font for titles/headers (falls back to Bahnschrift then
    /// Segoe UI when Orbitron isn't installed) and Cascadia Mono for logs, chat
    /// and FTP listings.
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// Color palette for the dark theme.
        /// Surfaces keep a consistent luminance ladder (Surface3 lightest ->
        /// BackgroundDarkest deepest) so panels, inputs and headers read as depth.
        /// </summary>
        public static class Colors
        {
            // Backgrounds (cool blue-charcoal, layered light -> dark)
            public static readonly Color Surface3 = Color.FromArgb(37, 42, 54);   // elevated panels / headers
            public static readonly Color Surface2 = Color.FromArgb(33, 38, 50);   // secondary surface
            public static readonly Color Background = Color.FromArgb(22, 26, 36);  // primary canvas
            public static readonly Color BackgroundLight = Color.FromArgb(37, 42, 54);
            public static readonly Color BackgroundDark = Color.FromArgb(13, 16, 24);   // inputs / wells
            public static readonly Color BackgroundDarkest = Color.FromArgb(9, 11, 17);

            // Text
            public static readonly Color Foreground = Color.FromArgb(228, 233, 242); // soft white, easier on the eyes
            public static readonly Color ForegroundMuted = Color.FromArgb(150, 160, 178);
            public static readonly Color ForegroundDisabled = Color.FromArgb(96, 104, 120);

            // Borders
            public static readonly Color Border = Color.FromArgb(42, 48, 62);
            public static readonly Color BorderHighlight = Color.FromArgb(0, 168, 255);

            // Buttons
            public static readonly Color ButtonBackground = Color.FromArgb(40, 46, 60);
            public static readonly Color ButtonHover = Color.FromArgb(52, 60, 78);
            public static readonly Color ButtonPressed = Color.FromArgb(34, 40, 52);
            public static readonly Color ButtonPrimary = Color.FromArgb(0, 168, 255);     // electric azure
            public static readonly Color ButtonPrimaryHover = Color.FromArgb(56, 195, 255);

            // Accents
            public static readonly Color Accent = Color.FromArgb(0, 168, 255);
            public static readonly Color AccentCyan = Color.FromArgb(0, 229, 214);
            public static readonly Color AccentViolet = Color.FromArgb(140, 120, 255);

            // Status colors (brightened for contrast on deep backgrounds)
            public static readonly Color Success = Color.FromArgb(34, 197, 120);
            public static readonly Color SuccessLight = Color.FromArgb(74, 222, 150);
            public static readonly Color Danger = Color.FromArgb(255, 78, 90);
            public static readonly Color DangerLight = Color.FromArgb(255, 110, 120);
            public static readonly Color Warning = Color.FromArgb(255, 196, 0);
            public static readonly Color WarningLight = Color.FromArgb(255, 176, 32);
            public static readonly Color Info = Color.FromArgb(0, 168, 255);
            public static readonly Color InfoLight = Color.FromArgb(0, 229, 214);

            // Highlights
            public static readonly Color Selection = Color.FromArgb(64, 180, 255);
            public static readonly Color Hover = Color.FromArgb(52, 60, 78);

            // Toggle/state indicators (light text reads well on both)
            public static readonly Color ToggleOn = Color.FromArgb(0, 120, 200);   // active deep azure
            public static readonly Color ToggleOff = Color.FromArgb(40, 46, 60);   // inactive (matches button)

            // Gradient stops for header bands (deep azure -> violet over charcoal)
            public static readonly Color GradientStart = Color.FromArgb(20, 28, 46);
            public static readonly Color GradientMid = Color.FromArgb(28, 40, 72);
            public static readonly Color GradientEnd = Color.FromArgb(18, 22, 34);
        }

        // Holds any privately-loaded font families (e.g. a bundled Orbitron .ttf)
        // so they survive for the lifetime of the app.
        private static readonly PrivateFontCollection _privateFonts = new PrivateFontCollection();
        private static readonly HashSet<string> _privateFamilyNames =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private enum ButtonTone
        {
            Neutral,
            Primary,
            Success,
            Danger,
            Warning
        }

        static ThemeManager()
        {
            TryLoadBundledFonts();
        }

        /// <summary>
        /// Loads any .ttf/.otf files found in a "Fonts" folder next to the
        /// executable into a private font collection. This lets us ship Orbitron
        /// (or any display font) with the app without requiring a system install.
        /// Harmless if the folder or files don't exist.
        /// </summary>
        private static void TryLoadBundledFonts()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                foreach (var dir in new[] { Path.Combine(baseDir, "Fonts"), Path.Combine(baseDir, "Resources", "Fonts") })
                {
                    if (!Directory.Exists(dir)) continue;
                    foreach (var file in Directory.GetFiles(dir))
                    {
                        var ext = Path.GetExtension(file).ToLowerInvariant();
                        if (ext != ".ttf" && ext != ".otf") continue;
                        try { _privateFonts.AddFontFile(file); }
                        catch { /* skip unreadable font */ }
                    }
                }
                foreach (var fam in _privateFonts.Families)
                    _privateFamilyNames.Add(fam.Name);
            }
            catch
            {
                // Never let font loading break startup.
            }
        }

        /// <summary>
        /// Resolves the first available font family from a preference list,
        /// checking privately-bundled fonts first, then system-installed fonts,
        /// finally falling back to a guaranteed-present family.
        /// </summary>
        private static string ResolveFamily(params string[] preferences)
        {
            try
            {
                using (var installed = new InstalledFontCollection())
                {
                    var names = installed.Families;
                    foreach (var pref in preferences)
                    {
                        if (_privateFamilyNames.Contains(pref))
                            return pref;
                        foreach (var fam in names)
                        {
                            if (string.Equals(fam.Name, pref, StringComparison.OrdinalIgnoreCase))
                                return pref;
                        }
                    }
                }
            }
            catch
            {
                // Ignore and fall through to the last preference.
            }
            return preferences.Length > 0 ? preferences[preferences.Length - 1] : "Segoe UI";
        }

        /// <summary>
        /// Builds a Font for a resolved family, using the private collection when
        /// the family was bundled rather than installed system-wide.
        /// </summary>
        private static Font MakeFont(string family, float size, FontStyle style)
        {
            try
            {
                if (_privateFamilyNames.Contains(family))
                {
                    foreach (var fam in _privateFonts.Families)
                    {
                        if (string.Equals(fam.Name, family, StringComparison.OrdinalIgnoreCase))
                            return new Font(fam, size, style);
                    }
                }
            }
            catch { }
            return new Font(family, size, style);
        }

        /// <summary>
        /// Common fonts used throughout the application. Sizes match the previous
        /// theme so existing fixed layouts are preserved; only the families and
        /// rendering are upgraded.
        /// </summary>
        public static class Fonts
        {
            // Body / UI font: modern Windows UI font, classic Segoe UI as fallback.
            public static readonly string UiFamily = ResolveFamily("Segoe UI Variable Text", "Segoe UI");
            // Futuristic display font for titles/headers. Orbitron if present,
            // otherwise Bahnschrift (ships with Win10/11, geometric/techy),
            // finally Segoe UI Semibold.
            public static readonly string DisplayFamily = ResolveFamily("Orbitron", "Bahnschrift SemiBold", "Bahnschrift", "Segoe UI Semibold", "Segoe UI");
            // Sleek terminal font for logs / chat / FTP listings.
            public static readonly string MonoFamily = ResolveFamily("Cascadia Mono", "Cascadia Code", "Consolas");
            // Icon glyph font (ships with Windows 10/11).
            public static readonly string IconFamily = ResolveFamily("Segoe Fluent Icons", "Segoe MDL2 Assets", "Segoe UI Symbol");

            public static readonly Font Default = MakeFont(UiFamily, 9F, FontStyle.Regular);
            public static readonly Font DefaultBold = MakeFont(UiFamily, 9F, FontStyle.Bold);
            public static readonly Font Large = MakeFont(UiFamily, 11F, FontStyle.Regular);
            public static readonly Font LargeBold = MakeFont(UiFamily, 11F, FontStyle.Bold);
            // Headers/titles use the display font.
            public static readonly Font Header = MakeFont(DisplayFamily, 14F, FontStyle.Bold);
            public static readonly Font Title = MakeFont(DisplayFamily, 18F, FontStyle.Bold);
            public static readonly Font Subtitle = MakeFont(DisplayFamily, 9.5F, FontStyle.Regular);
            public static readonly Font Monospace = MakeFont(MonoFamily, 9F, FontStyle.Regular);
            public static readonly Font MonospaceLarge = MakeFont(MonoFamily, 10F, FontStyle.Regular);
        }

        #region Win32 dark title bar (DWM immersive dark mode)

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCAPTION = 2;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Asks the desktop window manager to paint this form's title bar dark
        /// (Windows 10 1809+ / Windows 11). No-op / harmless on older systems.
        /// </summary>
        public static void EnableDarkTitleBar(Form form)
        {
            if (form == null) return;
            void Apply()
            {
                try
                {
                    int useDark = 1;
                    // 20 = DWMWA_USE_IMMERSIVE_DARK_MODE (1903+); 19 = older builds.
                    if (DwmSetWindowAttribute(form.Handle, 20, ref useDark, sizeof(int)) != 0)
                        DwmSetWindowAttribute(form.Handle, 19, ref useDark, sizeof(int));
                }
                catch { }
            }

            if (form.IsHandleCreated) Apply();
            else form.HandleCreated += (s, e) => Apply();
        }

        #endregion

        /// <summary>
        /// Applies the dark theme to an entire form and all its controls, and
        /// requests a dark title bar.
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            if (form == null) return;
            if (form is AntdUI.Window antWindow)
            {
                antWindow.Dark = true;
                antWindow.Mode = AntdUI.TAMode.Dark;
                antWindow.EnableHitTest = true;
                antWindow.BorderColor = Colors.Border;
                EnableAntdWindowDrag(antWindow);
            }

            form.BackColor = Colors.Background;
            form.ForeColor = Colors.Foreground;
            EnableDarkTitleBar(form);

            foreach (Control control in form.Controls)
                ApplyThemeToControl(control);
        }

        private static void EnableAntdWindowDrag(Form form)
        {
            if (form == null) return;

            WireDragSurface(form, form);

            foreach (Control control in form.Controls)
                WireDragSurfaceRecursive(form, control);
        }

        private static void WireDragSurfaceRecursive(Form form, Control control)
        {
            if (control == null) return;

            if (IsDragSurface(control))
                WireDragSurface(form, control);

            foreach (Control child in control.Controls)
                WireDragSurfaceRecursive(form, child);
        }

        private static bool IsDragSurface(Control control)
        {
            if (control is LinkLabel || control.Cursor == Cursors.Hand)
                return false;

            return control is Panel ||
                   control is AntdUI.Panel ||
                   control is Label ||
                   control is GroupBox ||
                   control is TableLayoutPanel ||
                   control is FlowLayoutPanel;
        }

        private static void WireDragSurface(Form form, Control surface)
        {
            if (surface == null) return;

            surface.MouseDown -= AntdWindowDragSurface_MouseDown;
            surface.MouseDown += AntdWindowDragSurface_MouseDown;
        }

        private static void AntdWindowDragSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (!(sender is Control control)) return;

            Form form = control as Form ?? control.FindForm();
            if (form == null || form.WindowState == FormWindowState.Maximized) return;

            ReleaseCapture();
            SendMessage(form.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        }

        /// <summary>
        /// Overload for UserControls (and any Control container) so the same
        /// theming reaches embedded controls like ChannelTab.
        /// </summary>
        public static void ApplyTheme(Control control)
        {
            if (control == null) return;
            if (control is Form form) { ApplyTheme(form); return; }
            control.BackColor = Colors.Background;
            control.ForeColor = Colors.Foreground;
            ApplyThemeToControl(control);
        }

        /// <summary>
        /// Applies the theme to a control and all its children recursively.
        /// </summary>
        private static void ApplyThemeToControl(Control control)
        {
            if (control == null || control.IsDisposed) return;

            switch (control)
            {
                case Button button: StyleButton(button); break;
                case TextBox textBox: StyleTextBox(textBox); break;
                case ComboBox comboBox: StyleComboBox(comboBox); break;
                case CheckBox checkBox: StyleCheckBox(checkBox); break;
                case RadioButton radioButton: StyleRadioButton(radioButton); break;
                case LinkLabel linkLabel: StyleLinkLabel(linkLabel); break;
                case Label label: StyleLabel(label); break;
                case GroupBox groupBox: StyleGroupBox(groupBox); break;
                case ListBox listBox: StyleListBox(listBox); break;
                case ListView listView: StyleListView(listView); break;
                case TreeView treeView: StyleTreeView(treeView); break;
                case NumericUpDown numeric: StyleNumericUpDown(numeric); break;
                case DateTimePicker picker: StyleDateTimePicker(picker); break;
                case DataGridView dataGridView: StyleDataGridView(dataGridView); break;
                case TabControl tabControl: StyleTabControl(tabControl); break;
                case RichTextBox richTextBox: StyleRichTextBox(richTextBox); break;
                case ToolStrip toolStrip: StyleToolStrip(toolStrip); break;
                // Derived panels must precede the Panel case or they're unreachable.
                case TableLayoutPanel tlp: StyleContainer(tlp); break;
                case FlowLayoutPanel flp: StyleContainer(flp); break;
                case Panel panel: StylePanel(panel); break;
            }

            foreach (Control child in control.Controls)
                ApplyThemeToControl(child);
        }

        private static Color Blend(Color accent, Color surface, double amount)
        {
            amount = Math.Max(0, Math.Min(1, amount));
            int r = (int)Math.Round(surface.R + (accent.R - surface.R) * amount);
            int g = (int)Math.Round(surface.G + (accent.G - surface.G) * amount);
            int b = (int)Math.Round(surface.B + (accent.B - surface.B) * amount);
            return Color.FromArgb(r, g, b);
        }

        private static bool ContainsAny(string value, params string[] terms)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            foreach (var term in terms)
            {
                if (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private static ButtonTone ResolveButtonTone(Button button)
        {
            if (button?.Tag is string tag)
            {
                switch (tag.Trim().ToLowerInvariant())
                {
                    case "primary": return ButtonTone.Primary;
                    case "success": return ButtonTone.Success;
                    case "danger": return ButtonTone.Danger;
                    case "warning": return ButtonTone.Warning;
                    case "skip": return ButtonTone.Neutral;
                }
            }

            string key = $"{button?.Name} {button?.Text}";

            if (ContainsAny(key, "delete", "remove", "exit", "close", "cancel", "stop", "clear", "deselect", "unmap"))
                return ButtonTone.Danger;

            if (ContainsAny(key, "disable", "drop"))
                return ButtonTone.Warning;

            if (ContainsAny(key, "save", "add", "import", "sync", "start", "enable", "apply", "ok", "confirm", "create"))
                return ButtonTone.Success;

            if (ContainsAny(key, "edit", "map", "test", "refresh", "fetch", "send", "run", "get", "help", "view", "export", "select", "rules", "blacklist"))
                return ButtonTone.Primary;

            return ButtonTone.Neutral;
        }

        private static void ApplyButtonPalette(Button button, ButtonTone tone)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.UseVisualStyleBackColor = false;
            if (button.Font == null) button.Font = Fonts.Default;

            switch (tone)
            {
                case ButtonTone.Primary:
                    button.BackColor = Blend(Colors.Accent, Colors.ButtonBackground, 0.34);
                    button.ForeColor = Color.FromArgb(224, 244, 255);
                    button.FlatAppearance.BorderColor = Blend(Colors.AccentCyan, Colors.Border, 0.58);
                    button.FlatAppearance.MouseOverBackColor = Blend(Colors.Accent, Colors.ButtonHover, 0.52);
                    button.FlatAppearance.MouseDownBackColor = Blend(Colors.Accent, Colors.ButtonPressed, 0.42);
                    break;

                case ButtonTone.Success:
                    button.BackColor = Blend(Colors.Success, Colors.ButtonBackground, 0.36);
                    button.ForeColor = Color.FromArgb(232, 255, 244);
                    button.FlatAppearance.BorderColor = Blend(Colors.SuccessLight, Colors.Border, 0.58);
                    button.FlatAppearance.MouseOverBackColor = Blend(Colors.Success, Colors.ButtonHover, 0.52);
                    button.FlatAppearance.MouseDownBackColor = Blend(Colors.Success, Colors.ButtonPressed, 0.42);
                    break;

                case ButtonTone.Danger:
                    button.BackColor = Blend(Colors.Danger, Colors.ButtonBackground, 0.34);
                    button.ForeColor = Color.FromArgb(255, 234, 236);
                    button.FlatAppearance.BorderColor = Blend(Colors.DangerLight, Colors.Border, 0.58);
                    button.FlatAppearance.MouseOverBackColor = Blend(Colors.Danger, Colors.ButtonHover, 0.52);
                    button.FlatAppearance.MouseDownBackColor = Blend(Colors.Danger, Colors.ButtonPressed, 0.42);
                    break;

                case ButtonTone.Warning:
                    button.BackColor = Blend(Colors.Warning, Colors.ButtonBackground, 0.28);
                    button.ForeColor = Color.FromArgb(255, 246, 220);
                    button.FlatAppearance.BorderColor = Blend(Colors.WarningLight, Colors.Border, 0.52);
                    button.FlatAppearance.MouseOverBackColor = Blend(Colors.Warning, Colors.ButtonHover, 0.45);
                    button.FlatAppearance.MouseDownBackColor = Blend(Colors.Warning, Colors.ButtonPressed, 0.35);
                    break;

                default:
                    button.BackColor = Colors.ButtonBackground;
                    button.ForeColor = Colors.Foreground;
                    button.FlatAppearance.BorderColor = Colors.Border;
                    button.FlatAppearance.MouseOverBackColor = Colors.ButtonHover;
                    button.FlatAppearance.MouseDownBackColor = Colors.ButtonPressed;
                    break;
            }
        }

        private static void StyleButton(Button button)
        {
            if (button.Tag is string tag && string.Equals(tag, "skip", StringComparison.OrdinalIgnoreCase))
                return;

            ApplyButtonPalette(button, ResolveButtonTone(button));
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Colors.BackgroundDark;
            textBox.ForeColor = Colors.Foreground;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Colors.BackgroundDark;
            comboBox.ForeColor = Colors.Foreground;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.ForeColor = Colors.Foreground;
        }

        private static void StyleRadioButton(RadioButton radioButton)
        {
            radioButton.ForeColor = Colors.Foreground;
        }

        private static void StyleLabel(Label label)
        {
            // Recolor only default-coloured labels; leave deliberate accents alone.
            int argb = label.ForeColor.ToArgb();
            if (argb == SystemColors.ControlText.ToArgb() || argb == Color.Black.ToArgb())
                label.ForeColor = Colors.Foreground;
        }

        private static void StyleLinkLabel(LinkLabel link)
        {
            link.LinkColor = Colors.Accent;
            link.ActiveLinkColor = Colors.AccentCyan;
            link.VisitedLinkColor = Colors.AccentViolet;
            link.ForeColor = Colors.Foreground;
        }

        private static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.ForeColor = Colors.AccentCyan;
            groupBox.Font = Fonts.DefaultBold;
            groupBox.BackColor = Colors.Background;
        }

        private static void StylePanel(Panel panel)
        {
            // Leave deliberately-coloured panels (e.g. gradient header) alone.
            if (panel.BackColor == SystemColors.Control)
                panel.BackColor = Colors.Background;
        }

        private static void StyleContainer(Control c)
        {
            if (c.BackColor == SystemColors.Control)
                c.BackColor = Colors.Background;
        }

        private static void StyleListBox(ListBox listBox)
        {
            listBox.BackColor = Colors.BackgroundDark;
            listBox.ForeColor = Colors.Foreground;
            listBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleListView(ListView listView)
        {
            listView.BackColor = Colors.BackgroundDark;
            listView.ForeColor = Colors.Foreground;
            listView.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleTreeView(TreeView treeView)
        {
            treeView.BackColor = Colors.BackgroundDark;
            treeView.ForeColor = Colors.Foreground;
            treeView.BorderStyle = BorderStyle.FixedSingle;
            treeView.LineColor = Colors.Border;
        }

        private static void StyleNumericUpDown(NumericUpDown numeric)
        {
            numeric.BackColor = Colors.BackgroundDark;
            numeric.ForeColor = Colors.Foreground;
            numeric.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleDateTimePicker(DateTimePicker picker)
        {
            picker.CalendarForeColor = Colors.Foreground;
            picker.CalendarMonthBackground = Colors.BackgroundDark;
        }

        private static void StyleDataGridView(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = Colors.BackgroundDark;
            dataGridView.ForeColor = Colors.Foreground;
            dataGridView.GridColor = Colors.Border;
            dataGridView.BorderStyle = BorderStyle.None;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Colors.Surface3;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = Colors.Surface3;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.DefaultCellStyle.BackColor = Colors.BackgroundDark;
            dataGridView.DefaultCellStyle.ForeColor = Colors.Foreground;
            dataGridView.DefaultCellStyle.SelectionBackColor = Colors.Selection;
            dataGridView.DefaultCellStyle.SelectionForeColor = Colors.BackgroundDarkest;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Colors.Surface2;
            dataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Colors.Foreground;
        }

        private static void StyleTabControl(TabControl tabControl)
        {
            tabControl.Font = Fonts.Default;
            // TabControl owner-painting is limited; leave default painting but
            // ensure embedded pages get themed.
            foreach (TabPage page in tabControl.TabPages)
            {
                page.BackColor = Colors.Background;
                page.ForeColor = Colors.Foreground;
            }
        }

        private static void StyleRichTextBox(RichTextBox richTextBox)
        {
            richTextBox.BackColor = Colors.BackgroundDark;
            richTextBox.ForeColor = Colors.Foreground;
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
            if (richTextBox.Font == null) richTextBox.Font = Fonts.Monospace;
        }

        private static void StyleToolStrip(ToolStrip toolStrip)
        {
            toolStrip.BackColor = Colors.Surface3;
            toolStrip.ForeColor = Colors.Foreground;
            toolStrip.RenderMode = ToolStripRenderMode.System;
        }

        /// <summary>
        /// Styles a button as a primary action button (azure/highlighted).
        /// </summary>
        public static void StylePrimaryButton(Button button)
        {
            button.Tag = "primary";
            ApplyButtonPalette(button, ButtonTone.Primary);
            button.Font = Fonts.DefaultBold;
        }

        /// <summary>
        /// Styles a button as a danger button (red, for deletions / exit).
        /// </summary>
        public static void StyleDangerButton(Button button)
        {
            button.Tag = "danger";
            ApplyButtonPalette(button, ButtonTone.Danger);
            button.Font = Fonts.DefaultBold;
        }

        /// <summary>
        /// Styles a button as a success button (green).
        /// </summary>
        public static void StyleSuccessButton(Button button)
        {
            button.Tag = "success";
            ApplyButtonPalette(button, ButtonTone.Success);
            button.Font = Fonts.DefaultBold;
        }

        /// <summary>
        /// Paints a horizontal gradient (azure-violet over charcoal) onto a panel,
        /// with a thin accent line along the bottom edge — the signature header band.
        /// Call once; it wires the Paint handler and makes the panel double-buffered.
        /// </summary>
        public static void StyleHeaderBar(Panel panel)
        {
            if (panel == null) return;
            panel.BackColor = Colors.GradientStart;
            typeof(Panel).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(panel, true, null);

            panel.Paint += (s, e) =>
            {
                var rect = panel.ClientRectangle;
                if (rect.Width <= 0 || rect.Height <= 0) return;
                using (var brush = new LinearGradientBrush(rect, Colors.GradientStart, Colors.GradientEnd, 0f))
                {
                    var blend = new ColorBlend
                    {
                        Colors = new[] { Colors.GradientStart, Colors.GradientMid, Colors.GradientEnd },
                        Positions = new[] { 0f, 0.5f, 1f }
                    };
                    brush.InterpolationColors = blend;
                    e.Graphics.FillRectangle(brush, rect);
                }
                // Accent underline.
                using (var pen = new Pen(Colors.Accent, 2f))
                    e.Graphics.DrawLine(pen, 0, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            };
        }

        // ===================== Dashboard building blocks =====================

        /// <summary>Builds a rounded-rectangle path for cards / pills.</summary>
        public static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            if (radius <= 0) { path.AddRectangle(r); path.CloseFigure(); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Turns a panel into an elevated, rounded "card" surface with a soft
        /// border — the building block of the dashboard's right-hand panels.
        /// </summary>
        public static void StyleCard(Panel panel, int radius = 14)
        {
            if (panel == null) return;
            panel.BackColor = Colors.Surface2;
            SetDoubleBuffered(panel);

            void Reshape()
            {
                if (panel.Width <= 0 || panel.Height <= 0) return;
                using (var p = RoundedRect(new Rectangle(0, 0, panel.Width, panel.Height), radius))
                    panel.Region = new Region(p);
            }
            Reshape();
            panel.Resize += (s, e) => { Reshape(); panel.Invalidate(); };
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var p = RoundedRect(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), radius))
                using (var pen = new Pen(Colors.Border))
                    e.Graphics.DrawPath(pen, p);
            };
        }

        /// <summary>Dark, flat sidebar surface.</summary>
        public static void StyleSidebar(Panel panel)
        {
            if (panel == null) return;
            panel.BackColor = Colors.BackgroundDarkest;
        }

        /// <summary>
        /// Paints a subtle diagonal gradient behind a panel (the main dashboard
        /// surface). Child controls with their own background sit on top.
        /// </summary>
        public static void PaintBackgroundGradient(Control c)
        {
            if (c == null) return;
            SetDoubleBuffered(c);
            c.Paint += (s, e) =>
            {
                var r = c.ClientRectangle;
                if (r.Width <= 0 || r.Height <= 0) return;
                using (var brush = new LinearGradientBrush(r, Colors.GradientMid, Colors.BackgroundDarkest, 55f))
                    e.Graphics.FillRectangle(brush, r);
            };
        }

        /// <summary>
        /// Styles a left-rail navigation button: full-width, left-aligned, with an
        /// icon glyph, a hover/active highlight and an accent bar when active.
        /// </summary>
        public static void StyleNavButton(Button b, bool active = false, string glyph = null)
        {
            if (b == null) return;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.TextAlign = ContentAlignment.MiddleLeft;
            b.Padding = new Padding(48, 0, 0, 0);
            b.Font = new Font(Fonts.UiFamily, 10F, active ? FontStyle.Bold : FontStyle.Regular);
            b.ForeColor = active ? Colors.AccentCyan : Colors.ForegroundMuted;
            b.BackColor = active ? Colors.Surface2 : Colors.BackgroundDarkest;
            b.UseVisualStyleBackColor = false;
            b.FlatAppearance.MouseOverBackColor = Colors.Surface2;
            b.FlatAppearance.MouseDownBackColor = Colors.Surface3;
            b.Tag = "skip"; // don't let the generic themer overwrite this
            SetDoubleBuffered(b);
            bool isActive = active;
            b.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                // Active accent bar.
                if (isActive)
                    using (var br = new SolidBrush(Colors.AccentCyan))
                        e.Graphics.FillRectangle(br, 0, 6, 3, b.Height - 12);
                // Icon glyph.
                if (!string.IsNullOrEmpty(glyph))
                {
                    using (var iconFont = new Font(Fonts.IconFamily, 12F))
                    using (var br = new SolidBrush(isActive ? Colors.AccentCyan : Colors.ForegroundMuted))
                    {
                        var sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };
                        e.Graphics.DrawString(glyph, iconFont, br, new RectangleF(16, 0, 24, b.Height), sf);
                    }
                }
            };
        }

        /// <summary>
        /// Styles a card action button: flat, soft border, sized text in
        /// the UI font (not the terminal mono font). Tagged so the generic themer
        /// leaves it alone.
        /// </summary>
        public static void StyleActionButton(Button b, int radius = 8)
        {
            if (b == null) return;
            var tone = ResolveButtonTone(b);
            b.Tag = "skip";
            ApplyButtonPalette(b, tone);
            b.Font = new Font(Fonts.UiFamily, 9.75F, FontStyle.Regular);
            SetDoubleBuffered(b);
            b.Region = null;
        }

        /// <summary>A bold card/section title in the accent colour.</summary>
        public static Label CreateCardTitle(string text)
        {
            return new Label
            {
                Text = text.ToUpperInvariant(),
                Font = new Font(Fonts.UiFamily, 8.5F, FontStyle.Bold),
                ForeColor = Colors.ForegroundMuted,
                BackColor = Color.Transparent,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 24,
                Padding = new Padding(2, 4, 0, 0)
            };
        }

        /// <summary>Reflection helper to enable double-buffering on any control.</summary>
        public static void SetDoubleBuffered(Control c)
        {
            try
            {
                typeof(Control).GetProperty("DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(c, true, null);
            }
            catch { }
        }

        /// <summary>
        /// Creates a styled section header label (uses the futuristic display font).
        /// </summary>
        public static Label CreateHeaderLabel(string text, int x, int y, int width = 400)
        {
            return new Label
            {
                Text = text,
                Font = Fonts.Header,
                ForeColor = Colors.Foreground,
                BackColor = Color.Transparent,
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
