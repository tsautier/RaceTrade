using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RaceTrade
{
    /// <summary>
    /// Helper class to parse and display IRC color codes in RichTextBox
    /// </summary>
    public static class IrcColorParser
    {
        // mIRC color palette (0-15)
        private static readonly Color[] IrcColors = new Color[]
        {
    Color.White,       // 0
    Color.Black,       // 1
    Color.DarkBlue,    // 2
    Color.DarkGreen,   // 3
    Color.Red,         // 4
    Color.DarkRed,     // 5
    Color.DarkMagenta, // 6
    Color.Orange,      // 7
    Color.Yellow,      // 8
    Color.LightGreen,  // 9
    Color.DarkCyan,    // 10
    Color.Black, // 11  <-- or Color.DimGray, Color.White, whatever you like
    Color.Blue,        // 12
    Color.Magenta,     // 13
    Color.DarkGray,    // 14
    Color.LightGray    // 15
        };

        /// <summary>
        /// Appends IRC formatted text to RichTextBox with proper colors and formatting
        /// </summary>
        public static void AppendIrcText(RichTextBox richTextBox, string text, Color defaultColor)
        {
            if (string.IsNullOrEmpty(text))
                return;

            int pos = 0;
            Color currentForeColor = defaultColor;
            Color currentBackColor = richTextBox.BackColor;
            bool isBold = false;
            bool isItalic = false;
            bool isUnderline = false;

            while (pos < text.Length)
            {
                char c = text[pos];

                // Color code: \x03
                if (c == '\x03')
                {
                    pos++;
                    if (pos >= text.Length)
                        break;

                    // Try to parse foreground color (1-2 digits)
                    int foreColor = -1;
                    if (char.IsDigit(text[pos]))
                    {
                        string colorStr = text[pos].ToString();
                        pos++;
                        if (pos < text.Length && char.IsDigit(text[pos]))
                        {
                            colorStr += text[pos];
                            pos++;
                        }
                        foreColor = int.Parse(colorStr);
                    }

                    // Check for background color (comma + 1-2 digits)
                    int backColor = -1;
                    if (pos < text.Length && text[pos] == ',')
                    {
                        pos++;
                        if (pos < text.Length && char.IsDigit(text[pos]))
                        {
                            string colorStr = text[pos].ToString();
                            pos++;
                            if (pos < text.Length && char.IsDigit(text[pos]))
                            {
                                colorStr += text[pos];
                                pos++;
                            }
                            backColor = int.Parse(colorStr);
                        }
                    }

                    // Apply colors
                    if (foreColor >= 0 && foreColor < IrcColors.Length)
                        currentForeColor = IrcColors[foreColor];
                    else if (foreColor == -1 && backColor == -1)
                        currentForeColor = defaultColor; // Reset color

                    if (backColor >= 0 && backColor < IrcColors.Length)
                        currentBackColor = IrcColors[backColor];

                    continue;
                }

                // Bold: \x02
                if (c == '\x02')
                {
                    isBold = !isBold;
                    pos++;
                    continue;
                }

                // Italic: \x1D
                if (c == '\x1D')
                {
                    isItalic = !isItalic;
                    pos++;
                    continue;
                }

                // Underline: \x1F
                if (c == '\x1F')
                {
                    isUnderline = !isUnderline;
                    pos++;
                    continue;
                }

                // Reset: \x0F
                if (c == '\x0F')
                {
                    currentForeColor = defaultColor;
                    currentBackColor = richTextBox.BackColor;
                    isBold = false;
                    isItalic = false;
                    isUnderline = false;
                    pos++;
                    continue;
                }

                // Reverse video: \x16 (swap fore/back colors)
                if (c == '\x16')
                {
                    var temp = currentForeColor;
                    currentForeColor = currentBackColor;
                    currentBackColor = temp;
                    pos++;
                    continue;
                }

                // Regular character - collect run of text with same formatting
                int runStart = pos;
                while (pos < text.Length)
                {
                    char ch = text[pos];
                    if (ch == '\x03' || ch == '\x02' || ch == '\x1D' || ch == '\x1F' || ch == '\x0F' || ch == '\x16')
                        break;
                    pos++;
                }

                string textRun = text.Substring(runStart, pos - runStart);

                // Apply formatting and append
                richTextBox.SelectionStart = richTextBox.TextLength;
                richTextBox.SelectionColor = currentForeColor;
                richTextBox.SelectionBackColor = currentBackColor;

                FontStyle fontStyle = FontStyle.Regular;
                if (isBold) fontStyle |= FontStyle.Bold;
                if (isItalic) fontStyle |= FontStyle.Italic;
                if (isUnderline) fontStyle |= FontStyle.Underline;

                richTextBox.SelectionFont = new Font(richTextBox.Font, fontStyle);
                richTextBox.AppendText(textRun);
            }
        }

        /// <summary>
        /// Strips all IRC formatting codes for parsing (but not for display)
        /// </summary>
        public static string StripIrcFormatting(string text)
        {
            // Remove color codes: \x03[0-9]{1,2}(,[0-9]{1,2})?
            text = Regex.Replace(text, @"\x03\d{0,2}(,\d{0,2})?", "");

            // Remove bold, italic, underline, reverse, reset
            text = Regex.Replace(text, @"[\x02\x1D\x1F\x16\x0F]", "");

            return text;
        }
    }
}