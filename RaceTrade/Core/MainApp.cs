using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using RaceTrader;

namespace RaceTrade
{
    public partial class MainApp : AntdUI.Window
    {
        private List<string> siteFiles = new List<string>();
        private bool isTraderRunning = false;

        private KnightRiderScanner traderScanner;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationTokenSource chatCancellationTokenSource; 
                                                                    
        private bool _dockLogsToMain = true; 
        private bool _restoringLogPositions = false;

        private const string LOG_WINDOW_SETTINGS_KEY = "log_window_layout";
        private const string COMMUNITY_GITHUB_URL = "https://github.com/Bl4DiEDiEBL4/RaceTrade";
        private const int WM_NCHITTEST = 0x0084;
        private const int WM_NCLBUTTONDOWN = 0x00A1;
        private const int HTCLIENT = 1;
        private const int HTCAPTION = 2;
        private const int DragTopStripHeight = 34;
        private const int RightChromeReserveWidth = 180;

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private List<Thread> ircThreads = new List<Thread>();
        private Dictionary<string, SiteConfig> siteConfigs = new Dictionary<string, SiteConfig>();
        private RaceLog raceLog;
        private ApplicationLog applicationLog;
        private bool _isPositioningLogs = false;
        private bool _hasCenteredMainForm = false;
        private static bool _debugEnabled;
        public static bool DebugEnabled
        {
            get => _debugEnabled;
            private set
            {
                _debugEnabled = value;
                LogManager.DebugEnabled = value;
            }
        }
        public static bool AllowInsecureSsl { get; set; }  
        private const string SETTINGS_FILE = "settings/settings.json";
        private const string GlobalBlacklistPath = "settings/global_blacklist.json";
        private List<string> globalBlacklistPatterns = new List<string>();


        private HelpForm helpForm;
        private ChangelogForm changelogForm;
        private RaceTrader.FtpClientForm _ftpClientForm;
        private IrcLog logOutput;
        private TabbedIrcLog tabbedIrcLog;
        private CBFTPIntegrationLog cbftpLog;

        /// <summary>
        /// Toggles a singleton child window: shows and focuses it if hidden/closed,
        /// hides it if currently visible. Never spawns duplicates.
        /// </summary>
        private void ToggleWindow<T>(ref T instance, Func<T> factory) where T : Form
        {
            if (instance != null && !instance.IsDisposed && instance.Visible)
            {
                instance.Hide();
                return;
            }
            if (instance == null || instance.IsDisposed)
            {
                instance = factory();
                instance.Owner = this;
            }
            instance.Show();
            instance.BringToFront();
            instance.Activate();
        }

        // split race vs chat IRC clients
        // ConcurrentDictionary: entries are added from parallel Task.Run bodies while the
        // UI thread enumerates/clears — a plain Dictionary corrupts under concurrent writes.
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, IRCClient> raceIrcClients = new System.Collections.Concurrent.ConcurrentDictionary<string, IRCClient>();
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ChatIrcClient> chatIrcClients = new System.Collections.Concurrent.ConcurrentDictionary<string, ChatIrcClient>();

        private static MainApp _instance;

        /// <summary>
        /// Installs channel keys straight into the RUNNING IRC clients for a site using the
        /// raw (un-encrypted) key — exactly what the chatbox's SetChannelKey does. This means
        /// keys entered in the site editor take effect immediately, without a restart and
        /// without depending on the DPAPI encrypt/decrypt round-trip.
        /// </summary>
        public static void ApplyChannelKeysToRunningClients(string siteName, IDictionary<string, string> channelKeys)
        {
            var app = _instance;
            if (app == null || string.IsNullOrWhiteSpace(siteName) || channelKeys == null)
                return;

            foreach (var kvp in channelKeys)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                    continue;

                try
                {
                    if (app.raceIrcClients.TryGetValue(siteName, out var raceClient))
                        raceClient.SetChannelKey(kvp.Key, kvp.Value, persist: false);

                    if (app.chatIrcClients.TryGetValue(siteName, out var chatClient))
                        chatClient.SetChannelKey(kvp.Key, kvp.Value, persist: false);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Failed to apply Blowfish key for {kvp.Key} on [{siteName}]: {ex.Message}");
                }
            }
        }


       // private enum LogLayout
      ////  {
      //      All,           // All 3 logs visible
       //     NoApplication, // Only IRC and CBFTP
       //     NoIRC,         // Only Application and CBFTP
      //      NoCBFTP        // Only Application and IRC
      //  }

      //  private LogLayout currentLayout = LogLayout.All;


        public MainApp()
        {
            InitializeComponent();
            LoadConfigIntoDropdown();
            _instance = this;
            CbftpRacer.SetMainForm(this);
            this.Move += Form1_Move;
            this.Resize += Form1_Resize;


            traderScanner = new KnightRiderScanner
            {
                Size = new Size(200, 18),
                Location = new Point(4, 18),
                LEDCount = 25,
                ScannerColor = Color.FromArgb(0, 220, 100),
                BackColor = Color.Transparent,
                Visible = false
            };
            this.Controls.Add(traderScanner);
            //Enable_Disable_Racer_button.Controls.Add(traderScanner);
            Enable_Disable_Racer_button.Padding = new Padding(0, 0, 0, 0);

            WireDockLogsLabel();
            WireCommunityLinkLabel();
            WireWindowDragSurface();
        }

        private void WireDockLogsLabel()
        {
            if (lblDockLogs == null) return;

            lblDockLogs.Cursor = Cursors.Hand;

            // Optional hover underline
            lblDockLogs.MouseEnter += (s, e) =>
                lblDockLogs.Font = new Font(lblDockLogs.Font, FontStyle.Underline);

            lblDockLogs.MouseLeave += (s, e) =>
                lblDockLogs.Font = new Font(lblDockLogs.Font, FontStyle.Regular);

            lblDockLogs.Click += (s, e) => ToggleDockLogsMode();

            UpdateDockLogsLabelText();
        }

        private void ToggleDockLogsMode()
        {
            _dockLogsToMain = !_dockLogsToMain;

            // Persist immediately
            SaveLogWindowLayout();

            if (_dockLogsToMain)
            {
                // Snap logs back to main form positions
                PositionLogForms();
            }

            UpdateDockLogsLabelText();
        }

        private void UpdateDockLogsLabelText()
        {
            if (lblDockLogs == null) return;
            lblDockLogs.Text = _dockLogsToMain ? "Logs: Docked" : "Logs: Free";
        }

        private void WireCommunityLinkLabel()
        {
            if (communityLinkLabel == null) return;

            communityLinkLabel.Cursor = Cursors.Hand;
            communityLinkLabel.LinkClicked += communityLinkLabel_LinkClicked;
        }

        private void communityLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = COMMUNITY_GITHUB_URL,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to open RaceTrade GitHub link: {ex.Message}");
                MessageBox.Show("Could not open the RaceTrade GitHub page.", "RaceTrade",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void WireWindowDragSurface()
        {
            WireDragControl(this);
            WireDragControl(contentPanel);
            WireDragControl(sidebarPanel);
            WireDragControl(logoPanel);
            WireDragControl(titleLabel);
            WireDragControl(label1);
            WireDragControl(pageTitleLabel);
            WireDragControl(navMenuLabel);
            WireDragControl(statusTitleLabel);
            WireDragControl(cardCbftpTitle);
            WireDragControl(cardSitesTitle);
            WireDragControl(cardPrebotsTitle);
            WireDragControl(cardLogsTitle);
            WireDragControl(cardToolsTitle);
        }

        private void WireDragControl(Control control)
        {
            if (control == null) return;

            control.MouseDown -= DashboardDragSurface_MouseDown;
            control.MouseDown += DashboardDragSurface_MouseDown;
        }

        private void DashboardDragSurface_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg != WM_NCHITTEST || (int)m.Result != HTCLIENT) return;

            var screenPoint = new Point(
                unchecked((short)(long)m.LParam),
                unchecked((short)((long)m.LParam >> 16)));
            var clientPoint = PointToClient(screenPoint);

            if (clientPoint.Y >= 0 &&
                clientPoint.Y <= DragTopStripHeight &&
                clientPoint.X < Width - RightChromeReserveWidth)
            {
                m.Result = (IntPtr)HTCAPTION;
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
        //        return cp;
        //    }
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            // Center main form ONCE at startup
            if (!_hasCenteredMainForm)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.CenterToScreen();
                _hasCenteredMainForm = true;
            }

            ApplyNebulaTheme();

            LoadApplicationSettings();
            UpdateTraderButton();
            InitializeLogForms();
            LoadGlobalBlacklist();


            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            this.FormClosing += MainApp_FormClosing;

