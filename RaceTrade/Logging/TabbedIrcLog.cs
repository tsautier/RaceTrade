using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class TabbedIrcLog : Form
    {
        // Store tabs by site and channel
        private Dictionary<string, TabPage> siteTabs = new Dictionary<string, TabPage>();
        private Dictionary<string, Dictionary<string, ChannelTab>> channelTabs =
            new Dictionary<string, Dictionary<string, ChannelTab>>();

        // Callback for sending messages
        private Action<string, string, string> sendMessageCallback;
        private Action<string, string, string> fishKeyExchangeCallback;
        private Action<string, string, string> setChannelKeyCallback;

        // Disposal flag
        private bool isClosing = false;

        public TabbedIrcLog()
        {
            InitializeComponent();          // created in Designer file
            RaceTrade.ThemeManager.ApplyTheme(this);
            this.FormClosing += TabbedIrcLog_FormClosing;
        }

        public void SetChannelKeyCallback(Action<string, string, string> callback)
        {
            this.setChannelKeyCallback = callback;
        }

        private void SendChannelKey(string siteName, string channelName, string key)
        {
            setChannelKeyCallback?.Invoke(siteName, channelName, key);
        }

        // -------- TAB DRAWING / MOUSE HANDLERS --------

        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            TabPage tabPage = tabControl.TabPages[e.Index];

            Rectangle tabRect = tabControl.GetTabRect(e.Index);




                    Color backColor = (e.State == DrawItemState.Selected)
        ? Color.FromArgb(58, 65, 82)   // ← lighter, stands out more
        : Color.FromArgb(22, 26, 36); ;

            using (SolidBrush brush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(brush, tabRect);
            }

            bool isChannelTab = tabControl.Name != null &&
                                tabControl.Name.StartsWith("channelTabs_");
            if (isChannelTab)
            {
                Rectangle closeRect = new Rectangle(tabRect.Right - 18, tabRect.Top + 4, 12, 12);

                using (Pen pen = new Pen(Color.Gray, 2))
                {
                    e.Graphics.DrawLine(pen, closeRect.Left, closeRect.Top, closeRect.Right, closeRect.Bottom);
                    e.Graphics.DrawLine(pen, closeRect.Right, closeRect.Top, closeRect.Left, closeRect.Bottom);
                }

                // keep any existing tag but we don't actually use this composite Tag
                tabPage.Tag = new { ChannelTab = tabPage.Tag, CloseRect = closeRect };
            }

            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                Rectangle textRect = isChannelTab
                    ? new Rectangle(tabRect.X, tabRect.Y, tabRect.Width - 20, tabRect.Height)
                    : tabRect;

                StringFormat stringFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(tabPage.Text, tabControl.Font, textBrush, textRect, stringFormat);
            }
        }

        private void ChannelTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (tabControl == null) return;

            // Close button (left click on X)
            if (e.Button == MouseButtons.Left)
            {
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    Rectangle tabRect = tabControl.GetTabRect(i);
                    Rectangle closeRect = new Rectangle(tabRect.Right - 18, tabRect.Top + 4, 12, 12);

                    if (closeRect.Contains(e.Location))
                    {
                        TabPage tabToClose = tabControl.TabPages[i];
                        string channelName = tabToClose.Text;

                        // Don't close first tab if you don't want to
                        if (i == 0)
                            return;

                        if (channelName.StartsWith("PM:"))
                        {
                            var result = MessageBox.Show(
                                $"Close private message with {channelName.Substring(3)}?",
                                "Close PM",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (result != DialogResult.Yes)
                                return;
                        }

                        tabControl.TabPages.RemoveAt(i);
                        return;
                    }
                }
            }
            // Right-click on tab -> Blowfish key menu
            else if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl.TabPages.Count; i++)
                {
                    Rectangle tabRect = tabControl.GetTabRect(i);

                    if (tabRect.Contains(e.Location))
                    {
                        string channelName = tabControl.TabPages[i].Text;

                        // Extract site name from TabControl.Name: "channelTabs_<siteName>"
                        string siteName = null;
                        if (!string.IsNullOrEmpty(tabControl.Name) &&
                            tabControl.Name.StartsWith("channelTabs_"))
                        {
                            siteName = tabControl.Name.Substring("channelTabs_".Length);
                        }

                        if (string.IsNullOrEmpty(siteName))
                            return;

                        var menu = new ContextMenuStrip
                        {
                            BackColor = Color.FromArgb(22, 26, 36),
                            ForeColor = Color.White
                        };

                        var setKeyItem = new ToolStripMenuItem("Set Blowfish key (UTF-8)")
                        {
                            BackColor = Color.FromArgb(22, 26, 36),
                            ForeColor = Color.White
                        };

                        setKeyItem.Click += (s, ev) =>
                        {
                            string key = PromptForBlowfishKey(channelName);
                            if (!string.IsNullOrWhiteSpace(key))
                            {
                                setChannelKeyCallback?.Invoke(siteName, channelName, key);

                                AppendChannelMessage(
                                    siteName,
                                    channelName,
                                    $"[FiSH] Manual Blowfish key set for {channelName}",
                                    Color.Yellow);
                            }
                        };

                        menu.Items.Add(setKeyItem);
                        menu.Show(tabControl, e.Location);
                        return;
                    }
                }
            }
        }

        // -------- TAB CREATION --------

        public TabPage GetOrCreateSiteTab(string siteName)
        {
            if (siteTabs.ContainsKey(siteName))
                return siteTabs[siteName];

            TabPage siteTab = new TabPage(siteName)
            {
                BackColor = Color.FromArgb(13, 16, 24)
            };

            TabControl channelTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(22, 26, 36),
                Font = new Font("Segoe UI", 9),
                DrawMode = TabDrawMode.OwnerDrawFixed,
                Padding = new Point(15, 3),
                Name = $"channelTabs_{siteName}"
            };
            channelTabControl.DrawItem += MainTabControl_DrawItem;
            channelTabControl.MouseDown += ChannelTabControl_MouseDown;

            siteTab.Controls.Add(channelTabControl);

            if (InvokeRequired)
                Invoke(new Action(() => mainTabControl.TabPages.Add(siteTab)));
            else
                mainTabControl.TabPages.Add(siteTab);

            siteTabs[siteName] = siteTab;
            channelTabs[siteName] = new Dictionary<string, ChannelTab>();

            return siteTab;
        }

        public ChannelTab GetOrCreateChannelTab(string siteName, string channelName)
        {
            if (!channelTabs.ContainsKey(siteName))
                GetOrCreateSiteTab(siteName);

            if (channelTabs[siteName].ContainsKey(channelName))
                return channelTabs[siteName][channelName];

            TabPage siteTab = siteTabs[siteName];
            TabControl channelTabControl = siteTab.Controls.OfType<TabControl>().FirstOrDefault();
            if (channelTabControl == null)
                return null;

            ChannelTab channelTab = new ChannelTab(
                siteName,
                channelName,
                SendMessage,
                SendFishKeyExchange,
                SendChannelKey);

            var tabPage = new TabPage(channelName)
            {
                BackColor = Color.FromArgb(13, 16, 24),
                Padding = new Padding(0)   // <<< remove default 3px padding
            };

            channelTab.Dock = DockStyle.Fill;
            channelTab.Margin = new Padding(0);  // <<< no margin inside tab

            tabPage.Controls.Add(channelTab);

            if (InvokeRequired)
                Invoke(new Action(() => channelTabControl.TabPages.Add(tabPage)));
            else
                channelTabControl.TabPages.Add(tabPage);

            channelTabs[siteName][channelName] = channelTab;

            return channelTab;
        }

        // -------- PUBLIC API USED BY IRC CLIENT --------

        public void AppendChannelMessage(string siteName, string channelName, string message, Color color)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        AppendChannelMessage(siteName, channelName, message, color)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                ChannelTab channelTab = GetOrCreateChannelTab(siteName, channelName);
                channelTab?.AppendMessage(message, color);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void AddUser(string siteName, string channelName, string username)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        AddUser(siteName, channelName, username)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                ChannelTab channelTab = GetOrCreateChannelTab(siteName, channelName);
                channelTab?.AddUser(username);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void RemoveUser(string siteName, string channelName, string username)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        RemoveUser(siteName, channelName, username)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                if (channelTabs.ContainsKey(siteName) &&
                    channelTabs[siteName].ContainsKey(channelName))
                {
                    channelTabs[siteName][channelName]?.RemoveUser(username);
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void SwitchToTab(string siteName, string channelName)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        SwitchToTab(siteName, channelName)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                if (!siteTabs.ContainsKey(siteName))
                    return;

                TabPage siteTab = siteTabs[siteName];
                mainTabControl.SelectedTab = siteTab;

                var channelTabControl = siteTab.Controls.OfType<TabControl>().FirstOrDefault();
                if (channelTabControl == null)
                    return;

                foreach (TabPage tab in channelTabControl.TabPages)
                {
                    if (tab.Text == channelName)
                    {
                        channelTabControl.SelectedTab = tab;
                        var channelTab = tab.Controls.OfType<ChannelTab>().FirstOrDefault();
                        channelTab?.FocusInput();
                        break;
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void UpdateUserList(string siteName, string channelName, List<string> users)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        UpdateUserList(siteName, channelName, users)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            try
            {
                ChannelTab channelTab = GetOrCreateChannelTab(siteName, channelName);
                channelTab?.UpdateUserList(users);
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void SetSendMessageCallback(Action<string, string, string> callback)
        {
            this.sendMessageCallback = callback;
        }

        public void SetFishKeyExchangeCallback(Action<string, string, string> callback)
        {
            this.fishKeyExchangeCallback = callback;
        }

        private void SendMessage(string siteName, string channelName, string message)
        {
            sendMessageCallback?.Invoke(siteName, channelName, message);
        }

        private void SendFishKeyExchange(string siteName, string channelName, string username)
        {
            fishKeyExchangeCallback?.Invoke(siteName, channelName, username);
        }

        public void AppendLog(string message, Color color)
        {
            if (isClosing || IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                try
                {
                    BeginInvoke(new Action(() => AppendLog(message, color)));
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            // Show log lines in first available channel
            if (siteTabs.Count > 0)
            {
                var firstSite = siteTabs.Keys.First();
                if (channelTabs.ContainsKey(firstSite) && channelTabs[firstSite].Count > 0)
                {
                    var firstChannel = channelTabs[firstSite].Keys.First();
                    AppendChannelMessage(firstSite, firstChannel, message, color);
                }
            }
        }

        public void RestoreLogs()
        {
            // No-op, kept for compatibility
        }

        private string PromptForBlowfishKey(string channelName)
        {
            using (var form = new Form())
            {
                form.Text = $"Blowfish key for {channelName}";
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ClientSize = new Size(350, 110);
                form.BackColor = Color.FromArgb(18, 22, 31);

                var label = new Label
                {
                    Text = "Enter UTF-8 Blowfish key:",
                    AutoSize = true,
                    ForeColor = Color.White,
                    Location = new Point(10, 10)
                };

                var textBox = new TextBox
                {
                    Location = new Point(10, 35),
                    Width = 320,
                    BackColor = Color.FromArgb(13, 16, 24),
                    ForeColor = Color.White
                };

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(175, 70),
                    Width = 70
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(255, 70),
                    Width = 70
                };

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                form.Controls.Add(okButton);
                form.Controls.Add(cancelButton);
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                return form.ShowDialog(this) == DialogResult.OK
                    ? textBox.Text.Trim()
                    : null;
            }
        }

        private void TabbedIrcLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;

            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
                isClosing = false;
            }
        }
    }

}