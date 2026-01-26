// ======================= RaceLog.cs =======================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class RaceLog : Form
    {
        private readonly List<(string message, Color color)> logEntries = new();

        private int _scrollToken = 0;

        private const int WM_VSCROLL = 0x0115;
        private const int SB_BOTTOM = 7;

        private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        private const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private bool AutoScrollEnabled => autoScrollCheckBox != null && autoScrollCheckBox.Checked;

        public RaceLog()
        {
            InitializeComponent();

            searchBox.Text = "Search logs...";
            searchBox.ForeColor = Color.Gray;

            if (filterComboBox.Items.Count > 0)
                filterComboBox.SelectedIndex = 0;

            autoScrollCheckBox.Checked = true;

            this.FormClosing += RaceLog_FormClosing;

            this.VisibleChanged += (_, __) =>
            {
                if (!this.Visible) return;

                RestoreLogs();
                if (AutoScrollEnabled)
                    RequestScrollBottom();
            };
        }

        private int GetFirstVisibleLine(RichTextBox rtb)
        {
            if (rtb == null || rtb.IsDisposed || !rtb.IsHandleCreated) return 0;
            return (int)SendMessage(rtb.Handle, EM_GETFIRSTVISIBLELINE, IntPtr.Zero, IntPtr.Zero);
        }

        private void RestoreFirstVisibleLine(RichTextBox rtb, int originalFirstLine)
        {
            if (rtb == null || rtb.IsDisposed || !rtb.IsHandleCreated) return;

            int currentFirst = GetFirstVisibleLine(rtb);
            int delta = originalFirstLine - currentFirst;

            if (delta != 0)
                SendMessage(rtb.Handle, EM_LINESCROLL, IntPtr.Zero, (IntPtr)delta);
        }

        private void ForceScrollBottom()
        {
            if (raceLogRichTextBox == null || raceLogRichTextBox.IsDisposed || !raceLogRichTextBox.IsHandleCreated) return;
            if (!this.Visible) return;

            SendMessage(raceLogRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);

            raceLogRichTextBox.SelectionStart = raceLogRichTextBox.TextLength;
            raceLogRichTextBox.SelectionLength = 0;
        }

        private void RequestScrollBottom()
        {
            if (!AutoScrollEnabled) return;

            int token = ++_scrollToken;

            BeginInvoke(new Action(() =>
            {
                if (!AutoScrollEnabled) return;
                if (token != _scrollToken) return;

                ForceScrollBottom();
            }));
        }

        private void SearchBox_GotFocus(object sender, EventArgs e)
        {
            if (searchBox.Text == "Search logs...")
            {
                searchBox.Text = string.Empty;
                searchBox.ForeColor = Color.White;
            }
        }

        private void SearchBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.Text = "Search logs...";
                searchBox.ForeColor = Color.Gray;
            }
        }

        private void autoScrollCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _scrollToken++; // cancel pending scrolls

            if (AutoScrollEnabled)
                RequestScrollBottom();
        }

        // Optional wrapper if Designer uses AutoScrollCheckBox_CheckedChanged
        private void AutoScrollCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            autoScrollCheckBox_CheckedChanged(sender, e);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text == "Search logs..." || string.IsNullOrWhiteSpace(searchBox.Text))
            {
                RestoreLogs();
                return;
            }

            string searchTerm = searchBox.Text.ToLowerInvariant();
            var filteredLogs = logEntries.Where(entry =>
                (entry.message ?? string.Empty).ToLowerInvariant().Contains(searchTerm)).ToList();

            DisplayFilteredLogs(filteredLogs);
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = filterComboBox.SelectedItem?.ToString();
            if (filter == "All" || filter == null)
            {
                RestoreLogs();
                return;
            }

            var filteredLogs = logEntries.Where(entry =>
            {
                string msg = (entry.message ?? string.Empty).ToUpperInvariant();
                return filter switch
                {
                    "Detected" => msg.Contains("[DETECTED]"),
                    "Racing" => msg.Contains("[RACING]"),
                    "Complete" => msg.Contains("[COMPLETE]"),
                    "Filtered" => msg.Contains("[FILTERED]"),
                    "Failed" => msg.Contains("[FAILED]"),
                    _ => true
                };
            }).ToList();

            DisplayFilteredLogs(filteredLogs);
        }

        private void DisplayFilteredLogs(List<(string message, Color color)> logs)
        {
            if (raceLogRichTextBox == null || raceLogRichTextBox.IsDisposed || !raceLogRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(raceLogRichTextBox);
            int selStart = raceLogRichTextBox.SelectionStart;
            int selLen = raceLogRichTextBox.SelectionLength;

            raceLogRichTextBox.Clear();
            foreach (var (message, color) in logs)
            {
                raceLogRichTextBox.SelectionStart = raceLogRichTextBox.TextLength;
                raceLogRichTextBox.SelectionLength = 0;

                // Parse color tags when filtering
                var parts = ParseColorTags(message, color);
                foreach (var part in parts)
                {
                    raceLogRichTextBox.SelectionColor = part.color;
                    raceLogRichTextBox.AppendText(part.text);
                }

                raceLogRichTextBox.AppendText(Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = raceLogRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                raceLogRichTextBox.SelectionStart = selStart;
                raceLogRichTextBox.SelectionLength = selLen;

                RestoreFirstVisibleLine(raceLogRichTextBox, firstLine);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all race logs?",
                "Clear Race Logs",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                logEntries.Clear();
                if (raceLogRichTextBox != null && !raceLogRichTextBox.IsDisposed)
                    raceLogRichTextBox.Clear();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*";
                saveDialog.FileName = $"RaceLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var logText = string.Join(Environment.NewLine, logEntries.Select(e => e.message));
                        File.WriteAllText(saveDialog.FileName, logText);

                        MessageBox.Show(
                            $"Race log exported successfully to:\n{saveDialog.FileName}",
                            "Export Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to export race log:\n{ex.Message}",
                            "Export Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        public void AppendLog(string message, Color defaultColor)
        {
            if (IsDisposed) return;
            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => AppendLog(message, defaultColor))); }
                catch { }
                return;
            }

            logEntries.Add((message, defaultColor));
            const int maxLines = 1000;
            if (logEntries.Count > maxLines)
                logEntries.RemoveAt(0);

            if (raceLogRichTextBox == null || raceLogRichTextBox.IsDisposed || !raceLogRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(raceLogRichTextBox);
            int selStart = raceLogRichTextBox.SelectionStart;
            int selLen = raceLogRichTextBox.SelectionLength;

            raceLogRichTextBox.SelectionStart = raceLogRichTextBox.TextLength;
            raceLogRichTextBox.SelectionLength = 0;

            //  Parse and apply colors
            var parts = ParseColorTags(message, defaultColor);

            foreach (var part in parts)
            {
                raceLogRichTextBox.SelectionColor = part.color;
                raceLogRichTextBox.AppendText(part.text);
            }

            raceLogRichTextBox.AppendText(Environment.NewLine);

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = raceLogRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                raceLogRichTextBox.SelectionStart = selStart;
                raceLogRichTextBox.SelectionLength = selLen;
                RestoreFirstVisibleLine(raceLogRichTextBox, firstLine);
            }
        }

        private List<(string text, Color color)> ParseColorTags(string message, Color defaultColor)
        {

             var result = new List<(string, Color)>();
            var colorMap = new Dictionary<string, Color>
    {
        { "CYAN", Color.Cyan },
        { "YELLOW", Color.Yellow },
        { "GREEN", Color.LimeGreen },
        { "RED", Color.Red },
        { "MAGENTA", Color.Magenta },
        { "ORANGE", Color.Orange },
        { "LIGHTBLUE", Color.LightBlue },
        { "WHITE", Color.White }
    };

            int pos = 0;
            while (pos < message.Length)
            {
                int startTag = message.IndexOf("{", pos);

                if (startTag == -1)
                {
                    // No more tags - add rest with default color
                    if (pos < message.Length)
                        result.Add((message.Substring(pos), defaultColor));
                    break;
                }

                // Add text before tag with default color
                if (startTag > pos)
                {
                    result.Add((message.Substring(pos, startTag - pos), defaultColor));
                }

                int endTag = message.IndexOf("}", startTag);
                if (endTag == -1)
                {
                    result.Add((message.Substring(startTag), defaultColor));
                    break;
                }

                string tag = message.Substring(startTag + 1, endTag - startTag - 1);

                if (tag.StartsWith("/"))
                {
                    pos = endTag + 1;
                    continue;
                }

                string closingTag = "{/" + tag + "}";
                int closePos = message.IndexOf(closingTag, endTag);

                if (closePos == -1)
                {
                    result.Add((message.Substring(startTag), defaultColor));
                    break;
                }

                string coloredText = message.Substring(endTag + 1, closePos - endTag - 1);
                Color color = colorMap.ContainsKey(tag) ? colorMap[tag] : defaultColor;

                result.Add((coloredText, color));
                pos = closePos + closingTag.Length;
            }

            return result;
        }



        public void ClearLog()
        {
            if (raceLogRichTextBox == null || raceLogRichTextBox.IsDisposed)
                return;

            if (raceLogRichTextBox.InvokeRequired)
            {
                try { raceLogRichTextBox.Invoke(new Action(ClearLog)); }
                catch { }
                return;
            }

            logEntries.Clear();
            raceLogRichTextBox.Clear();
        }

        public void RestoreLogs()
        {
            if (raceLogRichTextBox == null || raceLogRichTextBox.IsDisposed || !raceLogRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(raceLogRichTextBox);
            int selStart = raceLogRichTextBox.SelectionStart;
            int selLen = raceLogRichTextBox.SelectionLength;

            raceLogRichTextBox.Clear();

            foreach (var (message, color) in logEntries)
            {
                raceLogRichTextBox.SelectionStart = raceLogRichTextBox.TextLength;
                raceLogRichTextBox.SelectionLength = 0;

                // Parse color tags when restoring
                var parts = ParseColorTags(message, color);
                foreach (var part in parts)
                {
                    raceLogRichTextBox.SelectionColor = part.color;
                    raceLogRichTextBox.AppendText(part.text);
                }

                raceLogRichTextBox.AppendText(Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = raceLogRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                raceLogRichTextBox.SelectionStart = selStart;
                raceLogRichTextBox.SelectionLength = selLen;

                RestoreFirstVisibleLine(raceLogRichTextBox, firstLine);
            }
        }

        private void RaceLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