            if (_dockLogsToMain)
                ForceRepositionLogs();
            else
                // keep their saved free positions
                LoadLogWindowLayout(); 
        }

        private void ApplyNebulaTheme()
        {
            try
            {
                StyleDashboard();
            }
            catch
            {
                try { ThemeManager.ApplyTheme(this); } catch { }
            }
        }

        private void StyleDashboard()
        {
            // Base dark theme for the whole window.
            Dark = true;
            Mode = AntdUI.TAMode.Dark;
            EnableHitTest = true;
            ThemeManager.ApplyTheme(this);

            // Surfaces: the whole right content area is one gradient panel; the
            // header text and cards sit directly on it.
            ThemeManager.StyleSidebar(sidebarPanel);
            ThemeManager.PaintBackgroundGradient(contentPanel);

            // Brand + headings (futuristic display font).
            titleLabel.Font = new Font(ThemeManager.Fonts.DisplayFamily, 15F, FontStyle.Bold);
            titleLabel.ForeColor = ThemeManager.Colors.Foreground;
            label1.Font = ThemeManager.Fonts.Subtitle;
            pageTitleLabel.Font = new Font(ThemeManager.Fonts.DisplayFamily, 18F, FontStyle.Bold);
            navMenuLabel.Font = new Font(ThemeManager.Fonts.UiFamily, 7.5F, FontStyle.Bold);
            communityLinkLabel.Font = new Font(ThemeManager.Fonts.UiFamily, 8F, FontStyle.Bold);
            communityLinkLabel.LinkColor = ThemeManager.Colors.AccentCyan;
            communityLinkLabel.ActiveLinkColor = ThemeManager.Colors.Accent;
            communityLinkLabel.VisitedLinkColor = ThemeManager.Colors.AccentCyan;
            statusTitleLabel.Font = new Font(ThemeManager.Fonts.UiFamily, 8F, FontStyle.Bold);
            lblDockLogs.Font = new Font(ThemeManager.Fonts.UiFamily, 8.5F, FontStyle.Bold);

            // Logo diamond.
            ThemeManager.SetDoubleBuffered(logoPanel);
            logoPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var pts = new[] { new Point(12, 0), new Point(24, 12), new Point(12, 24), new Point(0, 12) };
                using (var br = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 24, 24), ThemeManager.Colors.AccentCyan, ThemeManager.Colors.Accent, 45f))
                    e.Graphics.FillPolygon(br, pts);
            };

            // AntdUI card surfaces.
            foreach (var card in new[] { statusCard, cardCbftp, cardSites, cardPrebots, cardLogs, cardTools })
                StyleDashboardCard(card);

            // Card titles.
            foreach (var lbl in new[] { cardCbftpTitle, cardSitesTitle, cardPrebotsTitle, cardLogsTitle, cardToolsTitle })
                lbl.Font = new Font(ThemeManager.Fonts.UiFamily, 9F, FontStyle.Bold);

            // AntdUI dashboard action buttons.
            foreach (var b in new[] {
                Add_Cbftp_button, Add_Ccbftp_Sections, Add_cbftp_site, Cbftp_Edit_Site, Sync_From_Cbftp_Button,
                add_sites_button, Edit_sites_button,
                Add_PreBot_button, Prebot_edit_button, buttonImportPredb,
                ToggleCBFTPLog, ToggleIRCLog, ToggleApplicationLog, ToggleRaceLog,
                button2, blacklist_add })
                StyleDashboardButton(b);

            // Exit stays danger-toned.
            StyleDashboardButton(exitButton, AntdUI.TTypeMini.Error);

            // Big trader Start/Stop button.
            Enable_Disable_Racer_button.Font = new Font(ThemeManager.Fonts.UiFamily, 11F, FontStyle.Bold);

            // Re-home the KnightRider scanner into the status card.
            if (traderScanner != null)
            {
                traderScanner.Parent = statusCard;
                traderScanner.Size = new Size(172, 16);
                traderScanner.Location = new Point(16, 98);
            }

            // Left-rail nav buttons with icon glyphs.
            ThemeManager.StyleNavButton(navDashboard, true, NavGlyph("home"));
            ThemeManager.StyleNavButton(Ftp_button, false, NavGlyph("sync"));
            ThemeManager.StyleNavButton(OpenTabbedIRC, false, NavGlyph("chat"));
            ThemeManager.StyleNavButton(Pre_button, false, NavGlyph("flag"));
            ThemeManager.StyleNavButton(button1, false, NavGlyph("settings"));
            ThemeManager.StyleNavButton(Help_button, false, NavGlyph("help"));
            ThemeManager.StyleNavButton(Changelog_button, false, NavGlyph("changelog"));

            UpdateTraderButton();
            UpdateLogButtonStates();
            LoadConfigIntoDropdown();
        }

        private static void StyleDashboardCard(AntdUI.Panel card)
        {
            if (card == null) return;

            card.Back = ThemeManager.Colors.Surface2;
            card.BackColor = ThemeManager.Colors.Surface2;
            card.ForeColor = ThemeManager.Colors.Foreground;
            card.BorderColor = ThemeManager.Colors.Border;
            card.BorderWidth = 1F;
            card.Radius = 8;
            card.Shadow = 0;
            card.AutoContainerBgTransparent = true;
            ThemeManager.SetDoubleBuffered(card);
        }

        private static void StyleDashboardButton(AntdUI.Button button, AntdUI.TTypeMini? forcedType = null)
        {
            if (button == null) return;

            button.Type = forcedType ?? ResolveDashboardButtonType(button);
            button.Shape = AntdUI.TShape.Default;
            button.Radius = 2;
            button.BorderWidth = 1F;
            button.Font = new Font(ThemeManager.Fonts.UiFamily, 9.5F, FontStyle.Regular);
            ApplyDashboardButtonPalette(button);
        }

        private static void ApplyDashboardButtonPalette(AntdUI.Button button)
        {
            Color back;
            Color hover;
            Color active;
            Color border;
            Color fore = Color.FromArgb(232, 238, 247);

            switch (button.Type)
            {
                case AntdUI.TTypeMini.Success:
                    back = Color.FromArgb(39, 132, 92);
                    hover = Color.FromArgb(46, 154, 108);
                    active = Color.FromArgb(31, 113, 78);
                    border = Color.FromArgb(55, 177, 123);
                    break;

                case AntdUI.TTypeMini.Error:
                    back = Color.FromArgb(146, 61, 76);
                    hover = Color.FromArgb(170, 70, 88);
                    active = Color.FromArgb(124, 51, 65);
                    border = Color.FromArgb(211, 91, 109);
                    fore = Color.FromArgb(255, 238, 241);
                    break;

                case AntdUI.TTypeMini.Primary:
                    back = Color.FromArgb(22, 114, 132);
                    hover = Color.FromArgb(25, 137, 158);
                    active = Color.FromArgb(18, 94, 110);
                    border = Color.FromArgb(0, 196, 210);
                    break;

                default:
                    back = ThemeManager.Colors.ButtonBackground;
                    hover = ThemeManager.Colors.ButtonHover;
                    active = ThemeManager.Colors.ButtonPressed;
                    border = ThemeManager.Colors.Border;
                    break;
            }

            button.DefaultBack = back;
            button.BackColor = back;
            button.BackHover = hover;
            button.BackActive = active;
            button.DefaultBorderColor = border;
            button.ForeColor = fore;
            button.ForeHover = Color.FromArgb(248, 252, 255);
            button.ForeActive = Color.FromArgb(248, 252, 255);
        }

        private static AntdUI.TTypeMini ResolveDashboardButtonType(AntdUI.Button button)
        {
            string key = $"{button.Name} {button.Text}";

            if (ContainsAny(key, "delete", "remove", "exit", "cancel", "clear", "stop"))
                return AntdUI.TTypeMini.Error;

            if (ContainsAny(key, "add", "import", "sync", "start", "save"))
                return AntdUI.TTypeMini.Success;

            if (ContainsAny(key, "edit", "section", "test", "refresh", "fetch", "send", "blacklist"))
                return AntdUI.TTypeMini.Primary;

            return AntdUI.TTypeMini.Default;
        }

        private static void SetDashboardToggleButton(AntdUI.Button button, bool active)
        {
            if (button == null) return;

            button.Type = active ? AntdUI.TTypeMini.Primary : AntdUI.TTypeMini.Default;
            button.Shape = AntdUI.TShape.Default;
            button.Radius = 2;
            button.BorderWidth = 1F;
            ApplyDashboardButtonPalette(button);

            if (!active)
            {
                var back = Color.FromArgb(28, 33, 45);
                var hover = Color.FromArgb(65, 76, 100);
                var activeBack = Color.FromArgb(54, 64, 84);
                button.DefaultBack = back;
                button.BackColor = back;
                button.BackHover = hover;
                button.BackActive = activeBack;
                button.DefaultBorderColor = Color.FromArgb(78, 88, 110);
                button.ForeColor = Color.FromArgb(226, 232, 242);
                button.ForeHover = Color.FromArgb(255, 255, 255);
                button.ForeActive = Color.FromArgb(255, 255, 255);
            }
        }

        private static bool ContainsAny(string value, params string[] terms)
        {
            foreach (var term in terms)
            {
                if (value.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private static string NavGlyph(string key)
        {
            int code;
            switch (key)
            {
                case "home": code = 0xE80F; break;
                case "sync": code = 0xE895; break;
                case "chat": code = 0xE8BD; break;
                case "flag": code = 0xE7C1; break;
                case "settings": code = 0xE713; break;
                case "help": code = 0xE897; break;
                case "changelog": code = 0xE8A5; break;
                default: return null;
            }
            return ((char)code).ToString();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            if (_dockLogsToMain) PositionLogForms();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) return;
            if (_dockLogsToMain) PositionLogForms();
        }

        private void AttachLogMoveResizePersistence()
        {
            void Wire(Form f)
            {
                if (f == null) return;

                f.Move += (_, __) => SaveLogWindowLayout();
                f.ResizeEnd += (_, __) => SaveLogWindowLayout();
                f.FormClosing += (_, __) => SaveLogWindowLayout();
            }

            Wire(applicationLog);
            Wire(raceLog);
            Wire(logOutput);
            Wire(cbftpLog);
        }

        private JObject CaptureFormBounds(Form f)
        {
            if (f == null || f.IsDisposed) return null;

            // Use RestoreBounds when minimized/maximized
            Rectangle b = f.WindowState == FormWindowState.Normal ? f.Bounds : f.RestoreBounds;

            return new JObject
            {
                ["x"] = b.X,
                ["y"] = b.Y,
                ["w"] = b.Width,
                ["h"] = b.Height,
                ["visible"] = f.Visible
            };
        }

        private void ApplyFormBounds(Form f, JObject o)
        {
            if (f == null || f.IsDisposed || o == null) return;

            int x = o.Value<int?>("x") ?? f.Left;
            int y = o.Value<int?>("y") ?? f.Top;
            bool vis = o.Value<bool?>("visible") ?? f.Visible;

            f.StartPosition = FormStartPosition.Manual;
            // Restore only the saved position; keep width matched to the main window.
            f.Width = this.Width;
            f.Location = new Point(x, y);

            if (vis) f.Show();
            else f.Hide();
        }

        private void SaveLogWindowLayout()
        {
            if (_restoringLogPositions) return;

            try
            {
                JObject root = File.Exists(SETTINGS_FILE)
                    ? JObject.Parse(File.ReadAllText(SETTINGS_FILE))
                    : new JObject();

                var layout = new JObject
                {
                    ["dockLogsToMain"] = _dockLogsToMain
                };

                // Only store bounds in free mode
                if (!_dockLogsToMain)
                {
                    layout["applicationLog"] = CaptureFormBounds(applicationLog);
                    layout["raceLog"] = CaptureFormBounds(raceLog);
                    layout["ircLog"] = CaptureFormBounds(logOutput);
                    layout["cbftpLog"] = CaptureFormBounds(cbftpLog);
                }

                root[LOG_WINDOW_SETTINGS_KEY] = layout;

                Directory.CreateDirectory(Path.GetDirectoryName(SETTINGS_FILE) ?? "settings");
                AtomicFile.WriteAllText(SETTINGS_FILE, root.ToString(Formatting.Indented));
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to save log window layout: {ex.Message}");
            }
        }


        private void LoadLogWindowLayout()
        {
            try
            {
                if (!File.Exists(SETTINGS_FILE)) return;

                var root = JObject.Parse(File.ReadAllText(SETTINGS_FILE));
                var layout = root[LOG_WINDOW_SETTINGS_KEY] as JObject;
                if (layout == null) return;

                _dockLogsToMain = layout.Value<bool?>("dockLogsToMain") ?? _dockLogsToMain;

                // If dock mode => ignore saved positions and keep your docking behavior
                if (_dockLogsToMain) return;

                _restoringLogPositions = true;

                ApplyFormBounds(applicationLog, layout["applicationLog"] as JObject);
                ApplyFormBounds(raceLog, layout["raceLog"] as JObject);
                ApplyFormBounds(logOutput, layout["ircLog"] as JObject);
                ApplyFormBounds(cbftpLog, layout["cbftpLog"] as JObject);
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to load log window layout: {ex.Message}");
            }
            finally
            {
                _restoringLogPositions = false;
            }
        }

        private void InitializeLogForms()
        {
            // Application Log window (top)
            if (applicationLog == null || applicationLog.IsDisposed)
            {
                applicationLog = new ApplicationLog { Owner = this };

                //FORCE HANDLE CREATION so Invoke() is safe even when hidden
                var handle = applicationLog.Handle;

                applicationLog.Hide();
            }
            else
            {
                applicationLog.Show();
                applicationLog.RestoreLogs();
                applicationLog.BringToFront();
            }

            // Race Log window
            if (raceLog == null || raceLog.IsDisposed)
            {
                raceLog = new RaceLog { Owner = this };
                var rh = raceLog.Handle;
                raceLog.Hide();
            }
            else
            {
                raceLog.Show();
                raceLog.RestoreLogs();
                raceLog.BringToFront();
            }

            if (logOutput == null || logOutput.IsDisposed)
            {
                logOutput = new IrcLog { Owner = this };

                // FORCE HANDLE CREATION so background IRC logging can Invoke()
                var handle = logOutput.Handle;

                logOutput.Hide();
            }
            else
            {
                logOutput.Show();
                logOutput.RestoreLogs();
                logOutput.BringToFront();
            }

            if (cbftpLog == null || cbftpLog.IsDisposed)
            {
                cbftpLog = new CBFTPIntegrationLog { Owner = this };
                var ch = cbftpLog.Handle;
                cbftpLog.Hide();
            }
            else
            {
                cbftpLog.Show();
                cbftpLog.RestoreLogs();
                cbftpLog.BringToFront();
            }

            // Initialize tabbed IRC log
            if (tabbedIrcLog == null || tabbedIrcLog.IsDisposed)
            {
                tabbedIrcLog = new TabbedIrcLog { Owner = this };

                // ⭐ DON'T SHOW IT YET - only show when Chat button is clicked

                tabbedIrcLog.SetSendMessageCallback(async (siteName, channel, message) =>
                {
                    if (chatIrcClients.TryGetValue(siteName, out var client))
                        await client.SendChannelMessage(channel, message);
                    else
                        MessageBox.Show($"Chat IRC client for {siteName} not found", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                });

                tabbedIrcLog.SetFishKeyExchangeCallback(async (siteName, username, _) =>
                {
                    if (chatIrcClients.TryGetValue(siteName, out var client))
                        await client.InitiateFishKeyExchange(username);
                    else
                        MessageBox.Show($"Chat IRC client for {siteName} not found", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                });

                tabbedIrcLog.SetChannelKeyCallback((siteName, channel, key) =>
                {
                    if (chatIrcClients.TryGetValue(siteName, out var client))
                        client.SetChannelKey(channel, key, persist: true);
                    else
                        MessageBox.Show($"Chat IRC client for {siteName} not found",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });

                tabbedIrcLog.SetChannelKeyLookupCallback((siteName, channel) =>
                {
                    return chatIrcClients.TryGetValue(siteName, out var client)
                        ? client.GetChannelKey(channel)
                        : string.Empty;
                });

                tabbedIrcLog.VisibleChanged += TabbedIrcLog_VisibleChanged;
            }

            // Initialize LogManager
            LogManager.Initialize(applicationLog, this, logOutput, cbftpLog, raceLog);
            LogManager.DebugEnabled = DebugEnabled;

            AttachLogVisibilityHandlers();

            AttachLogMoveResizePersistence();
            LoadLogWindowLayout();

            if (_dockLogsToMain)
                PositionLogForms();

            UpdateLogButtonStates();

            UpdateDockLogsLabelText();
        }
        
        private void UpdateLogButtonStates()
        {
            // Application Log button state
            if (applicationLog != null && !applicationLog.IsDisposed)
            {
                if (applicationLog.Visible)
                {
                    SetDashboardToggleButton(ToggleApplicationLog, true);
                    ToggleApplicationLog.Text = "App";
                }
                else
                {
                    SetDashboardToggleButton(ToggleApplicationLog, false);
                    ToggleApplicationLog.Text = "App (Off)";
                }
            }

            // CBFTP Log button state
            if (cbftpLog != null && !cbftpLog.IsDisposed)
            {
                if (cbftpLog.Visible)
                {
                    SetDashboardToggleButton(ToggleCBFTPLog, true);
                    ToggleCBFTPLog.Text = "Cbftp Log";
                }
                else
                {
                    SetDashboardToggleButton(ToggleCBFTPLog, false);
                    ToggleCBFTPLog.Text = "Cbftp (Off)";
                }
            }

            // IRC Log button state
            if (logOutput != null && !logOutput.IsDisposed)
            {
                if (logOutput.Visible)
                {
                    SetDashboardToggleButton(ToggleIRCLog, true);
                    ToggleIRCLog.Text = "IRC Log";
                }
                else
                {
                    SetDashboardToggleButton(ToggleIRCLog, false);
                    ToggleIRCLog.Text = "IRC (Off)";
                }
            }

            // Race Log button state
            if (raceLog != null && !raceLog.IsDisposed)
            {
                if (raceLog.Visible)
                {
                    SetDashboardToggleButton(ToggleRaceLog, true);
                    ToggleRaceLog.Text = "Race Log";
                }
                else
                {
                    SetDashboardToggleButton(ToggleRaceLog, false);
                    ToggleRaceLog.Text = "Race (Off)";
                }
            }
        }
        private void ForceRepositionLogs()
        {
            if (!_dockLogsToMain) return;

            if (applicationLog != null && !applicationLog.IsDisposed)
                applicationLog.StartPosition = FormStartPosition.Manual;

            if (cbftpLog != null && !cbftpLog.IsDisposed)
                cbftpLog.StartPosition = FormStartPosition.Manual;

            if (logOutput != null && !logOutput.IsDisposed)
                logOutput.StartPosition = FormStartPosition.Manual;

            if (raceLog != null && !raceLog.IsDisposed)
                raceLog.StartPosition = FormStartPosition.Manual;

            PositionLogForms();
        }

        private void ToggleApplicationLog_Click(object sender, EventArgs e)
        {
            if (LogManager.DisableApplicationLog) return;

            if (applicationLog != null && !applicationLog.IsDisposed)
            {
                if (applicationLog.Visible)
                {
                    applicationLog.Hide();
                    SetDashboardToggleButton(ToggleApplicationLog, false);
                    ToggleApplicationLog.Text = "App (Off)";
                }
                else
                {
                    applicationLog.Show();
                    SetDashboardToggleButton(ToggleApplicationLog, true);
                    ToggleApplicationLog.Text = "App";
                }
            }
        }

        private void ToggleCBFTPLog_Click(object sender, EventArgs e)
        {
            if (LogManager.DisableCbftpLog) return;

            if (cbftpLog != null && !cbftpLog.IsDisposed)
            {
                if (cbftpLog.Visible)
                {
                    cbftpLog.Hide();
                    SetDashboardToggleButton(ToggleCBFTPLog, false);
                    ToggleCBFTPLog.Text = "Cbftp (Off)";
                }
                else
                {
                    cbftpLog.Show();
                    SetDashboardToggleButton(ToggleCBFTPLog, true);
                    ToggleCBFTPLog.Text = "Cbftp Log";
                }
            }
        }

        private void ToggleIRCLog_Click(object sender, EventArgs e)
        {
            if (logOutput != null && !logOutput.IsDisposed)
            {
                if (logOutput.Visible)
                {
                    logOutput.Hide();
                    SetDashboardToggleButton(ToggleIRCLog, false);
                    ToggleIRCLog.Text = "IRC (Off)";
                }
                else
                {
                    logOutput.Show();
                    SetDashboardToggleButton(ToggleIRCLog, true);
                    ToggleIRCLog.Text = "IRC Log";
                }
            }
        }


        private void LoadGlobalBlacklist()
        {
            try
            {
                globalBlacklistPatterns.Clear();

                if (!File.Exists(GlobalBlacklistPath))
                {
                    CreateDefaultGlobalBlacklist();
                }

                var json = File.ReadAllText(GlobalBlacklistPath);
                var config = JObject.Parse(json);

                bool enabled = config["enabled"]?.ToObject<bool>() ?? true;
                if (!enabled)
                {
                    LogManager.Info("Global blacklist is disabled");
                    RaceHelper.SetGlobalBlacklist(new List<string>());
                    return;
                }

                var patterns = config["patterns"] as JArray;
                if (patterns != null)
                {
                    foreach (var pattern in patterns)
                    {
                        var patternStr = pattern.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(patternStr))
                        {
                            globalBlacklistPatterns.Add(patternStr);
                        }
                    }
                }

                // Set the blacklist in RaceHelper
                RaceHelper.SetGlobalBlacklist(globalBlacklistPatterns);

                LogManager.Info($"Global blacklist loaded: {globalBlacklistPatterns.Count} pattern(s), Enabled: {enabled}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading global blacklist: {ex.Message}");
            }
        }

        private void CreateDefaultGlobalBlacklist()
        {
            try
            {
                var directory = Path.GetDirectoryName(GlobalBlacklistPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var defaultBlacklist = new JObject
                {
                    ["enabled"] = true,
                    ["patterns"] = new JArray()
                };

                AtomicFile.WriteAllText(GlobalBlacklistPath, defaultBlacklist.ToString(Formatting.Indented));
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to create default global blacklist: {ex.Message}");
            }
        }

        private void SaveGlobalBlacklist()
        {
            try
            {
                var config = new JObject();

                if (File.Exists(GlobalBlacklistPath))
                {
                    var existingJson = File.ReadAllText(GlobalBlacklistPath);
                    config = JObject.Parse(existingJson);
                }
                else
                {
                    config["enabled"] = true;
                }

                config["patterns"] = new JArray(globalBlacklistPatterns);

                AtomicFile.WriteAllText(GlobalBlacklistPath, config.ToString(Formatting.Indented));

                // Update RaceHelper
                RaceHelper.SetGlobalBlacklist(globalBlacklistPatterns);
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to save global blacklist: {ex.Message}");
            }
        }

        public bool AddGlobalBlacklistPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return false;

            pattern = pattern.Trim();

            if (globalBlacklistPatterns.Contains(pattern, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            globalBlacklistPatterns.Add(pattern);
            SaveGlobalBlacklist();
            return true;
        }

        public bool RemoveGlobalBlacklistPattern(string pattern)
        {
            int removed = globalBlacklistPatterns.RemoveAll(p => string.Equals(p, pattern, StringComparison.OrdinalIgnoreCase));

            if (removed > 0)
            {
                SaveGlobalBlacklist();
                return true;
            }

            return false;
        }

        public List<string> GetGlobalBlacklist()
        {
            return new List<string>(globalBlacklistPatterns);
        }

        public void ClearGlobalBlacklist()
        {
            globalBlacklistPatterns.Clear();
            SaveGlobalBlacklist();
        }

        public bool IsGlobalBlacklistEnabled()
        {
            try
            {
                if (!File.Exists(GlobalBlacklistPath))
                    return true;

                var json = File.ReadAllText(GlobalBlacklistPath);
                var config = JObject.Parse(json);
                return config["enabled"]?.ToObject<bool>() ?? true;
            }
            catch
            {
                return true;
            }
        }

        public void SetGlobalBlacklistEnabled(bool enabled)
        {
            try
            {
                JObject config;

                if (File.Exists(GlobalBlacklistPath))
                {
                    var existingJson = File.ReadAllText(GlobalBlacklistPath);
                    config = JObject.Parse(existingJson);
                }
                else
                {
                    CreateDefaultGlobalBlacklist();
                    var json = File.ReadAllText(GlobalBlacklistPath);
                    config = JObject.Parse(json);
                }

                config["enabled"] = enabled;
                AtomicFile.WriteAllText(GlobalBlacklistPath, config.ToString(Formatting.Indented));

                LoadGlobalBlacklist();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to set global blacklist enabled state: {ex.Message}");
            }
        }

        private void TabbedIrcLog_VisibleChanged(object sender, EventArgs e)
        {
            // Performance optimization: only track users when window is visible
            bool isVisible = tabbedIrcLog != null && tabbedIrcLog.Visible;

            // Enable/disable user tracking based on visibility
            foreach (var client in chatIrcClients.Values)
            {
                try
                {
                    client.SetUserTrackingEnabled(isVisible);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error toggling user tracking: {ex.Message}");
                }
            }
        }


        //private void OpenTabbedIRC_Click(object sender, EventArgs e)
        //{
        //    if (tabbedIrcLog == null || tabbedIrcLog.IsDisposed)
        //    {
        //        InitializeLogForms(); // Make sure it's initialized
        //    }

        //    tabbedIrcLog.Show();
        //    tabbedIrcLog.BringToFront();

        //    // Enable user tracking when window opens
        //    foreach (var client in chatIrcClients.Values)
        //    {
        //        client.SetUserTrackingEnabled(true);
        //    }
        //}


        private void LoadApplicationSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    var settings = JObject.Parse(json);

                    // Apply app name to taskbar
                    var appName = settings["app_name"]?.ToString();
                    if (!string.IsNullOrEmpty(appName))
                    {
                        this.Text = appName;
                    }

                    // Apply debug setting
                    var debugEnabled = settings["debug_enabled"]?.ToObject<bool>() ?? false;
                    DebugEnabled = debugEnabled;  // Just set the property

                    var allowInsecureSsl = settings["allow_insecure_ssl"]?.ToObject<bool>() ?? false;
                    AllowInsecureSsl = allowInsecureSsl;

                    var disableRaceLog = settings["disable_race_log"]?.ToObject<bool>() ?? false;
                    var disableCbftpLog = settings["disable_cbftp_log"]?.ToObject<bool>() ?? false;
                    var disableAppLog = settings["disable_app_log"]?.ToObject<bool>() ?? false;

                    // push into LogManager
                    LogManager.DisableRaceLog = disableRaceLog;
                    LogManager.DisableCbftpLog = disableCbftpLog;
                    LogManager.DisableApplicationLog = disableAppLog;


                    // Keep AntdUI buttons visually readable; handlers no-op when a log is disabled.
                    ToggleRaceLog.Enabled = true;
                    ToggleCBFTPLog.Enabled = true;
                    ToggleApplicationLog.Enabled = true;
                    ToggleRaceLog.Cursor = disableRaceLog ? Cursors.No : Cursors.Hand;
                    ToggleCBFTPLog.Cursor = disableCbftpLog ? Cursors.No : Cursors.Hand;
                    ToggleApplicationLog.Cursor = disableAppLog ? Cursors.No : Cursors.Hand;

                    LogManager.Info($"Settings loaded - App: {appName ?? "RaceTrader"}, Debug: {debugEnabled}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading application settings: {ex.Message}");
            }
        }

        private void AttachLogVisibilityHandlers()
        {
            if (applicationLog != null)
            {
                applicationLog.VisibleChanged += (s, e) =>
                {
                    if (_dockLogsToMain) PositionLogForms();
                    UpdateLogButtonStates();
                };
                applicationLog.FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        applicationLog.Hide();
                    }
                };
            }

            if (raceLog != null)
            {
                raceLog.VisibleChanged += (s, e) =>
                {
                    if (_dockLogsToMain) PositionLogForms();
                    UpdateLogButtonStates();
                };
                raceLog.FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        raceLog.Hide();
                    }
                };
            }

            if (cbftpLog != null)
            {
                cbftpLog.VisibleChanged += (s, e) =>
                {
                    if (_dockLogsToMain) PositionLogForms();
                    UpdateLogButtonStates();
                };
                cbftpLog.FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        cbftpLog.Hide();
                    }
                };
            }

            if (logOutput != null)
            {
                logOutput.VisibleChanged += (s, e) =>
                {
                    if (_dockLogsToMain) PositionLogForms();
                    UpdateLogButtonStates();
                };
                logOutput.FormClosing += (s, e) =>
                {
                    if (e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        logOutput.Hide();
                    }
                };
            }
        }
        private void ToggleRaceLog_Click(object sender, EventArgs e)
        {
            if (LogManager.DisableRaceLog) return;

            if (raceLog != null && !raceLog.IsDisposed)
            {
                if (raceLog.Visible)
                {
                    raceLog.Hide();
                    SetDashboardToggleButton(ToggleRaceLog, false);
                    ToggleRaceLog.Text = "Race (Off)";
                }
                else
                {
                    raceLog.Show();
                    SetDashboardToggleButton(ToggleRaceLog, true);
                    ToggleRaceLog.Text = "Race Log";
                }
            }
        }
        private void AdjustLogWindowHeightForScreen()
        {
            if (_dockLogsToMain) PositionLogForms();
        }    


        private void PositionLogForms()
        {
            // Prevent recursion from Move/Resize/VisibleChanged firing while we reposition
            if (_isPositioningLogs)
                return;

            _isPositioningLogs = true;
            try
            {
                int spacing = 8;
                int minLogHeight = 80; // minimal height for logs so they don't collapse completely

                // Screen work area
                Rectangle wa = Screen.FromControl(this).WorkingArea;

                // Main window is the anchor – DO NOT CHANGE ITS LOCATION HERE
                int mainX = this.Location.X;
                int mainY = this.Location.Y;
                int mainWidth = this.Width;
                int mainHeight = this.Height;
                const int logFrameCompensation = 8;
                int logX = mainX - logFrameCompensation;
                int logWidth = mainWidth + (logFrameCompensation * 2);

                // Base (ideal) heights
                int appBaseHeight = 250;
                int raceBaseHeight = 400;
                int cbftpBaseHeight = 300;
                int ircBaseHeight = 250;

                bool appVisible = applicationLog != null && !applicationLog.IsDisposed && applicationLog.Visible;
                bool raceVisible = raceLog != null && !raceLog.IsDisposed && raceLog.Visible;
                bool cbftpVisible = cbftpLog != null && !cbftpLog.IsDisposed && cbftpLog.Visible;
                bool ircVisible = logOutput != null && !logOutput.IsDisposed && logOutput.Visible;

                // ------------------ TOP: Application log ABOVE main ----------------------
                int appHeight = 0;
                if (appVisible)
                {
                    // Free space above main window
                    int topAvailable = mainY - wa.Top - spacing;

                    if (topAvailable <= 0)
                    {
                        // No space above – make it as small as possible or basically hidden
                        appHeight = Math.Max(0, topAvailable);
                    }
                    else if (appBaseHeight <= topAvailable)
                    {
                        appHeight = appBaseHeight;
                    }
                    else
                    {
                        // Shrink to fit above, but keep a reasonable minimum
                        appHeight = Math.Max(Math.Min(appBaseHeight, topAvailable), Math.Min(minLogHeight, topAvailable));
                    }
                }

                
                // What space do we have under the main window?
                int bottomAvailable = wa.Bottom - (mainY + mainHeight) - spacing;
                if (bottomAvailable < 0) bottomAvailable = 0;

                // Collect visible logs under main with their base heights
                var bottomLogs = new List<(Form Form, int BaseHeight)>();
                if (raceVisible) bottomLogs.Add((raceLog, raceBaseHeight));
                if (cbftpVisible) bottomLogs.Add((cbftpLog, cbftpBaseHeight));
                if (ircVisible) bottomLogs.Add((logOutput, ircBaseHeight));

                var bottomHeights = new Dictionary<Form, int>();

                if (bottomLogs.Count > 0)
                {
                    int totalBase = bottomLogs.Sum(l => l.BaseHeight);
                    int totalSpacing = spacing * (bottomLogs.Count - 1);

                    if (totalBase + totalSpacing <= bottomAvailable || bottomAvailable == 0)
                    {
                        // Either everything fits as-is, or there is literally no space (bottomAvailable==0)
                        foreach (var (form, baseH) in bottomLogs)
                        {
                            bottomHeights[form] = (bottomAvailable == 0) ? 0 : baseH;
                        }
                    }
                    else
                    {
                        // Need to shrink logs to fit into bottomAvailable
                        double factor = (double)(bottomAvailable - totalSpacing) / totalBase;
                        if (factor < 0) factor = 0;

                        // First pass: scaled heights with a minimum
                        foreach (var (form, baseH) in bottomLogs)
                        {
                            int h = (int)(baseH * factor);
                            h = Math.Max(minLogHeight, h);
                            bottomHeights[form] = h;
                        }

                        // Ensure we don't exceed bottomAvailable due to minLogHeight / rounding
                        int sumHeights = bottomHeights.Values.Sum();
                        if (sumHeights + totalSpacing > bottomAvailable && sumHeights > 0)
                        {
                            double factor2 = (double)(bottomAvailable - totalSpacing) / sumHeights;
                            if (factor2 < 0) factor2 = 0;

                            foreach (var key in bottomHeights.Keys.ToList())
                            {
                                int h = (int)(bottomHeights[key] * factor2);
                                bottomHeights[key] = Math.Max(30, h); // don't go too tiny
                            }
                        }
                    }
                }
                               
                // App log above main
                if (appVisible && appHeight > 0)
                {
                    applicationLog.StartPosition = FormStartPosition.Manual;
                    applicationLog.Size = new Size(logWidth, appHeight);
                    applicationLog.Location = new Point(logX, mainY - spacing - appHeight);
                }

                // Logs below main – we start from bottom edge of main
                int currentY = mainY + mainHeight + spacing;

                if (raceVisible && bottomHeights.TryGetValue(raceLog, out int raceHeight) && raceHeight > 0)
                {
                    raceLog.StartPosition = FormStartPosition.Manual;
                    raceLog.Size = new Size(logWidth, raceHeight);
                    raceLog.Location = new Point(logX, currentY);
                    currentY += raceHeight + spacing;
                }

                if (cbftpVisible && bottomHeights.TryGetValue(cbftpLog, out int cbftpHeight) && cbftpHeight > 0)
                {
                    cbftpLog.StartPosition = FormStartPosition.Manual;
                    cbftpLog.Size = new Size(logWidth, cbftpHeight);
                    cbftpLog.Location = new Point(logX, currentY);
                    currentY += cbftpHeight + spacing;
                }

                if (ircVisible && bottomHeights.TryGetValue(logOutput, out int ircHeight) && ircHeight > 0)
                {
                    logOutput.StartPosition = FormStartPosition.Manual;
                    logOutput.Size = new Size(logWidth, ircHeight);
                    logOutput.Location = new Point(logX, currentY);
                }
            }
            finally
            {
                _isPositioningLogs = false;
            }
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    if (_dockLogsToMain)
                    {
                        AdjustLogWindowHeightForScreen();
                        PositionLogForms();
                    }
                }));
                return;
            }

            if (_dockLogsToMain)
            {
                AdjustLogWindowHeightForScreen();
                PositionLogForms();
            }
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    if (e.Mode == PowerModes.Resume && _dockLogsToMain)
                    {
                        AdjustLogWindowHeightForScreen();
                        PositionLogForms();
                        ForceConsoleRedraw();
                    }
                }));
                return;
            }

            if (e.Mode == PowerModes.Resume && _dockLogsToMain)
            {
                AdjustLogWindowHeightForScreen();
                PositionLogForms();
                ForceConsoleRedraw();
            }
        }
        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);
        //    ForceConsoleRedraw();
        //}

        private void ForceConsoleRedraw()
        {
            if (logOutput != null && !logOutput.IsDisposed)
            {
                logOutput.BackColor = ThemeManager.Colors.BackgroundDarkest;
                logOutput.ForeColor = ThemeManager.Colors.Foreground;
                logOutput.Invalidate();
                logOutput.Refresh();
            }

            if (applicationLog != null && !applicationLog.IsDisposed)
            {
                applicationLog.BackColor = ThemeManager.Colors.BackgroundDarkest;
                applicationLog.ForeColor = ThemeManager.Colors.Foreground;
                applicationLog.Invalidate();
                applicationLog.Refresh();
            }
        }

        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    base.OnFormClosed(e);
        //    SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        //    SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
        //}


        public void LoadConfigIntoDropdown()
        {
            EditDropdown_cbftp_comboBox.Items.Clear();
            string filePath = "cbftp/cbftp_config.json";

            // ENSURE CONFIG EXISTS BEFORE LOADING
            try
            {
                AddCbftp.EnsureConfigExists();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to ensure config exists: {ex.Message}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var configData = JsonConvert.DeserializeObject<Config>(jsonContent) ?? new Config();

                foreach (var server in configData.CbftpServers)
                {
                    // Display server.Name instead of server.Id
                    // Fallback to Id if Name is null

                    string displayName = server.Name ?? server.Id; 

                    if (!EditDropdown_cbftp_comboBox.Items.Contains(displayName))
                    {
                        EditDropdown_cbftp_comboBox.Items.Add(displayName);
                    }
                }

                LogManager.Success($"Configuration file '{filePath}' loaded successfully.");

                // Keep the racer's in-memory server list in sync — otherwise added/edited
                // cbftp servers are only picked up after an application restart.
                CbftpRacer.ReloadConfiguration();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error loading configuration file '{filePath}': {ex.Message}");
            }
        }

        private void AppendOutput(string message, System.Drawing.Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendOutput(message, color)));
                return;
            }

            if (logOutput != null && !logOutput.IsDisposed)
            {
                logOutput.AppendLog(message, color);
            }
        }

        private void EditDropdown_cbftp_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EditDropdown_cbftp_comboBox.SelectedItem != null)
            {
                string selectedCbftp = EditDropdown_cbftp_comboBox.SelectedItem.ToString();
                LogManager.Info($"Editing configuration for: {selectedCbftp}");
                EditCbftpConfig(selectedCbftp);
            }
        }

        private void EditCbftpConfig(string cbftpName)  
        {
            string filePath = "cbftp/cbftp_config.json";

            try
            {
                AddCbftp.EnsureConfigExists();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Failed to create configuration directory/file");
                return;
            }

            try
            {
                var jsonData = File.ReadAllText(filePath);
                var configData = JsonConvert.DeserializeObject<Config>(jsonData);

                if (configData?.CbftpServers != null)
                {
                    // Find by Name instead of Id
                    var server = configData.CbftpServers.FirstOrDefault(s =>
                        (s.Name ?? s.Id) == cbftpName);

                    if (server != null)
                    {
                        AddCbftp addCbftpForm = new AddCbftp();
                        addCbftpForm.SetCbftpConfig(server.Id, server.Host, server.Port,
                            server.Password, server.Profile, server.Name);

                        addCbftpForm.OnServerDeleted += () =>
                        {
                            LoadConfigIntoDropdown();
                        };

                        addCbftpForm.ShowDialog();

                        if (addCbftpForm.DialogResult == DialogResult.OK)
                        {
                            server.Name = addCbftpForm.ServerName;
                            server.Host = addCbftpForm.Host;
                            server.Port = addCbftpForm.Port;
                            server.Password = addCbftpForm.Password;
                            server.Profile = addCbftpForm.Profile;

                            AtomicFile.WriteAllText(filePath, JsonConvert.SerializeObject(configData, Formatting.Indented));
                            LogManager.Success($"Configuration for '{server.Name}' updated successfully.");
                        }   

                        LoadConfigIntoDropdown();
                    }
                    else
                    {
                        LogManager.Error($"Server '{cbftpName}' not found in the configuration.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error editing configuration for '{cbftpName}': {ex.Message}");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isTraderRunning)
            {
                StopTrader();
            }
            else
            {
                StartTrader();
            }
            UpdateTraderButton();
        }

        private void UpdateTraderButton()
        {
            if (isTraderRunning)
            {
                Enable_Disable_Racer_button.Text = "Stop";
                Enable_Disable_Racer_button.BackColor = ThemeManager.Colors.Danger;
                Enable_Disable_Racer_button.ForeColor = Color.White;
                Enable_Disable_Racer_button.Image = null; // Remove GIF
                if (traderScanner != null) traderScanner.Visible = true;
            }
            else
            {
                Enable_Disable_Racer_button.Text = "Start";
                Enable_Disable_Racer_button.BackColor = ThemeManager.Colors.Success;
                Enable_Disable_Racer_button.ForeColor = ThemeManager.Colors.BackgroundDarkest;
                Enable_Disable_Racer_button.Image = null; // Remove GIF
                if (traderScanner != null) traderScanner.Visible = false;
            }
        }

        private async void StartTrader()
        {
            // Prevent double-start if the button is spam-clicked
            if (isTraderRunning)
            {
                LogManager.Warning("Trader called while trader is already running. Ignoring.");
                return;
            }

            try
            {
                // Check if any sites exist
                if (GetAllSiteFiles().Count == 0)
                {
                    MessageBox.Show("No sites configured!\n\nPlease add at least one site configuration.",
                        "No Sites Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogManager.Error("Cannot start trader: No sites configured");
                    return;
                }

                // Check if any valid sites exist
                var validSites = 0;
                var errors = new List<string>();

                foreach (var item in GetAllSiteFiles())
                {
                    var siteFile = Path.Combine("sites", $"{item.ToString()}.json");
                    string siteName = Path.GetFileNameWithoutExtension(item.ToString());

                    if (!File.Exists(siteFile))
                    {
                        errors.Add($"{siteName}: Configuration file not found");
                        continue;
                    }

                    try
                    {
                        var siteConfig = LoadConfiguration(siteFile);
                        if (siteConfig == null)
                        {
                            errors.Add($"{siteName}: Failed to load configuration");
                            continue;
                        }

                        if (siteConfig.SiteSettings?.DisableSite == true)
                        {
                            LogManager.Info($"Site '{siteName}' is disabled, skipping");
                            continue;
                        }

                        LogManager.Info($"[{siteName}] Enabled IRC sections ({siteConfig.RaceSectionsEnabled.Count}): {string.Join(", ", siteConfig.RaceSectionsEnabled)}");

                        if (MainApp.DebugEnabled)
                        {
                            foreach (var section in siteConfig.Sections)
                            {
                                if (section.Tags != null && section.Tags.Any())
                                {
                                    foreach (var tag in section.Tags)
                                    {
                                        LogManager.Debug($"  [{siteName}] IRC '{section.IrcName}' → CBFTP '{tag.MapCbftpSection}' (trigger: {tag.TriggerRegex})");
                                    }
                                }
                            }
                        }

                        // Global PreBot config validation
                        if (siteConfig.SiteSettings.PreOrSite?.StartsWith("Global PreBot",
                                StringComparison.OrdinalIgnoreCase) == true)
                        {
                            var prebotNameMatch = Regex.Match(siteConfig.SiteSettings.PreOrSite, @"\((.*?)\)");
                            if (!prebotNameMatch.Success)
                            {
                                errors.Add($"{siteName}: Invalid Global PreBot format");
                                continue;
                            }

                            var prebotName = prebotNameMatch.Groups[1].Value.Trim();
                            var prebotFile = Path.Combine("pre_bots", $"{prebotName}.json");

                            if (!File.Exists(prebotFile))
                            {
                                errors.Add($"{siteName}: PreBot '{prebotName}' not found");
                                continue;
                            }

                            var prebotConfig = JsonConvert.DeserializeObject<PreBotConfig>(File.ReadAllText(prebotFile));
                            if (prebotConfig == null)
                            {
                                errors.Add($"{siteName}: PreBot '{prebotName}' config invalid");
                                continue;
                            }

                            if (string.IsNullOrEmpty(prebotConfig.ZncServer?.Host))
                            {
                                errors.Add($"{siteName}: PreBot '{prebotName}' missing IRC host");
                                continue;
                            }

                            if (string.IsNullOrEmpty(prebotConfig.SiteSettings?.BotName))
                            {
                                errors.Add($"{siteName}: PreBot '{prebotName}' missing bot name");
                                continue;
                            }

                            validSites++;
                        }
                        else
                        {
                            // Regular site validation
                            if (string.IsNullOrEmpty(siteConfig.Server?.Host))
                            {
                                errors.Add($"{siteName}: Missing IRC host");
                                continue;
                            }

                            if (string.IsNullOrEmpty(siteConfig.Server?.Username))
                            {
                                errors.Add($"{siteName}: Missing IRC username");
                                continue;
                            }

                            if (string.IsNullOrEmpty(siteConfig.Server?.Password))
                            {
                                errors.Add($"{siteName}: Missing IRC password");
                                continue;
                            }

                            if (string.IsNullOrEmpty(siteConfig.SiteSettings?.BotName))
                            {
                                errors.Add($"{siteName}: Missing bot name");
                                continue;
                            }

                            var channels = new[]
                            {
                siteConfig.SiteSettings.Chan1,
                siteConfig.SiteSettings.Chan2,
                siteConfig.SiteSettings.Chan3
            }.Where(c => !string.IsNullOrEmpty(c)).ToList();

                            if (!channels.Any())
                            {
                                errors.Add($"{siteName}: No IRC channels defined");
                                continue;
                            }

                            validSites++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"{siteName}: {ex.Message}");
                    }
                }

                // Show errors if no valid sites
                if (validSites == 0)
                {
                    var errorMsg = "No valid sites to connect!\n\n";
                    if (errors.Any())
                    {
                        errorMsg += "Configuration errors:\n\n";
                        errorMsg += string.Join("\n", errors.Take(5));
                        if (errors.Count > 5)
                            errorMsg += $"\n\n... and {errors.Count - 5} more errors";
                    }

                    MessageBox.Show(errorMsg, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogManager.Error($"Cannot start trader: {errors.Count} configuration errors found");
                    return;
                }

                LogManager.Success($"Validation passed: {validSites} valid site(s) found");
                LogManager.Info("Starting IRC connections...");

                RaceHelper.LoadAllSiteConfigs();

                // siteConfigs is (re)populated in the loop below; the auto-fill runner is
                // started AFTER the loop — starting it here always passed an empty list,
                // which silently disabled request auto-fill entirely.

                // reset CTS before starting
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                cancellationTokenSource = new CancellationTokenSource();

                // Capture a local — StopTrader disposes/nulls the field, and the Task.Run
                // bodies below may not have started yet when that happens (NRE/ODE).
                var startCts = cancellationTokenSource;

                isTraderRunning = true;

                siteConfigs.Clear(); // rebuilt below; don't keep sites removed since last start

                var siteTasks = new List<Task>();
                var connectedPrebots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in GetAllSiteFiles())
                {
                    var siteFile = Path.Combine("sites", $"{item.ToString()}.json");
                    string siteName = Path.GetFileNameWithoutExtension(item.ToString());

                    if (MainApp.DebugEnabled)
                    {
                        LogManager.Debug($"Reading configuration for Site '{siteName}' from '{siteFile}'");
                    }

                    var siteConfig = LoadConfiguration(siteFile);
                    if (siteConfig == null)
                    {
                        LogManager.Warning($"Skipping Site '{siteName}': Configuration could not be loaded.");
                        continue;
                    }

                    if (siteConfig.SiteSettings == null)
                    {
                        LogManager.Warning($"Skipping Site '{siteName}': No site_settings section in the configuration.");
                        continue;
                    }

                    if (siteConfig.SiteSettings.DisableSite == true)
                    {
                        LogManager.Warning($"Skipping Site '{siteName}': Site is disabled in the configuration.");
                        continue;
                    }

                    siteConfigs[siteName] = siteConfig;

                    // ---------- Global PreBot handling ----------
                    if (siteConfig.SiteSettings.PreOrSite?.StartsWith("Global PreBot",
                            StringComparison.OrdinalIgnoreCase) == true)
                    {
                        var prebotNameMatch = Regex.Match(siteConfig.SiteSettings.PreOrSite, @"\((.*?)\)");
                        if (!prebotNameMatch.Success || string.IsNullOrWhiteSpace(prebotNameMatch.Groups[1].Value))
                        {
                            LogManager.Error($"Failed to extract PreBot name from 'PreOrSite' value: {siteConfig.SiteSettings.PreOrSite}");
                            continue;
                        }

                        var prebotName = prebotNameMatch.Groups[1].Value.Trim();

                        LogManager.Info($"Site '{siteConfig.SiteSettings.Sitename}' uses Global PreBot '{prebotName}'.");

                        var prebotFile = Path.Combine("pre_bots", $"{prebotName}.json");
                        if (!File.Exists(prebotFile))
                        {
                            LogManager.Error($"PreBot configuration file '{prebotFile}' not found for Global Prebot '{prebotName}'.");
                            continue;
                        }

                        var prebotConfig = JsonConvert.DeserializeObject<PreBotConfig>(File.ReadAllText(prebotFile));
                        if (prebotConfig == null)
                        {
                            LogManager.Error($"Failed to load PreBot configuration for '{prebotName}'.");
                            continue;
                        }

                        // Collect ALL sections from ALL sites using this PreBot
                        var allEnabledSectionsForPrebot = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        var allSectionsConfigForPrebot = new List<Section>();
                        int siteCount = 0;

                        if (MainApp.DebugEnabled)
                        {
                            LogManager.Debug($"Collecting sections from all sites using PreBot '{prebotName}'...");
                        }

                        foreach (var otherSiteItem in GetAllSiteFiles())
                        {
                            var tempSiteFile = Path.Combine("sites", $"{otherSiteItem}.json");
                            if (!File.Exists(tempSiteFile)) continue;

                            try
                            {
                                var tempSiteConfig = LoadConfiguration(tempSiteFile);
                                if (tempSiteConfig == null) continue;
                                if (tempSiteConfig.SiteSettings?.DisableSite == true) continue;

                                // Check if this site uses the same PreBot
                                if (tempSiteConfig.SiteSettings.PreOrSite?.StartsWith("Global PreBot", StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    var tempPrebotNameMatch = Regex.Match(tempSiteConfig.SiteSettings.PreOrSite, @"\((.*?)\)");
                                    if (tempPrebotNameMatch.Success &&
                                        string.Equals(tempPrebotNameMatch.Groups[1].Value.Trim(), prebotName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        siteCount++;

                                        // This site uses the same PreBot - merge its sections
                                        if (tempSiteConfig.RaceSectionsEnabled != null)
                                        {
                                            foreach (var section in tempSiteConfig.RaceSectionsEnabled)
                                            {
                                                allEnabledSectionsForPrebot.Add(section);
                                            }
                                        }

                                        if (tempSiteConfig.Sections != null)
                                        {
                                            foreach (var section in tempSiteConfig.Sections)
                                            {
                                                // Add section config if not already present
                                                if (!allSectionsConfigForPrebot.Any(s =>
                                                    string.Equals(s.IrcName, section.IrcName, StringComparison.OrdinalIgnoreCase)))
                                                {
                                                    allSectionsConfigForPrebot.Add(section);
                                                }
                                            }
                                        }

                                        if (MainApp.DebugEnabled)
                                        {
                                            LogManager.Debug($"  Added sections from site '{tempSiteConfig.SiteSettings.Sitename}'");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"Error processing site '{otherSiteItem}' for PreBot sections: {ex.Message}");
                            }
                        }

                        // Single consolidated log line
                        LogManager.Success($"PreBot '{prebotName}' configured for {siteCount} site(s), monitoring {allEnabledSectionsForPrebot.Count} sections");

                        if (MainApp.DebugEnabled)
                        {
                            var sectionList = string.Join(", ", allEnabledSectionsForPrebot.Take(15));
                            if (allEnabledSectionsForPrebot.Count > 15)
                                sectionList += $" ... and {allEnabledSectionsForPrebot.Count - 15} more";
                            LogManager.Debug($"[{prebotName}] Sections: {sectionList}");
                        }

                        // Create PreBot config with MERGED sections
                        var siteConfigForPrebot = new SiteConfig
                        {
                            Server = new ServerSettings
                            {
                                Host = prebotConfig.ZncServer.Host,
                                Port = prebotConfig.ZncServer.Port,
                                Username = prebotConfig.ZncServer.Username,
                                Password = prebotConfig.ZncServer.Password
                            },
                            SiteSettings = new SiteSettings
                            {
                                Sitename = siteConfig.SiteSettings.Sitename,
                                BotName = prebotConfig.SiteSettings.BotName,
                                Chan1 = prebotConfig.SiteSettings.Channel1,
                                BlowfishKey1 = prebotConfig.SiteSettings.BlowfishKey1,
                                SectionRegexPattern = prebotConfig.SiteSettings.SectionRegex,
                                SectionPrefix = prebotConfig.SiteSettings.SectionPrefix,
                                SectionSuffix = prebotConfig.SiteSettings.SectionSuffix,
                                ReleaseRegexPattern = prebotConfig.SiteSettings.NameRegex,
                                PreOrSite = siteConfig.SiteSettings.PreOrSite
                            },
                            RaceSectionsEnabled = allEnabledSectionsForPrebot.ToList(), // MERGED!
                            Sections = allSectionsConfigForPrebot, // MERGED!
                            GlobalBlacklist = siteConfig.GlobalBlacklist
                        };

                        if (!connectedPrebots.Add(prebotName))
                        {
                            if (MainApp.DebugEnabled)
                            {
                                LogManager.Debug($"Global PreBot '{prebotName}' already connected, skipping");
                            }
                            continue;
                        }

                        var prebotTask = Task.Run(async () =>
                        {
                            try
                            {
                                LogManager.Info($"Connecting to ZNC server for PreBot '{prebotName}'");

                                var prebotClient = new IRCClient(
                                    siteConfigForPrebot,
                                    prebotName,
                                    logOutput,
                                    startCts.Token
                                );

                                raceIrcClients[prebotName] = prebotClient;

                                await prebotClient.ConnectToZNCAsync();
                                LogManager.Success($"Connection established to PreBot '{prebotName}'");
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"Error connecting to PreBot '{prebotName}': {ex.Message}");
                            }
                        }, startCts.Token);

                        siteTasks.Add(prebotTask);
                    }
                    else
                    {
                        // ---------- Regular site handling ----------
                        if (string.IsNullOrEmpty(siteConfig.Server?.Host))
                        {
                            LogManager.Warning($"Not connecting to IRC. Skipping Site'{siteName}': Missing IRC host.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(siteConfig.Server?.Username))
                        {
                            LogManager.Warning($"Not connecting to IRC. Skipping Site '{siteName}': Missing IRC username.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(siteConfig.Server?.Password))
                        {
                            LogManager.Warning($"Not connecting to IRC. Skipping Site '{siteName}': Missing IRC password.");
                            continue;
                        }

                        if (string.IsNullOrEmpty(siteConfig.SiteSettings?.BotName))
                        {
                            LogManager.Warning($"Not connecting to IRC. Skipping Site '{siteName}': Missing bot name.");
                            continue;
                        }

                        var channels = new[]
                        {
            siteConfig.SiteSettings.Chan1,
            siteConfig.SiteSettings.Chan2,
            siteConfig.SiteSettings.Chan3
        }.Where(c => !string.IsNullOrEmpty(c)).ToList();

                        if (!channels.Any())
                        {
                            LogManager.Warning($"Not connecting to IRC. Skipping Site '{siteName}': No IRC channels defined.");
                            continue;
                        }

                        LogManager.Info($"Starting connection to ZNC server for Site '{siteName}'");

                        var siteTask = Task.Run(async () =>
                        {
                            var ircClient = new IRCClient(
                                siteConfig,
                                siteName,
                                logOutput,
                                startCts.Token
                            );

                            raceIrcClients[siteName] = ircClient;

                            try
                            {
                                await ircClient.ConnectToZNCAsync();
                                LogManager.Success($"Successfully connected and processed Site '{siteName}'");
                            }
                            catch (Exception ex)
                            {
                                LogManager.Error($"Error connecting to IRC for Site '{siteName}': {ex.Message}");
                            }
                        }, startCts.Token);

                        siteTasks.Add(siteTask);
                    }
                }

                // Start request auto-fill now that siteConfigs is populated
                // (WhenAll below blocks until the trader stops).
                RequestAutoFillRunner.StartForAllSites(siteConfigs.Values);

                try
                {
                    // WhenAll completes only when every connection loop has exited — i.e.
                    // when the trader is stopping, NOT when it finished connecting.
                    await Task.WhenAll(siteTasks);
                    LogManager.Info("All trader connection tasks have ended.");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error in one or more IRC connections: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Fatal error starting trader: {ex.Message}");
                MessageBox.Show($"Failed to start trader:\n\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // The connection tasks have all exited. Either the user pressed Stop
                // (StopTrader already ran → isTraderRunning=false) or every connection
                // dropped on its own. In the latter case isTraderRunning is still true and
                // the button still says "Stop" while nothing is running — reconcile state,
                // tear down the dead clients, and refresh the button on the UI thread.
                Action reconcile = () =>
                {
                    if (isTraderRunning)
                    {
                        LogManager.Warning("All trader connections ended on their own; stopping trader.");
                        StopTrader(); // clears clients, cancels/disposes CTS, sets flag false
                    }
                    UpdateTraderButton();
                };

                if (IsHandleCreated && !IsDisposed)
                {
                    try { BeginInvoke(reconcile); }
                    catch { isTraderRunning = false; }
                }
                else
                {
                    isTraderRunning = false;
                }
            }
        }

        private void StopTrader()
        {
            // If already stopped, avoid doing work twice
            if (!isTraderRunning && raceIrcClients.Count == 0)
            {
                LogManager.Info("Trader already stopped.");
                return;
            }

            isTraderRunning = false;

            RequestAutoFillRunner.Stop();

            // Cancel the race/trader token so all ConnectToZNCAsync loops can exit
            try
            {
                cancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error cancelling trader token: {ex.Message}");
            }

            // Disconnect all race IRC clients (the ones created in StartTrader)
            foreach (var client in raceIrcClients.Values.ToList())
            {
                try
                {
                    client.Disconnect();
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error disconnecting race IRC client: {ex.Message}");
                }
            }

            raceIrcClients.Clear();

            // Join any legacy threads if you still use them
            foreach (var thread in ircThreads.ToList())
            {
                try
                {
                    if (thread != null && thread.IsAlive)
                    {
                        thread.Join(1000);
                    }
                }
                catch
                {
                    // ignore
                }
            }
            ircThreads.Clear();

            // Dispose the CTS so we don't reuse a cancelled token next time
            try
            {
                cancellationTokenSource?.Dispose();
            }
            catch
            {
                // ignore
            }

            cancellationTokenSource = null;

            LogManager.Warning("Trader stopped.");
        }

        private void StopChatConnections()
        {
            // Turn off user tracking first (to stop join/part spam)
            foreach (var client in chatIrcClients.Values.ToList())
            {
                try
                {
                    client.SetUserTrackingEnabled(false);
                }
                catch
                {
                    // ignore
                }

                try
                {
                    client.Disconnect();
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error disconnecting chat IRC client: {ex.Message}");
                }
            }

            chatIrcClients.Clear();

            try
            {
                chatCancellationTokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error cancelling chat token: {ex.Message}");
            }

            try
            {
                chatCancellationTokenSource?.Dispose();
            }
            catch
            {
                // ignore
            }

            chatCancellationTokenSource = null;

            LogManager.Info("Chat IRC connections stopped.");
        }

        private SiteConfig LoadConfiguration(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    LogManager.Success($"Configuration file '{filePath}' loaded successfully.");
                    return JsonConvert.DeserializeObject<SiteConfig>(jsonContent);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error loading configuration file '{filePath}': {ex.Message}");
                    return null;
                }
            }
            else
            {
                LogManager.Error($"Configuration file '{filePath}' not found.");
                return null;
            }
        }


        private void Edit_sites_button_Click_1(object sender, EventArgs e)
        {
            // Open Add/Edit form - site selection will be done inside the form
            AddSite form2 = new AddSite(null); // Pass null to trigger edit mode with dropdown
            form2.ShowDialog();
            // Refresh dropdown after closing            
        }


        private void Add_Cbftp_button1_Click(object sender, EventArgs e)
        {
            AddCbftp addCbftpForm = new AddCbftp();
            //addCbftpForm.SetCbftpConfig(null, "", "", "", "", "");

            var result = addCbftpForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    AddCbftp.EnsureConfigExists();
                }
                catch (Exception ex)
                {
                    DialogHelper.ShowError(ex, "Failed to create configuration directory/file");
                    return;
                }

                string filePath = "cbftp/cbftp_config.json";
                Config configData;

                try
                {
                    string jsonContent = File.ReadAllText(filePath);
                    configData = JsonConvert.DeserializeObject<Config>(jsonContent) ?? new Config();
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error reading configuration file: {ex.Message}");
                    return;
                }

                string serverName = addCbftpForm.ServerName; 
                string host = addCbftpForm.Host?.Trim();
                string port = addCbftpForm.Port?.Trim();
                string password = addCbftpForm.Password?.Trim();
                string profile = addCbftpForm.Profile;

                if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port) ||
                    string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(serverName))
                {
                    DialogHelper.ShowError("Server Name, Host, Port, and Password fields must not be empty.");
                    return;
                }

                if (configData.CbftpServers.Any(s => s.Host == host && s.Port == port))
                {
                    DialogHelper.ShowError($"A server with Host '{host}' and Port '{port}' already exists.");
                    return;
                }

                //  Use server name for ID
                int nextId = configData.CbftpServers
                    .Where(s => s.Id.StartsWith($"{serverName}_"))
                    .Select(s => {
                        var parts = s.Id.Split('_');
                        return parts.Length > 1 && int.TryParse(parts[1], out int num) ? num : 0;
                    })
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                string newSectionName = nextId == 1 ? serverName : $"{serverName}_{nextId}";

                var newServer = new CbftpServer
                {
                    Id = newSectionName,
                    Name = serverName,  
                    Host = host,
                    Port = port,
                    Password = password,
                    Profile = profile
                };

                configData.CbftpServers.Add(newServer);

                try
                {
                    string updatedJsonContent = JsonConvert.SerializeObject(configData, Formatting.Indented);
                    AtomicFile.WriteAllText(filePath, updatedJsonContent);
                    LogManager.Success($"New configuration '{newServer.Name}' added successfully.");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Failed to save configuration: {ex.Message}");
                }

                LoadConfigIntoDropdown();
            }
        }

        //private void SaveNewSite(Dictionary<string, dynamic> siteData)
        //{
        //    try
        //    {
        //        LogMessage("Entering SaveNewSite");

        //        if (siteData == null || siteData.Count == 0)
        //        {
        //            throw new ArgumentNullException(nameof(siteData), "The siteData dictionary is null or empty.");
        //        }

        //        LogMessage("Site data received in SaveNewSite:");
        //        foreach (var kvp in siteData)
        //        {
        //            LogMessage($"Key: {kvp.Key}, Value: {(kvp.Value == null ? "NULL or EMPTY" : kvp.Value.ToString())}");
        //        }

        //        if (!siteData.ContainsKey("sitename") || string.IsNullOrEmpty(siteData["sitename"]?.ToString()))
        //        {
        //            throw new KeyNotFoundException("The 'sitename' key is missing or has no value.");
        //        }

        //        if (!siteData.ContainsKey("host") || string.IsNullOrEmpty(siteData["host"]?.ToString()))
        //        {
        //            throw new KeyNotFoundException("The 'host' key is missing or has no value.");
        //        }

        //        string siteName = siteData["sitename"];
        //        string filePath = Path.Combine("sites", $"{siteName}.json");
        //        LogMessage($"File path for the new JSON file: {filePath}");

        //        var siteConfig = new
        //        {
        //            server = new
        //            {
        //                host = siteData["host"],
        //                port = siteData.ContainsKey("port") ? siteData["port"] : "",
        //                username = siteData.ContainsKey("username") ? siteData["username"] : "",
        //                password = siteData.ContainsKey("password") ? siteData["password"] : ""
        //            },
        //            site_settings = new
        //            {
        //                sitename = siteName,
        //                bot_name = siteData.ContainsKey("bot_name") ? siteData["bot_name"] : "",
        //                new_regex_pattern = siteData.ContainsKey("new_regex_pattern") ? siteData["new_regex_pattern"] : "",
        //                pre_announce = siteData.ContainsKey("pre_announce") ? siteData["pre_announce"] : "Site",
        //                ignore_words = siteData.ContainsKey("ignore_words") ? siteData["ignore_words"] : "",
        //                release_regex_pattern = siteData.ContainsKey("release_regex_pattern") ? siteData["release_regex_pattern"] : "",
        //                section_regex_pattern = siteData.ContainsKey("section_regex_pattern") ? siteData["section_regex_pattern"] : "",
        //                section_prefix = siteData.ContainsKey("section_prefix") ? siteData["section_prefix"] : "",
        //                section_suffix = siteData.ContainsKey("section_suffix") ? siteData["section_suffix"] : "",
        //                dl_only_site = siteData.ContainsKey("dl_only_site") ? (bool.TryParse(siteData["dl_only_site"]?.ToString(), out bool dlOnly) ? dlOnly : false) : false,
        //                channels = GetChannelData(siteData),
        //                blowfish_keys = GetBlowfishKeyData(siteData)
        //            }
        //        };

        //        File.WriteAllText(filePath, JsonConvert.SerializeObject(siteConfig, Formatting.Indented));
        //        LogMessage($"Successfully wrote JSON data to file: {filePath}");
        //    }
        //    catch (KeyNotFoundException keyEx)
        //    {
        //        LogMessage($"KeyNotFoundException: {keyEx.Message}");
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage($"Error in SaveNewSite: {ex.Message}");
        //        throw;
        //    }
        //}

        //private Dictionary<string, string> GetChannelData(Dictionary<string, dynamic> siteData)
        //{
        //    var channels = new Dictionary<string, string>();
        //    int index = 1;

        //    while (siteData.ContainsKey($"chan{index}"))
        //    {
        //        channels[$"chan{index}"] = siteData[$"chan{index}"]?.ToString() ?? "";
        //        index++;
        //    }

        //    return channels;
        //}

        //private Dictionary<string, string> GetBlowfishKeyData(Dictionary<string, dynamic> siteData)
        //{
        //    var blowfishKeys = new Dictionary<string, string>();
        //    int index = 1;

        //    while (siteData.ContainsKey($"blowfish_key{index}"))
        //    {
        //        blowfishKeys[$"blowfish_key{index}"] = siteData[$"blowfish_key{index}"]?.ToString() ?? "";
        //        index++;
        //    }

        //    return blowfishKeys;
        //}

        //private void LogMessage(string message)
        //{
        //    string logFilePath = "debug.log";
        //    using (StreamWriter writer = new StreamWriter(logFilePath, true))
        //    {
        //        writer.WriteLine($"{DateTime.Now}: {message}");
        //    }
        //}

        private void exitButton_Click_1(object sender, EventArgs e)
        {
            var result = DialogHelper.ShowConfirmation("Are you sure you want to exit the application?");
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddCbftpSection addEditSectionForm = new AddCbftpSection();
            addEditSectionForm.ShowDialog();
        }

        private void add_sites_button_Click(object sender, EventArgs e)
        {
            string defaultFileName = "new_site.json";
            string defaultFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", defaultFileName);

            // Create default template if it doesn't exist
            if (!File.Exists(defaultFilePath))
            {
                var defaultSite = new SiteConfig
                {
                    Server = new ServerSettings
                    {
                        Host = "irc.example.com",
                        Port = 6667,
                        Username = "username",
                        Password = SecureConfig.Encrypt("password")
                    },
                    SiteSettings = new SiteSettings
                    {
                        Sitename = "NewSite",
                        PreOrSite = "SiteBot",
                        BotName = "Bot",
                        Chan1 = "#channel1",
                        BlowfishKey1 = SecureConfig.Encrypt("BlowfishKey1"),
                        Chan2 = "#channel2",
                        BlowfishKey2 = SecureConfig.Encrypt("BlowfishKey2"),
                        Chan3 = "#channel3",
                        BlowfishKey3 = SecureConfig.Encrypt("BlowfishKey3"),
                        NewRegexPattern = @"\bNEW\b",
                        IgnoreWords = "LEADER,RELEASE",
                        ReleaseRegexPattern = @"\].s*(.*?)\s",
                        SectionRegexPattern = @"\[(.*?)\]",
                        SectionPrefix = "[",
                        SectionSuffix = "]",
                        DlOnlySite = false,
                        DisableSite = false
                    },
                    Sections = new List<Section>(),
                    RaceSectionsEnabled = new List<string>(),

                    GlobalBlacklist = new List<string>()
                };

                string jsonContent = JsonConvert.SerializeObject(defaultSite, Formatting.Indented);
                AtomicFile.WriteAllText(defaultFilePath, jsonContent);
            }

            // Open the AddSite form
            AddSite form2 = new AddSite(defaultFileName);
            form2.ShowDialog();

            // Clean up template file if user saved with a different name
            if (File.Exists(defaultFilePath))
            {
                var config = JsonConvert.DeserializeObject<SiteConfig>(File.ReadAllText(defaultFilePath));
                string actualSiteName = config?.SiteSettings?.Sitename;

                if (!string.IsNullOrEmpty(actualSiteName) && actualSiteName != "NewSite")
                {
                    string actualFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites", actualSiteName + ".json");

                    if (File.Exists(actualFilePath))
                    {
                        // Clean up the template file since user saved with custom name
                        File.Delete(defaultFilePath);
                    }
                }
            }
        }

        private void Sync_From_Cbftp_Button_Click(object sender, EventArgs e)
        {
            try
            {
                CbftpSyncForm syncForm = new CbftpSyncForm();
                var result = syncForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // Reload sites dropdown after successful import
                    LogManager.Success("Sites successfully imported from CBFTP!");
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening CBFTP sync form");
                DialogHelper.ShowError($"Error opening sync form:\n{ex.Message}");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new SettingsForm())
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        LogManager.Success("Settings saved");

                        // reload settings to apply changes immediately
                        LoadApplicationSettings();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening settings");
                MessageBox.Show($"Error opening settings:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            try
            {
                using (var form = new TestReleaseForm())
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening Test Release form");
                MessageBox.Show($"Error opening Test Release form:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Help_button_Click_1(object sender, EventArgs e)
        {

            try
            {
                ToggleWindow(ref helpForm, () => new HelpForm());
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening help");
                MessageBox.Show($"Error opening help:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Changelog_button_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleWindow(ref changelogForm, () => new ChangelogForm());
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening changelog");
                MessageBox.Show($"Error opening changelog:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Starts IRC connections for chat only (without processing releases)
        /// </summary>
        private async Task StartIrcConnectionsOnly()
        {
            try
            {
                if (GetAllSiteFiles().Count == 0)
                {
                    LogManager.Warning("No sites configured for chat");
                    return;
                }

                int validSites = 0;

                foreach (var item in GetAllSiteFiles())
                {
                    var siteFile = Path.Combine("sites", $"{item}.json");

                    if (!File.Exists(siteFile))
                        continue;

                    try
                    {
                        var siteConfig = LoadConfiguration(siteFile);
                        if (siteConfig == null)
                            continue;

                        if (siteConfig.SiteSettings?.DisableSite == true)
                            continue;

                        if (string.IsNullOrWhiteSpace(siteConfig.Server?.Host))
                            continue;

                        if (string.IsNullOrWhiteSpace(siteConfig.Server?.Username))
                            continue;

                        validSites++;
                    }
                    catch
                    {
                        // ignore validation errors here
                        continue;
                    }
                }

                if (validSites == 0)
                {
                    LogManager.Warning("No valid IRC connections found for chat");
                    return;
                }

                LogManager.Info($"Connecting to {validSites} IRC server(s) for chat...");

                // reset chat CTS
                chatCancellationTokenSource?.Cancel();
                chatCancellationTokenSource?.Dispose();
                chatCancellationTokenSource = new CancellationTokenSource();

                // Local capture — the field can be disposed/nulled by StopChatClients
                // before the Task.Run bodies below have started.
                var chatCts = chatCancellationTokenSource;

                var siteTasks = new List<Task>();

                foreach (var item in GetAllSiteFiles())
                {
                    var siteFile = Path.Combine("sites", $"{item}.json");
                    string siteName = Path.GetFileNameWithoutExtension(item.ToString());

                    if (!File.Exists(siteFile))
                        continue;

                    var siteConfig = LoadConfiguration(siteFile);
                    if (siteConfig == null)
                        continue;

                    if (siteConfig.SiteSettings?.DisableSite == true)
                        continue;

                    if (string.IsNullOrWhiteSpace(siteConfig.Server?.Host) ||
                        string.IsNullOrWhiteSpace(siteConfig.Server?.Username))
                    {
                        continue;
                    }

                    var siteTask = Task.Run(async () =>
                    {
                        var chatClient = new ChatIrcClient(
                            siteConfig,
                            siteName,
                            (category, release) => { }, // not used for chat
                            logOutput,
                            chatCts.Token
                        );

                        chatClient.SetChatOnlyMode(true);
                        chatClient.SetTabbedLogOutput(tabbedIrcLog);
                        chatClient.SetUserTrackingEnabled(true);

                        chatIrcClients[siteName] = chatClient;

                        var channels = new List<string>();

                        void AddChannel(string chan)
                        {
                            if (!string.IsNullOrWhiteSpace(chan) && !channels.Contains(chan))
                                channels.Add(chan);
                        }

                        var ss = siteConfig.SiteSettings;
                        if (ss != null)
                        {
                            AddChannel(ss.Chan1);
                            AddChannel(ss.Chan2);
                            AddChannel(ss.Chan3);
                            AddChannel(ss.Chan4);
                            AddChannel(ss.Chan5);
                            AddChannel(ss.Chan6);
                            AddChannel(ss.Chan7);
                            AddChannel(ss.Chan8);
                            AddChannel(ss.Chan9);
                            AddChannel(ss.Chan10);
                            AddChannel(ss.Chan11);
                            AddChannel(ss.Chan12);
                            AddChannel(ss.Chan13);
                            AddChannel(ss.Chan14);
                            AddChannel(ss.Chan15);
                            AddChannel(ss.Chan16);
                            AddChannel(ss.Chan17);
                            AddChannel(ss.Chan18);
                            AddChannel(ss.Chan19);
                            AddChannel(ss.Chan20);

                            if (ss.ChatKeys != null)
                            {
                                foreach (var key in ss.ChatKeys.Keys)
                                {
                                    if (!string.IsNullOrWhiteSpace(key) &&
                                        key.StartsWith("#") &&
                                        !channels.Contains(key))
                                    {
                                        channels.Add(key);
                                    }
                                }
                            }
                        }

                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                foreach (var channel in channels)
                                {
                                    tabbedIrcLog?.GetOrCreateChannelTab(siteName, channel);
                                    tabbedIrcLog?.AppendChannelMessage(
                                        siteName,
                                        channel,
                                        $"*** Connecting to {channel}...",
                                        Color.Gray
                                    );
                                }
                            }));
                        }
                        catch
                        {
                            return;
                        }

                        try
                        {
                            await chatClient.ConnectToZNCAsync();

                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    foreach (var channel in channels)
                                    {
                                        tabbedIrcLog?.AppendChannelMessage(
                                            siteName,
                                            channel,
                                            $"*** Connected to {channel}",
                                            Color.Green
                                        );
                                    }
                                }));
                            }
                            catch { }

                            LogManager.Success($"Chat connected to '{siteName}'");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Error($"Error connecting to '{siteName}': {ex.Message}");

                            try
                            {
                                this.Invoke(new Action(() =>
                                {
                                    foreach (var channel in channels)
                                    {
                                        tabbedIrcLog?.AppendChannelMessage(
                                            siteName,
                                            channel,
                                            $"*** Connection failed: {ex.Message}",
                                            Color.Red
                                        );
                                    }
                                }));
                            }
                            catch { }
                        }
                    }, chatCts.Token);

                    siteTasks.Add(siteTask);
                }

                if (siteTasks.Any())
                {
                    await Task.WhenAll(siteTasks);
                    LogManager.Success("Chat connections established");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error starting chat connections: {ex.Message}");
            }
        }


        private async void OpenTabbedIRC_Click_1(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OpenTabbedIRC_Click_1(sender, e)));
                return;
            }

            // Toggle: if the chat window is already open, hide it (preserves connections).
            if (tabbedIrcLog != null && !tabbedIrcLog.IsDisposed && tabbedIrcLog.Visible)
            {
                tabbedIrcLog.Hide();
                return;
            }

            // Ensure the tabbed IRC window exists
            if (tabbedIrcLog == null || tabbedIrcLog.IsDisposed)
            {
                InitializeLogForms();
            }

            tabbedIrcLog.Show();
            tabbedIrcLog.BringToFront();

            // Update all existing CHAT clients with the tabbed log
            foreach (var client in chatIrcClients.Values)
            {
                try
                {
                    client.SetTabbedLogOutput(tabbedIrcLog);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error setting tabbed log output (chat): {ex.Message}");
                }
            }

            // Enable user tracking for existing chat connections
            foreach (var client in chatIrcClients.Values)
            {
                try
                {
                    client.SetUserTrackingEnabled(true);
                }
                catch (Exception ex)
                {
                    LogManager.Error($"Error enabling user tracking (chat): {ex.Message}");
                }
            }

            // AUTO-CONNECT: Start chat connections when Chat is opened
            if (chatIrcClients.Count == 0)
            {
                LogManager.Info("Chat window opened - starting chat IRC connections...");
                await StartIrcConnectionsOnly();
            }
        }




        private void MainApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Stop user tracking for chat clients
                foreach (var client in chatIrcClients.Values)
                {
                    try
                    {
                        client.SetUserTrackingEnabled(false);
                    }
                    catch
                    {
                        // ignore
                    }
                }

                // Stop trader + chat IRC cleanly
                StopTrader();
                StopChatConnections();

                // Small pause to flush logs
                Thread.Sleep(100);

                // Close tabbed IRC log
                try
                {
                    if (tabbedIrcLog != null && !tabbedIrcLog.IsDisposed)
                    {
                        tabbedIrcLog.Close();
                    }
                }
                catch { }

                // Close other log windows
                try
                {
                    if (applicationLog != null && !applicationLog.IsDisposed)
                    {
                        applicationLog.Close();
                    }
                }
                catch { }

                try
                {
                    if (cbftpLog != null && !cbftpLog.IsDisposed)
                    {
                        cbftpLog.Close();
                    }
                }
                catch { }

                try
                {
                    if (logOutput != null && !logOutput.IsDisposed)
                    {
                        logOutput.Close();
                    }
                }
                catch { }

                try
                {
                    if (raceLog != null && !raceLog.IsDisposed)
                    {
                        raceLog.Close();
                    }
                }
                catch { }

                // Unsubscribe from system events
                SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
            }
        }

        private List<string> GetAllSiteFiles()
        {
            var siteFiles = new List<string>();
            var sitesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sites");

            if (!Directory.Exists(sitesDir))
                return siteFiles;

            foreach (var file in Directory.GetFiles(sitesDir, "*.json"))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (!fileName.Equals("default_site", StringComparison.OrdinalIgnoreCase) &&
                    !fileName.Equals("new_site", StringComparison.OrdinalIgnoreCase))
                {
                    siteFiles.Add(fileName);
                }
            }

            return siteFiles;
        }




        private void Add_cbftp_site_Click(object sender, EventArgs e)
        {
            using (var form = new CbftpAddSiteForm())
            {
                // show as modal dialog, with this form as owner
                var result = form.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    // refresh your sites combo/list
                    //ReloadSitesFromCbftpOrConfig();
                }
            }
        }

        // EDIT SITE button
        private void Cbftp_Edit_Site_Click(object sender, EventArgs e)
        {
            using (var f = new CbftpAddSiteForm(
                       CbftpAddSiteForm.CbftpSiteFormMode.Edit,
                       null))   // user will pick server inside the form
            {
                f.ShowDialog(this);
            }
        }

        private void blacklist_add_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new GlobalBlacklistForm(this))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        // Reload the blacklist in RaceHelper
                        LoadGlobalBlacklist();
                        LogManager.Success("Global blacklist updated");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening global blacklist manager");
                MessageBox.Show($"Error opening blacklist manager:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private PreSpreadForm preSpreadForm = null;

        private void Pre_button_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleWindow(ref preSpreadForm, () => new PreSpreadForm());
            }
            catch (Exception ex)
            {
                LogManager.Exception(ex, "Error opening Pre Spread form");
                MessageBox.Show($"Error opening Pre Spread form:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void Prebot_edit_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Open PreBot form - it has its own dropdown to select which PreBot to edit
                PreBot preBotForm = new PreBot(null);
                preBotForm.ShowDialog();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error opening PreBot form: {ex.Message}");
                MessageBox.Show($"Error opening PreBot:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Add_PreBot_button_Click(object sender, EventArgs e)
        {
            string templateFileName = "new_prebot.json";
            string templateFilePath = Path.Combine("pre_bots", templateFileName);

            // Create template if doesn't exist
            if (!File.Exists(templateFilePath))
            {
                Directory.CreateDirectory("pre_bots");

                var defaultPreBot = new PreBotConfig
                {
                    ZncServer = new ZncServerSettings
                    {
                        Host = "irc.example.com",
                        Port = 6667,
                        Username = "username",
                        Password = SecureConfig.Encrypt("password")
                    },
                    SiteSettings = new PreBotSiteSettings
                    {
                        Sitename = "NewPreBot",
                        BotName = "PreBot",
                        Channel1 = "#pre",
                        BlowfishKey1 = SecureConfig.Encrypt("key"),
                        SectionRegex = @"\[(.*?)\]",
                        NameRegex = @"\].s*(.*?)\s"
                    }
                };

                AtomicFile.WriteAllText(templateFilePath, JsonConvert.SerializeObject(defaultPreBot, Formatting.Indented));
            }

            try
            {
                PreBot preBotForm = new PreBot(templateFilePath);
                preBotForm.ShowDialog();

                // Clean up template if saved with different name
                if (File.Exists(templateFilePath))
                {
                    var config = JsonConvert.DeserializeObject<PreBotConfig>(File.ReadAllText(templateFilePath));
                    string actualName = config?.SiteSettings?.Sitename;

                    if (!string.IsNullOrEmpty(actualName) && actualName != "NewPreBot")
                    {
                        string actualFilePath = Path.Combine("pre_bots", actualName + ".json");
                        if (File.Exists(actualFilePath))
                        {
                            File.Delete(templateFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"Error opening new PreBot form: {ex.Message}");
            }
        }

        private async void buttonImportPredb_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable button during import
                buttonImportPredb.Enabled = false;
                buttonImportPredb.Text = "Importing...";

                // Show confirmation
                var result = MessageBox.Show(
                    "Import the latest 100 releases from predb.club?\n\n" +
                    "This will populate the pretime database with recent releases.",
                    "Import Pretimes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    buttonImportPredb.Enabled = true;
                    buttonImportPredb.Text = "Import";
                    return;
                }

                // Import
                int imported = await PreBotManager.ImportFromPredbClubAsync(100);

                // Show result
                if (imported > 0)
                {
                    DialogHelper.ShowSuccess($"Successfully imported {imported} releases from predb.club!");
                }
                else
                {
                    DialogHelper.ShowWarning("No releases were imported. Check the logs for details.");
                }
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex, "Failed to import from predb.club");
            }
            finally
            {
                // Re-enable button
                buttonImportPredb.Enabled = true;
                buttonImportPredb.Text = "Import";
            }
        }


        private void Ftp_button_Click(object sender, EventArgs e)
        {
            try
            {
                ToggleWindow(ref _ftpClientForm, () => new RaceTrader.FtpClientForm());
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to open FTP client: {ex.Message}");
                MessageBox.Show($"Failed to open FTP client:\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

    // Keep all the class definitions (SiteConfig, ServerSettings, etc.)
    public class SiteConfig
    {
        [JsonProperty("server")]
        public ServerSettings Server { get; set; }

        [JsonProperty("site_settings")]
        public SiteSettings SiteSettings { get; set; }

        [JsonProperty("affils")]
        public List<string> Affils { get; set; }

        [JsonProperty("race_sections_enabled")]
        public List<string> RaceSectionsEnabled { get; set; } = new();

        [JsonProperty("global_blacklist")]
        public List<string> GlobalBlacklist { get; set; } = new();

        [JsonProperty("sections")]
        public List<Section> Sections { get; set; } = new();
    }

    public class ServerSettings
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class SiteSettings
    {
        [JsonProperty("chat_keys")]
        public Dictionary<string, string> ChatKeys { get; set; } = new();

        [JsonProperty("pre_announce")]
        public string PreOrSite { get; set; }

        [JsonProperty("sitename")]
        public string Sitename { get; set; }

        [JsonProperty("bot_name")]
        public string BotName { get; set; }

        [JsonProperty("new_regex_pattern")]
        public string NewRegexPattern { get; set; }

        [JsonProperty("section_regex_pattern")]
        public string SectionRegexPattern { get; set; }

        [JsonProperty("release_regex_pattern")]
        public string ReleaseRegexPattern { get; set; }

        [JsonProperty("section_prefix")]
        public string SectionPrefix { get; set; }

        [JsonProperty("section_suffix")]
        public string SectionSuffix { get; set; }

        [JsonProperty("release_prefix")]
        public string ReleasePrefix { get; set; }

        [JsonProperty("release_suffix")]
        public string ReleaseSuffix { get; set; }

        [JsonProperty("ignore_words")]
        public string IgnoreWords { get; set; }

        [JsonProperty("dl_only_site")]
        public bool DlOnlySite { get; set; }

        [JsonProperty("disable_site")]
        public bool DisableSite { get; set; }

        [JsonProperty("chan1")]
        public string Chan1 { get; set; }

        [JsonProperty("blowfish_key1")]
        public string BlowfishKey1 { get; set; }

        [JsonProperty("chan2")]
        public string Chan2 { get; set; }

        [JsonProperty("blowfish_key2")]
        public string BlowfishKey2 { get; set; }

        [JsonProperty("chan3")]
        public string Chan3 { get; set; }

        [JsonProperty("blowfish_key3")]
        public string BlowfishKey3 { get; set; }

        [JsonProperty("chan4")]
        public string Chan4 { get; set; }

        [JsonProperty("blowfish_key4")]
        public string BlowfishKey4 { get; set; }

        [JsonProperty("chan5")]
        public string Chan5 { get; set; }

        [JsonProperty("blowfish_key5")]
        public string BlowfishKey5 { get; set; }

        [JsonProperty("chan6")]
        public string Chan6 { get; set; }

        [JsonProperty("blowfish_key6")]
        public string BlowfishKey6 { get; set; }

        [JsonProperty("chan7")]
        public string Chan7 { get; set; }

        [JsonProperty("blowfish_key7")]
        public string BlowfishKey7 { get; set; }

        [JsonProperty("chan8")]
        public string Chan8 { get; set; }

        [JsonProperty("blowfish_key8")]
        public string BlowfishKey8 { get; set; }

        [JsonProperty("chan9")]
        public string Chan9 { get; set; }

        [JsonProperty("blowfish_key9")]
        public string BlowfishKey9 { get; set; }

        [JsonProperty("chan10")]
        public string Chan10 { get; set; }

        [JsonProperty("blowfish_key10")]
        public string BlowfishKey10 { get; set; }

        [JsonProperty("chan11")]
        public string Chan11 { get; set; }

        [JsonProperty("blowfish_key11")]
        public string BlowfishKey11 { get; set; }

        [JsonProperty("chan12")]
        public string Chan12 { get; set; }

        [JsonProperty("blowfish_key12")]
        public string BlowfishKey12 { get; set; }

        [JsonProperty("chan13")]
        public string Chan13 { get; set; }

        [JsonProperty("blowfish_key13")]
        public string BlowfishKey13 { get; set; }

        [JsonProperty("chan14")]
        public string Chan14 { get; set; }

        [JsonProperty("blowfish_key14")]
        public string BlowfishKey14 { get; set; }

        [JsonProperty("chan15")]
        public string Chan15 { get; set; }

        [JsonProperty("blowfish_key15")]
        public string BlowfishKey15 { get; set; }

        [JsonProperty("chan16")]
        public string Chan16 { get; set; }

        [JsonProperty("blowfish_key16")]
        public string BlowfishKey16 { get; set; }

        [JsonProperty("chan17")]
        public string Chan17 { get; set; }

        [JsonProperty("blowfish_key17")]
        public string BlowfishKey17 { get; set; }

        [JsonProperty("chan18")]
        public string Chan18 { get; set; }

        [JsonProperty("blowfish_key18")]
        public string BlowfishKey18 { get; set; }

        [JsonProperty("chan19")]
        public string Chan19 { get; set; }

        [JsonProperty("blowfish_key19")]
        public string BlowfishKey19 { get; set; }

        [JsonProperty("chan20")]
        public string Chan20 { get; set; }

        [JsonProperty("blowfish_key20")]
        public string BlowfishKey20 { get; set; }

        // ─────────────────────────────────────────────────────────────
        // request-autofill template settings (no hardcoded regex)
        // ─────────────────────────────────────────────────────────────

        [JsonProperty("request_auto_fill_enabled")]
        public bool RequestAutoFillEnabled { get; set; }

        // Command to fetch requests, e.g. "SITE REQUESTS"
        [JsonProperty("request_list_command")]
        public string RequestListCommand { get; set; }

        // Regex pattern for one request line. Groups: "id", "name" (and optionally "user")
        [JsonProperty("request_line_pattern")]
        public string RequestLinePattern { get; set; }

        // How to send REQFILLED, e.g. "SITE REQFILLED {id}" or "SITE REQFILLED {name}"
        [JsonProperty("request_fill_template")]
        public string RequestFillTemplate { get; set; }

        // Where to create the transfer job dst path, e.g. "/REQUESTS/REQ-{release}"
        [JsonProperty("request_dst_path_template")]
        public string RequestDstPathTemplate { get; set; }

        // Pattern (or plain text) that marks a request as COMPLETE on the site
        [JsonProperty("request_complete_pattern")]
        public string RequestCompletePattern { get; set; }

        // How often to poll the site for requests (in seconds)
        [JsonProperty("request_poll_seconds")]
        public int RequestPollSeconds { get; set; } = 300;

        // Can this site be used as a SOURCE when filling requests for other sites?
        // (i.e. we can take releases from here to fill requests elsewhere)
        [JsonProperty("request_can_fill_source")]
        public bool RequestCanFillSource { get; set; } = false;

        [JsonProperty("pre_regex_pattern")]
        public string PreRegexPattern { get; set; }              // Pre_field_regex

        [JsonProperty("pre_section_regex_pattern")]
        public string PreSectionRegexPattern { get; set; }       // Section_pre_field

        [JsonProperty("pre_section_prefix")]
        public string PreSectionPrefix { get; set; }             // Section_pre_prefix_field

        [JsonProperty("pre_section_suffix")]
        public string PreSectionSuffix { get; set; }             // Section_pre_suffix_field

        [JsonProperty("pre_release_regex_pattern")]
        public string PreReleaseRegexPattern { get; set; }       // Release_Pre_field

        [JsonProperty("max_pre_time")]
        public int? MaxPreTime { get; set; }

    }

    public class Section
    {
        [JsonProperty("irc_name")]
        public string IrcName { get; set; }

        [JsonProperty("pretime")]
        public int? Pretime { get; set; }

        [JsonProperty("bnc")]
        public string Bnc { get; set; }

        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; } = new();

        [JsonProperty("rules")]
        public List<string> Rules { get; set; } = new();

        [JsonProperty("skiplists")]
        public List<string> Skiplists { get; set; } = new();

        [JsonProperty("dupeRules")]
        public DupeRules DupeRules { get; set; }

        [JsonProperty("imdb", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Imdb { get; set; }

        [JsonProperty("tvmaze", NullValueHandling = NullValueHandling.Ignore)]
        public JObject Tvmaze { get; set; }

    }

    public class Tag
    {
        [JsonProperty("map_cbftp_section")]
        public string MapCbftpSection { get; set; }

        [JsonProperty("trigger_regex")]
        public string TriggerRegex { get; set; }

        [JsonProperty("rules")]
        public List<string> Rules { get; set; } = new();
    }

    public class DupeRules
    {
        [JsonProperty("firstWins")]
        public bool FirstWins { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }
    }

    public class Mapping
    {
        public string MapCbftpSection { get; set; }
        public string TriggerRegex { get; set; }
        public List<string> Rules { get; set; } = new List<string>();
    }
