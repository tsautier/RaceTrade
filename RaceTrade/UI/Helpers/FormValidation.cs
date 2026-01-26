using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RaceTrade
{
    /// <summary>
    /// Provides reusable validation methods for Windows Forms.
    /// Includes visual feedback (color-coded fields) and error collection.
    /// </summary>
    public static class FormValidation
    {
        private static readonly Color ErrorColor = Color.FromArgb(255, 200, 200); // Light red
        private static readonly Color WarningColor = Color.FromArgb(255, 255, 200); // Light yellow
        private static readonly Color ValidColor = SystemColors.Window;

        /// <summary>
        /// Validates that a TextBox is not empty.
        /// </summary>
        public static bool ValidateNotEmpty(TextBox textBox, string fieldName, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = $"{fieldName} cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a TextBox contains a valid port number (1-65535).
        /// </summary>
        public static bool ValidatePort(TextBox textBox, out string error)
        {
            if (!int.TryParse(textBox.Text, out int port) || port < 1 || port > 65535)
            {
                error = "Port must be a number between 1 and 65535.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a TextBox contains a valid regular expression.
        /// </summary>
        public static bool ValidateRegex(TextBox textBox, string fieldName, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = $"{fieldName} cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            try
            {
                new Regex(textBox.Text);
                textBox.BackColor = ValidColor;
                error = null;
                return true;
            }
            catch (ArgumentException)
            {
                error = $"{fieldName} contains an invalid regular expression.";
                textBox.BackColor = ErrorColor;
                return false;
            }
        }

        /// <summary>
        /// Validates that a TextBox contains a valid hostname, IP address, or URL.
        /// Supports: domain.com, 192.168.1.1, https://domain.com, http://192.168.1.1
        /// </summary>
        public static bool ValidateHostname(TextBox textBox, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = "Hostname/IP cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            string input = textBox.Text.Trim();

            // Try to parse as URI if it has a protocol
            if (input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                input.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                if (Uri.TryCreate(input, UriKind.Absolute, out Uri uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    textBox.BackColor = ValidColor;
                    error = null;
                    return true;
                }

                error = "Invalid URL format.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            // Otherwise validate as hostname or IP
            bool isValidIp = System.Net.IPAddress.TryParse(input, out _);
            bool isValidHostname = Regex.IsMatch(input, @"^[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?)*$");

            if (!isValidIp && !isValidHostname)
            {
                error = "Invalid hostname or IP address format.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a TextBox contains a valid integer within a range.
        /// </summary>
        public static bool ValidateInteger(TextBox textBox, string fieldName, int min, int max, out string error)
        {
            if (!int.TryParse(textBox.Text, out int value))
            {
                error = $"{fieldName} must be a valid number.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            if (value < min || value > max)
            {
                error = $"{fieldName} must be between {min} and {max}.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a ComboBox has a selected item.
        /// </summary>
        public static bool ValidateComboBoxSelection(ComboBox comboBox, string fieldName, out string error)
        {
            if (comboBox.SelectedIndex == -1)
            {
                error = $"Please select a {fieldName}.";
                comboBox.BackColor = ErrorColor;
                return false;
            }

            comboBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that at least one CheckBox in a group is checked.
        /// </summary>
        public static bool ValidateCheckBoxGroup(CheckBox[] checkBoxes, string groupName, out string error)
        {
            if (!checkBoxes.Any(cb => cb.Checked))
            {
                error = $"Please select at least one option in {groupName}.";
                return false;
            }

            error = null;
            return true;
        }

        /// <summary>
        /// Validates a password meets minimum requirements.
        /// </summary>
        public static bool ValidatePassword(TextBox textBox, int minLength, out string error, bool requireSpecialChars = false)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = "Password cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            if (textBox.Text.Length < minLength)
            {
                error = $"Password must be at least {minLength} characters long.";
                textBox.BackColor = WarningColor;
                return false;
            }

            if (requireSpecialChars)
            {
                bool hasUpper = textBox.Text.Any(char.IsUpper);
                bool hasLower = textBox.Text.Any(char.IsLower);
                bool hasDigit = textBox.Text.Any(char.IsDigit);
                bool hasSpecial = textBox.Text.Any(c => !char.IsLetterOrDigit(c));

                if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
                {
                    error = "Password must contain uppercase, lowercase, digit, and special character.";
                    textBox.BackColor = WarningColor;
                    return false;
                }
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Displays a list of validation errors in a message box.
        /// </summary>
        public static void ShowValidationErrors(List<string> errors)
        {
            if (errors == null || errors.Count == 0)
                return;

            string errorMessage = "Please fix the following errors:\n\n" +
                                string.Join("\n", errors.Select((e, i) => $"{i + 1}. {e}"));

            MessageBox.Show(
                errorMessage,
                "Validation Errors",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Resets all TextBox backgrounds to default (clears validation colors).
        /// </summary>
        public static void ResetValidationColors(Form form)
        {
            ResetControlColors(form);
        }

        private static void ResetControlColors(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.BackColor = ValidColor;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.BackColor = ValidColor;
                }

                // Recursively reset child controls
                if (control.HasChildren)
                {
                    ResetControlColors(control);
                }
            }
        }

        /// <summary>
        /// Validates all required fields in a form and returns a list of errors.
        /// </summary>
        public static List<string> ValidateForm(params (Func<bool> validator, string errorMessage)[] validations)
        {
            var errors = new List<string>();

            foreach (var (validator, errorMessage) in validations)
            {
                if (!validator())
                {
                    errors.Add(errorMessage);
                }
            }

            return errors;
        }

        /// <summary>
        /// Adds tooltip hints to controls for better UX.
        /// </summary>
        public static void AddTooltip(Control control, string tooltipText)
        {
            var tooltip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 100,
                ShowAlways = true
            };

            tooltip.SetToolTip(control, tooltipText);
        }

        /// <summary>
        /// Validates an email address format.
        /// </summary>
        public static bool ValidateEmail(TextBox textBox, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = "Email cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            // Simple email validation regex
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(textBox.Text, emailPattern))
            {
                error = "Invalid email format.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a file path exists.
        /// </summary>
        public static bool ValidateFilePath(TextBox textBox, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = "File path cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            if (!System.IO.File.Exists(textBox.Text))
            {
                error = "File does not exist.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }

        /// <summary>
        /// Validates that a directory path exists.
        /// </summary>
        public static bool ValidateDirectoryPath(TextBox textBox, out string error)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                error = "Directory path cannot be empty.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            if (!System.IO.Directory.Exists(textBox.Text))
            {
                error = "Directory does not exist.";
                textBox.BackColor = ErrorColor;
                return false;
            }

            textBox.BackColor = ValidColor;
            error = null;
            return true;
        }
    }
}