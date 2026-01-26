using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace RaceTrade
{
    public partial class ChannelTab : UserControl
    {
        private readonly List<(string message, Color color)> messageHistory = new();

        private readonly string siteName;
        private readonly string channelName;

        private readonly Action<string, string, string> sendMessageCallback;
        private readonly Action<string, string, string> fishKeyExchangeCallback;
        private Action<string, string, string> setChannelKeyCallback;

        // *** Parameterless ctor for the designer ***
        public ChannelTab()
        {
            InitializeComponent();
            // adjust item height here instead of in designer
            userListBox.ItemHeight = TextRenderer
                .MeasureText("Xg", userListBox.Font).Height + 2;
        }

        // Runtime ctor used by TabbedIrcLog
        public ChannelTab(
            string siteName,
            string channelName,
            Action<string, string, string> sendCallback,
            Action<string, string, string> fishCallback,
            Action<string, string, string> setKeyCallback)
            : this() // call parameterless ctor (InitializeComponent)
        {
            this.siteName = siteName;
            this.channelName = channelName;
            this.sendMessageCallback = sendCallback;
            this.fishKeyExchangeCallback = fishCallback;
            this.setChannelKeyCallback = setKeyCallback;
        }

        public void SetChannelKeyCallback(Action<string, string, string> callback)
        {
            setChannelKeyCallback = callback;
        }

        private void UserListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= userListBox.Items.Count)
                return;

            string username = userListBox.Items[e.Index].ToString();

            Color userColor = Color.White;
            if (username.StartsWith("~"))
                userColor = Color.Red;
            else if (username.StartsWith("&"))
                userColor = Color.DarkRed;
            else if (username.StartsWith("@"))
                userColor = Color.Orange;
            else if (username.StartsWith("%"))
                userColor = Color.Yellow;
            else if (username.StartsWith("+"))
                userColor = Color.LightGreen;

            e.DrawBackground();

            TextRenderer.DrawText(e.Graphics, username, e.Font,
                new Point(e.Bounds.X + 2, e.Bounds.Y + 1),
                userColor,
                e.BackColor);

            e.DrawFocusRectangle();
        }

        private void UserListBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = userListBox.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    userListBox.SelectedIndex = index;
                    string username = userListBox.SelectedItem.ToString();

                    ContextMenuStrip contextMenu = new ContextMenuStrip
                    {
                        BackColor = Color.FromArgb(45, 45, 48),
                        ForeColor = Color.White
                    };

                    ToolStripMenuItem keyExchangeItem = new ToolStripMenuItem("FiSH Key Exchange")
                    {
                        BackColor = Color.FromArgb(45, 45, 48),
                        ForeColor = Color.White
                    };
                    keyExchangeItem.Click += (s, ev) =>
                    {
                        fishKeyExchangeCallback?.Invoke(siteName, channelName, username);
                        AppendMessage($"[FiSH] Initiating key exchange with {username}", Color.Yellow);
                    };

                    ToolStripMenuItem pmItem = new ToolStripMenuItem("Send Private Message")
                    {
                        BackColor = Color.FromArgb(45, 45, 48),
                        ForeColor = Color.White
                    };
                    pmItem.Click += (s, ev) =>
                    {
                        AppendMessage($"[PM] Opening private message to {username}", Color.Cyan);
                    };

                    contextMenu.Items.Add(keyExchangeItem);
                    contextMenu.Items.Add(pmItem);
                    contextMenu.Show(userListBox, e.Location);
                }
            }
        }

        private void UserListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (userListBox.SelectedItem != null)
            {
                string username = userListBox.SelectedItem.ToString();
                string cleanUser = username.TrimStart('~', '&', '@', '%', '+');

                string pmTabName = $"PM:{cleanUser}";
                var parentForm = FindForm() as TabbedIrcLog;
                if (parentForm != null)
                {
                    var pmTab = parentForm.GetOrCreateChannelTab(siteName, pmTabName);
                    pmTab.AppendMessage($"[FiSH] Initiating key exchange with {cleanUser}", Color.Yellow);

                    parentForm.SwitchToTab(siteName, pmTabName);

                    fishKeyExchangeCallback?.Invoke(siteName, cleanUser, cleanUser);
                }
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendCurrentMessage();
            }
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            SendCurrentMessage();
        }

        private void SendCurrentMessage()
        {
            string message = inputBox.Text.Trim();
            if (!string.IsNullOrEmpty(message))
            {
                sendMessageCallback?.Invoke(siteName, channelName, message);
                inputBox.Clear();
            }
        }

        public void AppendMessage(string message, Color color)
        {
            if (outputBox.InvokeRequired)
            {
                outputBox.Invoke(new Action(() => AppendMessage(message, color)));
                return;
            }

            // Inject @ / + / etc. into "<nick>" based on user list
            message = ApplyUserModePrefixToMessage(message);

            string timestamp = DateTime.Now.ToString("[HH:mm:ss] ");

            messageHistory.Add((timestamp + message, color));

            // Keep last 1000 messages
            if (messageHistory.Count > 1000)
                messageHistory.RemoveAt(0);

            // Timestamp in gray
            outputBox.SelectionStart = outputBox.TextLength;
            outputBox.SelectionColor = Color.Gray;
            outputBox.AppendText(timestamp);

            // force “normal text” color regardless of the incoming color
            var defaultTextColor = Color.Black;  // or Color.Black
            IrcColorParser.AppendIrcText(outputBox, message, defaultTextColor);

            outputBox.AppendText(Environment.NewLine);
            outputBox.SelectionStart = outputBox.TextLength;
            outputBox.ScrollToCaret();
        }

        private string ApplyUserModePrefixToMessage(string message)
        {
            // Match something like "<BOTNICK>" at the start of the line (after optional spaces)
            var m = Regex.Match(message, @"^(\s*)<(?<nick>[^>]+)>(?<rest>.*)$");
            if (!m.Success)
                return message;

            string nick = m.Groups["nick"].Value;
            string leadingWs = m.Groups[1].Value;
            string rest = m.Groups["rest"].Value;

            // Find matching user in the user list (with mode prefix)
            string nickWithMode = null;

            foreach (string user in userListBox.Items)
            {
                string clean = StripUserMode(user); // you already have this method
                if (string.Equals(clean, nick, StringComparison.OrdinalIgnoreCase))
                {
                    nickWithMode = user;  // e.g. "@BOTNICK"
                    break;
                }
            }

            if (nickWithMode == null)
                return message; // not in list, return as-is

            // Build "<@BOTNICK> rest of line"
            return $"{leadingWs}<{nickWithMode}>{rest}";
        }


        private int FindUserIndexByNick(string nick)
        {
            string cleanNick = StripUserMode(nick);

            for (int i = 0; i < userListBox.Items.Count; i++)
            {
                string existing = userListBox.Items[i].ToString();
                if (StripUserMode(existing).Equals(cleanNick, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public void AddUser(string username)
        {
            if (userListBox.InvokeRequired)
            {
                userListBox.Invoke(new Action(() => AddUser(username)));
                return;
            }

            int existingIndex = FindUserIndexByNick(username);
            if (existingIndex == -1)
            {
                var users = userListBox.Items.Cast<string>().ToList();
                users.Add(username);

                userListBox.Items.Clear();
                var sortedUsers = users
                    .OrderBy(u => GetUserPriority(u))
                    .ThenBy(u => StripUserMode(u).ToLower())
                    .ToList();

                userListBox.Items.AddRange(sortedUsers.ToArray());
            }
            else
            {
                string existing = userListBox.Items[existingIndex].ToString();

                if (GetUserPriority(username) < GetUserPriority(existing))
                {
                    userListBox.Items[existingIndex] = username;
                }
            }
        }

        public void RemoveUser(string username)
        {
            if (userListBox.InvokeRequired)
            {
                userListBox.Invoke(new Action(() => RemoveUser(username)));
                return;
            }

            int index = FindUserIndexByNick(username);
            if (index != -1)
                userListBox.Items.RemoveAt(index);
        }

        private void OutputBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            var menu = new ContextMenuStrip
            {
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White
            };

            var copyItem = new ToolStripMenuItem("Copy")
            {
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Enabled = !string.IsNullOrEmpty(outputBox.SelectedText)
            };
            copyItem.Click += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(outputBox.SelectedText))
                    Clipboard.SetText(outputBox.SelectedText);
            };

            var copyAllItem = new ToolStripMenuItem("Copy All")
            {
                BackColor = Color.FromArgb(45, 45, 48),
                ForeColor = Color.White,
                Enabled = !string.IsNullOrEmpty(outputBox.Text)
            };
            copyAllItem.Click += (s, ev) =>
            {
                if (!string.IsNullOrEmpty(outputBox.Text))
                    Clipboard.SetText(outputBox.Text);
            };

            menu.Items.Add(copyItem);
            menu.Items.Add(copyAllItem);
            menu.Show(outputBox, e.Location);
        }

        public void UpdateUserList(List<string> users)
        {
            if (userListBox.InvokeRequired)
            {
                userListBox.Invoke(new Action(() => UpdateUserList(users)));
                return;
            }

            userListBox.Items.Clear();

            var sortedUsers = users
                .OrderBy(u => GetUserPriority(u))
                .ThenBy(u => StripUserMode(u).ToLower())
                .ToList();

            userListBox.Items.AddRange(sortedUsers.ToArray());
        }

        private int GetUserPriority(string username)
        {
            if (username.StartsWith("~")) return 0;
            if (username.StartsWith("&")) return 1;
            if (username.StartsWith("@")) return 2;
            if (username.StartsWith("%")) return 3;
            if (username.StartsWith("+")) return 4;
            return 5;
        }

        private string StripUserMode(string username)
        {
            return username.TrimStart('~', '&', '@', '%', '+');
        }

        public void FocusInput()
        {
            if (inputBox != null && !inputBox.IsDisposed)
                inputBox.Focus();
        }
    }
}
