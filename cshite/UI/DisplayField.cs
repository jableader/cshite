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

        protected virtual IEnumerable<Renderable> Pieces { get; }

        public DisplayField(params Renderable[] pieces)
            => Pieces = pieces;

        protected DisplayField(string text, TextJustification positioning = TextJustification.Left, ConsoleColor? backgroundColour = null, ConsoleColor? forgoundColour = null)
            : this (new Renderable { text = text, position = positioning, background = backgroundColour, forground = forgoundColour })
        {
        }

        /// <summary>
        /// Reads the response from the user and performs any validation
        /// </summary>
        /// <param name="message">The message to display to the user for a validation failure. Use null for no message.</param>
        /// <returns>Returns true when the user input was successfully accepted</returns>
        public virtual ResponseType ReadResponse(out string message)
        {
            message = null;
            return ResponseType.Valid;
        }

        public void Draw(Renderable console)
        {
            foreach (var piece in Pieces)
            {
                var longestLine = Console.WindowWidth - (2 * (piece.border ?? console.border).Length);
                var pieceText = ProcessPattern(piece, longestLine);

                foreach (var line in WrapTextIntoLines(pieceText, longestLine))
                {
                    PrintLine(line, longestLine, piece, console);
                }
            }
        }

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

        void PrintLine(string line, int longestLine, Renderable piece, Renderable console)
        {
            int rightPadding, leftPadding;
            if (piece.position == TextJustification.Left)
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

            SetColors(console.forground.Value, console.background.Value);
            Console.Write(piece.border ?? console.border);
            Console.Write(new string(' ', leftPadding));

            SetColors(piece.forground ?? console.forground.Value, piece.background ?? console.background.Value);
            DrawConsoleText(line);

            SetColors(console.forground.Value, console.background.Value);
            Console.Write(new string(' ', rightPadding));
            Console.WriteLine(Reverse(piece.border ?? console.border));
        }

        protected virtual void DrawConsoleText(string s)
            => Console.Write(s);

        static string Reverse(string s)
            => string.Join(string.Empty, s.Reverse());

        static string ProcessPattern(Renderable grouping, int longestLine)
            => grouping.repeat ? RepeatPattern(grouping.text, longestLine) : grouping.text;

        static string RepeatPattern(string patternToRepeat, int length)
            => patternToRepeat.Split("\r\n")
                .Select(subpattern => Repeat(subpattern, length))
                .Aggregate(new StringBuilder(), (builder, line) => builder.AppendLine(line))
                .ToString();

        static string Repeat(string subpattern, int length)
        {
            if (string.IsNullOrEmpty(subpattern))
                return new string(' ', length);

            var line = new StringBuilder();
            while (line.Length < length)
                line.Append(subpattern);

            return line.ToString(0, length);
        }

        public static void SetColors(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }
    }
}
