using System;
using System.Drawing;
using System.Windows.Forms;

namespace RaceTrade
{
    /// <summary>
    /// Provides consistent, reusable dialog methods throughout the application.
    /// Eliminates repetitive MessageBox calls with inconsistent styling.
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Shows a confirmation dialog with Yes/No buttons.
        /// </summary>
        /// <param name="message">The question to ask the user</param>
        /// <param name="title">Dialog title (default: "Confirm")</param>
        /// <returns>DialogResult.Yes or DialogResult.No</returns>
        public static DialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows a confirmation dialog with Yes/No/Cancel buttons.
        /// </summary>
        public static DialogResult ShowConfirmationWithCancel(string message, string title = "Confirm")
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);
        }

        /// <summary>
        /// Shows an error message dialog.
        /// </summary>
        public static void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows an error message with exception details.
        /// </summary>
        public static void ShowError(Exception ex, string context = null)
        {
            string message = context != null
                ? $"{context}\n\nError: {ex.Message}"
                : $"Error: {ex.Message}";

            MessageBox.Show(
                message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a success message dialog.
        /// </summary>
        public static void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a warning message dialog.
        /// </summary>
        public static void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows an informational message dialog.
        /// </summary>
        public static void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a custom input dialog box.
        /// </summary>
        /// <param name="prompt">The prompt text</param>
        /// <param name="title">Dialog title</param>
        /// <param name="defaultValue">Default text in the input box</param>
        /// <returns>User input string, or null if cancelled</returns>
        public static string ShowInputDialog(string prompt, string title = "Input", string defaultValue = "")
        {
            Form inputForm = new Form
            {
                Width = 450,
                Height = 160,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(22, 26, 36)
            };

            Label label = new Label
            {
                Left = 15,
                Top = 20,
                Text = prompt,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            TextBox textBox = new TextBox
            {
                Left = 15,
                Top = 50,
                Width = 400,
                Text = defaultValue,
                BackColor = Color.FromArgb(13, 16, 24),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            Button confirmButton = new Button
            {
                Text = "OK",
                Left = 240,
                Width = 80,
                Top = 85,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 168, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            confirmButton.FlatAppearance.BorderSize = 0;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 335,
                Width = 80,
                Top = 85,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(37, 42, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9)
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            confirmButton.Click += (s, e) => inputForm.Close();
            cancelButton.Click += (s, e) => inputForm.Close();

            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(confirmButton);
            inputForm.Controls.Add(cancelButton);
            inputForm.AcceptButton = confirmButton;
            inputForm.CancelButton = cancelButton;

            return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        /// <summary>
        /// Shows a multi-line input dialog box.
        /// </summary>
        public static string ShowMultilineInputDialog(string prompt, string title = "Input", string defaultValue = "")
        {
            Form inputForm = new Form
            {
                Width = 500,
                Height = 350,
                FormBorderStyle = FormBorderStyle.Sizable,
                Text = title,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = true,
                MinimizeBox = false,
                BackColor = Color.FromArgb(22, 26, 36)
            };

            Label label = new Label
            {
                Left = 15,
                Top = 15,
                Text = prompt,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            TextBox textBox = new TextBox
            {
                Left = 15,
                Top = 45,
                Width = 450,
                Height = 220,
                Text = defaultValue,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(13, 16, 24),
                ForeColor = Color.White,
                Font = new Font("Cascadia Mono", 9)
            };

            Button confirmButton = new Button
            {
                Text = "OK",
                Left = 290,
                Width = 80,
                Top = 275,
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 168, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            confirmButton.FlatAppearance.BorderSize = 0;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 385,
                Width = 80,
                Top = 275,
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(37, 42, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            confirmButton.Click += (s, e) => inputForm.Close();
            cancelButton.Click += (s, e) => inputForm.Close();

            inputForm.Controls.Add(label);
            inputForm.Controls.Add(textBox);
            inputForm.Controls.Add(confirmButton);
            inputForm.Controls.Add(cancelButton);
            inputForm.AcceptButton = confirmButton;
            inputForm.CancelButton = cancelButton;

            return inputForm.ShowDialog() == DialogResult.OK ? textBox.Text : null;
        }

        /// <summary>
        /// Shows a progress dialog for long-running operations.
        /// </summary>
        public static ProgressDialog ShowProgressDialog(string title, string message)
        {
            return new ProgressDialog(title, message);
        }

        /// <summary>
        /// Shows a file save dialog with common file filters.
        /// </summary>
        public static string ShowSaveFileDialog(string filter = "All Files|*.*", string defaultFileName = "")
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = filter;
                dialog.FileName = defaultFileName;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        /// <summary>
        /// Shows a file open dialog with common file filters.
        /// </summary>
        public static string ShowOpenFileDialog(string filter = "All Files|*.*", bool multiSelect = false)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = filter;
                dialog.Multiselect = multiSelect;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
            }
        }

        /// <summary>
        /// Shows a folder browser dialog.
        /// </summary>
        public static string ShowFolderBrowserDialog(string description = "Select a folder")
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = description;
                dialog.ShowNewFolderButton = true;

                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
            }
        }

        /// <summary>
        /// Shows a deletion confirmation with strong warning.
        /// </summary>
        public static DialogResult ShowDeleteConfirmation(string itemName)
        {
            return MessageBox.Show(
                $"Are you sure you want to delete '{itemName}'?\n\nThis action cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Shows a retry dialog for failed operations.
        /// </summary>
        public static DialogResult ShowRetryDialog(string operation, string error)
        {
            return MessageBox.Show(
                $"Failed to {operation}:\n\n{error}\n\nWould you like to retry?",
                "Operation Failed",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// A reusable progress dialog for long-running operations.
    /// </summary>
    public class ProgressDialog : Form
    {
        private ProgressBar progressBar;
        private Label statusLabel;
        private Button cancelButton;
        private bool cancelRequested = false;

        public bool CancelRequested => cancelRequested;

        public ProgressDialog(string title, string message)
        {
            this.Text = title;
            this.Width = 450;
            this.Height = 180;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(22, 26, 36);

            statusLabel = new Label
            {
                Text = message,
                Left = 20,
                Top = 20,
                Width = 390,
                Height = 40,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9)
            };

            progressBar = new ProgressBar
            {
                Left = 20,
                Top = 70,
                Width = 390,
                Height = 25,
                Style = ProgressBarStyle.Continuous
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                Left = 165,
                Top = 105,
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(37, 42, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) =>
            {
                cancelRequested = true;
                cancelButton.Enabled = false;
                statusLabel.Text = "Cancelling...";
            };

            this.Controls.Add(statusLabel);
            this.Controls.Add(progressBar);
            this.Controls.Add(cancelButton);

            ThemeManager.EnableDarkTitleBar(this);
        }

        public void UpdateProgress(int value, string status = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateProgress(value, status)));
                return;
            }

            progressBar.Value = Math.Min(Math.Max(value, 0), 100);
            if (status != null)
            {
                statusLabel.Text = status;
            }
        }

        public void SetIndeterminate(bool indeterminate)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => SetIndeterminate(indeterminate)));
                return;
            }

            progressBar.Style = indeterminate ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
        }
    }
}