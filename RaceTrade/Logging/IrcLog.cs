// ======================= IrcLog.cs =======================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RaceTrade
{
    public partial class IrcLog : Form
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

        public IrcLog()
        {
            InitializeComponent();

            // --- defaults for toolbar controls ---
            searchBox.Text = "Search logs...";
            searchBox.ForeColor = Color.Gray;

            if (filterComboBox.Items.Count > 0)
                filterComboBox.SelectedIndex = 0;

            autoScrollCheckBox.Checked = true;
            // -------------------------------------

            this.FormClosing += IrcLog_FormClosing;

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
            if (log_richTextBox1 == null || log_richTextBox1.IsDisposed || !log_richTextBox1.IsHandleCreated) return;
            if (!this.Visible) return;

            SendMessage(log_richTextBox1.Handle, WM_VSCROLL, (IntPtr)SB_BOTTOM, IntPtr.Zero);

            log_richTextBox1.SelectionStart = log_richTextBox1.TextLength;
            log_richTextBox1.SelectionLength = 0;
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

        // ---------- SEARCH / FILTER UI ----------

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

        // Optional: keep this wrapper ONLY if your Designer is wired to AutoScrollCheckBox_CheckedChanged
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
            if (log_richTextBox1 == null || log_richTextBox1.IsDisposed || !log_richTextBox1.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(log_richTextBox1);
            int selStart = log_richTextBox1.SelectionStart;
            int selLen = log_richTextBox1.SelectionLength;

            log_richTextBox1.Clear();
            foreach (var (message, color) in logs)
            {
                log_richTextBox1.SelectionStart = log_richTextBox1.TextLength;
                log_richTextBox1.SelectionLength = 0;
                log_richTextBox1.SelectionColor = color;
                log_richTextBox1.AppendText(message + Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = log_richTextBox1.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                log_richTextBox1.SelectionStart = selStart;
                log_richTextBox1.SelectionLength = selLen;

                RestoreFirstVisibleLine(log_richTextBox1, firstLine);
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
                if (log_richTextBox1 != null && !log_richTextBox1.IsDisposed)
                    log_richTextBox1.Clear();
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            using var saveDialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt|Log Files|*.log|All Files|*.*",
                FileName = $"IrcLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
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

            if (log_richTextBox1 == null || log_richTextBox1.IsDisposed || !log_richTextBox1.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(log_richTextBox1);
            int selStart = log_richTextBox1.SelectionStart;
            int selLen = log_richTextBox1.SelectionLength;

            log_richTextBox1.SelectionStart = log_richTextBox1.TextLength;
            log_richTextBox1.SelectionLength = 0;
            log_richTextBox1.SelectionColor = color;
            log_richTextBox1.AppendText(message + Environment.NewLine);

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = log_richTextBox1.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                log_richTextBox1.SelectionStart = selStart;
                log_richTextBox1.SelectionLength = selLen;

                RestoreFirstVisibleLine(log_richTextBox1, firstLine);
            }
        }

        public void RestoreLogs()
        {
            if (log_richTextBox1 == null || log_richTextBox1.IsDisposed || !log_richTextBox1.IsHandleCreated)
                return;

            int firstLine = GetFirstVisibleLine(log_richTextBox1);
            int selStart = log_richTextBox1.SelectionStart;
            int selLen = log_richTextBox1.SelectionLength;

            log_richTextBox1.Clear();

            foreach (var (message, color) in logEntries)
            {
                log_richTextBox1.SelectionStart = log_richTextBox1.TextLength;
                log_richTextBox1.SelectionLength = 0;
                log_richTextBox1.SelectionColor = color;
                log_richTextBox1.AppendText(message + Environment.NewLine);
            }

            if (AutoScrollEnabled)
            {
                RequestScrollBottom();
            }
            else
            {
                int max = log_richTextBox1.TextLength;
                selStart = Math.Min(selStart, max);
                selLen = Math.Min(selLen, Math.Max(0, max - selStart));
                log_richTextBox1.SelectionStart = selStart;
                log_richTextBox1.SelectionLength = selLen;

                RestoreFirstVisibleLine(log_richTextBox1, firstLine);
            }
        }

        // ---------- FORM LIFECYCLE ----------

        private void IrcLog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
