using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class ApplicationLog : Form
    {
        private readonly List<(string message, Color color)> logEntries = new();

        // Cancel pending scroll actions
        private int _scrollToken = 0;

        // Scroll commands
        private const int WM_VSCROLL = 0x0115;
        private const int SB_BOTTOM = 7;

        // Preserve viewport when auto-scroll is OFF
        private const int EM_GETFIRSTVISIBLELINE = 0x00CE;
        private const int EM_LINESCROLL = 0x00B6;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private bool AutoScrollEnabled => autoScrollCheckBox != null && autoScrollCheckBox.Checked;

        public ApplicationLog()
        {
            InitializeComponent();
            RaceTrade.ThemeManager.ApplyTheme(this);

            // --- defaults for toolbar controls ---
            searchBox.Text = "Search logs...";
            searchBox.ForeColor = Color.Gray;

            if (filterComboBox.Items.Count > 0)
                filterComboBox.SelectedIndex = 0;

            autoScrollCheckBox.Checked = true;
            // -------------------------------------

            this.FormClosing += RaceLog_FormClosing;

            // Works every time after Hide()->Show()
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

            // Park caret at end (fine when auto-scroll is ON)
            logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            logRichTextBox.SelectionLength = 0;
        }

        private void RequestScrollBottom()
        {
            if (!AutoScrollEnabled) return;

            int token = ++_scrollToken;

            BeginInvoke(new Action(() =>
            {
                // Must re-check at execution time
                if (!AutoScrollEnabled) return;
                if (token != _scrollToken) return;

                ForceScrollBottom();
            }));
        }

        // ---------- SEARCH / FILTER ----------

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
            // Cancel any pending scrolls
            _scrollToken++;

            // If turning ON, immediately go to bottom
            if (AutoScrollEnabled)
                RequestScrollBottom();
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
                return filter switch
                {
                    "Errors" => entry.color == Color.Red || entry.color == Color.Magenta,
                    "Warnings" => entry.color == Color.Orange,
                    "Success" => entry.color == Color.LimeGreen || entry.color == Color.Green,
                    "Info" => entry.color == Color.White,
                    "Debug" => entry.color == Color.Cyan,
                    _ => true
                };
            }).ToList();

            DisplayFilteredLogs(filteredLogs);
        }

        private void DisplayFilteredLogs(List<(string message, Color color)> logs)
        {
            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated)
                return;

            // If user turned off autoscroll, preserve viewport while rebuilding
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
                // restore selection + viewport so it doesn't jump
                int max = logRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                logRichTextBox.SelectionStart = selStart;
                logRichTextBox.SelectionLength = selLen;

                RestoreFirstVisibleLine(logRichTextBox, firstLine);
            }
        }

        // ---------- CLEAR / EXPORT ----------

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all logs?",
                "Clear Logs",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                logEntries.Clear();
                if (logRichTextBox != null && !logRichTextBox.IsDisposed)
                    logRichTextBox.Clear();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*",
                FileName = $"RaceLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

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

        // ---------- APPEND / RESTORE ----------

        public void AppendLog(string message, Color color)
        {
            if (IsDisposed) return;

            // No duplicates: marshal first, store only on UI thread
            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => AppendLog(message, color))); }
                catch { }
                return;
            }

            logEntries.Add((message, color));

            const int maxLines = 1000;
            if (logEntries.Count > maxLines)
                logEntries.RemoveAt(0);

            if (logRichTextBox == null || logRichTextBox.IsDisposed || !logRichTextBox.IsHandleCreated)
                return;

            // Preserve selection + viewport when auto-scroll is OFF
            int firstLine = GetFirstVisibleLine(logRichTextBox);
            int selStart = logRichTextBox.SelectionStart;
            int selLen = logRichTextBox.SelectionLength;

            // Append at end (needed for color)
            logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            logRichTextBox.SelectionLength = 0;
            logRichTextBox.SelectionColor = color;
            logRichTextBox.AppendText(message + Environment.NewLine);

            if (AutoScrollEnabled)
            {
                // Follow mode
                RequestScrollBottom();
            }
            else
            {
                // Restore user's viewport + selection so copying isn't interrupted
                int max = logRichTextBox.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                logRichTextBox.SelectionStart = selStart;
                logRichTextBox.SelectionLength = selLen;

                RestoreFirstVisibleLine(logRichTextBox, firstLine);
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

        // ---------- FORM LIFECYCLE ----------

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
