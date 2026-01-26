using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class InlineLogger
{
    private readonly Action<string, Color> _appendOutput;

    /// <summary>
    /// Initializes a new instance of the InlineLogger class.
    /// </summary>
    /// <param name="appendOutput">The logging function to append output, e.g., raceLog.AppendLog.</param>
    public InlineLogger(Action<string, Color> appendOutput)
    {
        _appendOutput = appendOutput ?? throw new ArgumentNullException(nameof(appendOutput));
    }

    /// <summary>
    /// Logs a message with inline colors on a single line.
    /// </summary>
    /// <param name="segments">A list of tuples where each tuple contains the text and its corresponding color.</param>
    public void LogInline(List<(string text, Color color)> segments)
    {
        if (segments == null || segments.Count == 0)
        {
            throw new ArgumentException("Segments cannot be null or empty.", nameof(segments));
        }

        // Combine all text into a single string and append it inline
        foreach (var segment in segments)
        {
            _appendOutput(segment.text, segment.color);
        }

        // Optionally add a newline at the end for clarity
        _appendOutput(Environment.NewLine, Color.White);
    }

    /// <summary>
    /// Logs a single line of text in a specific color.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="color">The color of the message.</param>
    public void LogSingle(string message, Color color)
    {
        _appendOutput(message, color);
        _appendOutput(Environment.NewLine, Color.White);
    }
}
