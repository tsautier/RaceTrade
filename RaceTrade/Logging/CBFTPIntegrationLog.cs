// ======================= CBFTPIntegrationLog.cs =======================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class CBFTPIntegrationLog : Form
    {
        private readonly List<(string message, Color color)> logEntries = new List<(string, Color)>();

        private int _scrollToken = 0;

        // Track which line belongs to which job
        private readonly Dictionary<int, int> jobLogLineIndex = new Dictionary<int, int>();

        private const int WM_VSCROLL = 0x0115;
        private const int SB_BOTTOM = 7;

        private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        private const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private bool AutoScrollEnabled => autoScrollCheckBox != null && autoScrollCheckBox.Checked;

        public CBFTPIntegrationLog()
        {
            InitializeComponent();
            this.FormClosing += CBFTPIntegrationLog_FormClosing;

            searchBox.Text = "Search logs...";
            searchBox.ForeColor = Color.Gray;

            if (filterComboBox.Items.Count > 0)
                filterComboBox.SelectedIndex = 0;

            autoScrollCheckBox.Checked = true;

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
            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated) return;
            if (!this.Visible) return;

            SendMessage(logRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);

            logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            logRichTextBox.SelectionLength = 0;
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

        public void UpdateJobProgress(int jobId, string newMessage, Color color)
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => UpdateJobProgress(jobId, newMessage, color))); }
                catch { }
                return;
            }

            try
            {
                if (jobLogLineIndex.TryGetValue(jobId, out int logIndex) &&
                    logIndex >= 0 &&
                    logIndex < logEntries.Count)
                {
                    logEntries[logIndex] = (newMessage, color);
                    RestoreLogs();
                }
            }
            catch
            {
                // ignored
            }
        }

        public void AddJobProgressEntry(int jobId, string message, Color color)
        {
            jobLogLineIndex[jobId] = logEntries.Count;
            AppendLog(message, color);
        }

        public void RemoveJobTracking(int jobId)
        {
            jobLogLineIndex.Remove(jobId);
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
                string msg = entry.message ?? string.Empty;
                return filter switch
                {
                    "Connected" => msg.Contains("[CONNECTED]"),
                    "Jobs Sent" => msg.Contains("[JOB SENT]"),
                    "Completed" => msg.Contains("[COMPLETED]"),
                    "Failed" => msg.Contains("[FAILED]"),
                    "Errors" => entry.color == Color.Red || entry.color == Color.LightCoral,
                    _ => true
                };
            }).ToList();

            DisplayFilteredLogs(filteredLogs);
        }

        private void DisplayFilteredLogs(List<(string message, Color color)> logs)
        {
            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(logRichTextBox);
            int selStart = logRichTextBox.SelectionStart;
            int selLen = logRichTextBox.SelectionLength;

            logRichTextBox.Clear();
            foreach (var (message, color) in logs)
            {
                logRichTextBox.SelectionStart = logRichTextBox.TextLength;
                logRichTextBox.SelectionLength = 0;
                logRichTextBox.SelectionColor = color;
                logRichTextBox.AppendText(message + Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = logRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                logRichTextBox.SelectionStart = selStart;
                logRichTextBox.SelectionLength = selLen;

                RestoreFirstVisibleLine(logRichTextBox, firstLine);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all CBFTP logs?",
                "Clear Logs",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                logEntries.Clear();
                jobLogLineIndex.Clear();

                if (logRichTextBox != null && !logRichTextBox.IsDisposed)
                    logRichTextBox.Clear();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*";
                saveDialog.FileName = $"CBFTPLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var logText = string.Join(Environment.NewLine, logEntries.Select(e => e.message));
                        File.WriteAllText(saveDialog.FileName, logText);

                        MessageBox.Show(
                            $"Log exported successfully to:\n{saveDialog.FileName}",
                            "Export Success",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Failed to export log:\n{ex.Message}",
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

            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(logRichTextBox);
            int selStart = logRichTextBox.SelectionStart;
            int selLen = logRichTextBox.SelectionLength;

            logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            logRichTextBox.SelectionLength = 0;

            //Parse and apply colors
            var parts = ParseColorTags(message, defaultColor);
            foreach (var part in parts)
            {
                logRichTextBox.SelectionColor = part.color;
                logRichTextBox.AppendText(part.text);
            }

            logRichTextBox.AppendText(Environment.NewLine);

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = logRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                logRichTextBox.SelectionStart = selStart;
                logRichTextBox.SelectionLength = selLen;
                RestoreFirstVisibleLine(logRichTextBox, firstLine);
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
                    if (pos < message.Length)
                        result.Add((message.Substring(pos), defaultColor));
                    break;
                }

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


        private void CBFTPIntegrationLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        public void RestoreLogs()
        {
            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(logRichTextBox);
            int selStart = logRichTextBox.SelectionStart;
            int selLen = logRichTextBox.SelectionLength;

            logRichTextBox.Clear();

            foreach (var (message, color) in logEntries)
            {
                logRichTextBox.SelectionStart = logRichTextBox.TextLength;
                logRichTextBox.SelectionLength = 0;

                // Parse color tags when restoring
                var parts = ParseColorTags(message, color);
                foreach (var part in parts)
                {
                    logRichTextBox.SelectionColor = part.color;
                    logRichTextBox.AppendText(part.text);
                }

                logRichTextBox.AppendText(Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = logRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                logRichTextBox.SelectionStart = selStart;
                logRichTextBox.SelectionLength = selLen;
                RestoreFirstVisibleLine(logRichTextBox, firstLine);
            }
        }
    }
}
