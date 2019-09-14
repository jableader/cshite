using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace cshite.UI
{
    public enum TextJustification { Left, Center }
    public enum ResponseType { Cancel, Retry, Valid }

    /// <summary>
    /// A representation for any field to be displayed within the console window
    /// </summary>
    [DebuggerDisplay("Field: {Text}")]
    public class DisplayField
    {
        /// <summary>
        /// Override this when you need a response from the user. If the value is false the field will still be rendered but the console will provide a response.
        /// </summary>
        public virtual bool AcceptsInput => false;

        /// <summary>
        /// The renderable pieces for this display field.
        /// </summary>
        protected virtual IEnumerable<Renderable> Pieces { get; }

        protected internal DisplayField(params Renderable[] pieces)
            => Pieces = pieces;

        /// <summary>
        /// Override this method to read the response from the user and performs any validation
        /// </summary>
        /// <param name="message">The message to display to the user for a validation failure. Use null for no message.</param>
        /// <returns>Returns true when the user input was successfully accepted</returns>
        public virtual ResponseType ReadResponse(out string message)
        {
            throw new InvalidOperationException("A text only region cannot respond"); // Lets tell the developer when they've messed up
        }

        /// <summary>
        /// Render this displayfield, providing default values for some fields.
        /// </summary>
        public void Draw(Renderable console)
        {
            foreach (var piece in Pieces)
            {
                var longestLine = Console.WindowWidth - (2 * (piece.Border ?? console.Border).Length) - 2;
                var pieceText = ProcessPattern(piece, longestLine);

                foreach (var line in WrapTextIntoLines(pieceText, longestLine))
                {
                    PrintLine(line, longestLine, piece, console);
                }
            }
        }

        /// <summary>
        /// Wraps the provided text into substrings of length less than or equal to the provided length
        /// </summary>
        IEnumerable<string> WrapTextIntoLines(string text, int length)
        {
            foreach (var line in text.Split(Environment.NewLine))
            {
                var remaining = line;
                while (remaining.Length > length)
                {
                    yield return remaining.Substring(0, length);
                    remaining = remaining.Substring(length);
                }

                if (!string.IsNullOrEmpty(remaining))
                {
                    yield return remaining;
                }
            }
        }

        /// <summary>
        /// Prints a single line to the console, with borders and padding
        /// </summary>
        void PrintLine(string line, int longestLine, Renderable piece, Renderable console)
        {
            int rightPadding, leftPadding;
            if (piece.Position == TextJustification.Left)
            {
                leftPadding = 0;
                rightPadding = longestLine - line.Length;
            }
            else
            {
                var totalPadding = (longestLine - line.Length);
                leftPadding = totalPadding / 2;
                rightPadding = (totalPadding / 2) + (totalPadding % 2); // When the text is odd, the right padding will need 1 extra cell to center align
            }

            SetColors(console.Background.Value, console.Forground.Value);
            Console.Write(piece.Border ?? console.Border);
            Console.Write(new string(' ', leftPadding));

            SetColors(piece.Background ?? console.Background.Value, piece.Forground ?? console.Forground.Value);
            DrawConsoleText(line);

            SetColors(console.Background.Value, console.Forground.Value);
            Console.Write(new string(' ', rightPadding));
            Console.WriteLine(Reverse(piece.Border ?? console.Border));
        }

        /// <summary>
        /// Puts a Renderable's text value to the console
        /// </summary>
        protected virtual void DrawConsoleText(string s)
            => Console.Write(s);

        /// <summary>
        /// Reverse string s, such that "xyz" becomes "zyx"
        /// </summary>
        static string Reverse(string s)
            => string.Join(string.Empty, s.Reverse());

        /// <summary>
        /// Converts a renderable into its textual representation
        /// </summary>
        static string ProcessPattern(Renderable grouping, int longestLine)
            => grouping.Repeat ? RepeatPattern(grouping.Text, longestLine) : grouping.Text;

        /// <summary>
        /// Convert repeating patterns into their full-length line of text, handling newlines
        /// </summary>
        static string RepeatPattern(string patternToRepeat, int length)
            => patternToRepeat.Split("\r\n")
                .Select(subpattern => Repeat(subpattern, length))
                .Aggregate(new StringBuilder(), (builder, line) => builder.AppendLine(line))
                .ToString();

        /// <summary>
        /// Repeats a string until the desired length is reached
        /// </summary>
        static string Repeat(string subpattern, int length)
        {
            if (string.IsNullOrEmpty(subpattern))
                return new string(' ', length);

            var line = new StringBuilder();
            while (line.Length < length)
                line.Append(subpattern);

            return line.ToString(0, length); // Handles the case where the subpattern doesn't perfectly divide into the console width by truncating the RHS
        }

        protected static void SetColors(ConsoleColor background, ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
    }
}
