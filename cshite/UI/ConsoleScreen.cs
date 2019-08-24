using System;
using System.Collections.Generic;
using System.Linq;

namespace cshite.UI
{
    /// <summary>
    /// A pretty UI builder for the console
    /// </summary>
    class ConsoleScreen
    {
        readonly List<DisplayField> regions = new List<DisplayField>(); // The areas in this screen to render
        readonly ConsoleColor backgroundColor, foregroundColor; // The forground & background colors for headings & borders

        public ConsoleScreen(IEnumerable<Renderable> header = null, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor forgroundColor = ConsoleColor.Green)
        {
            this.backgroundColor = backgroundColor;
            this.foregroundColor = forgroundColor;

            if (header != null)
            {
                Add(header.ToArray());
            }
        }

        public ConsoleScreen(string header = null, ConsoleColor backgroundColor = ConsoleColor.Black, ConsoleColor forgroundColor = ConsoleColor.Green)
            : this(header == null ? null : Art.AsHeader(header), backgroundColor, forgroundColor)
        {
        }

        #region Public API for building the UI

        /// <summary>
        /// A simple text label to display to the user
        /// </summary>
        /// <param name="text">The content to display</param>
        /// <param name="positioning">The text justification (centered or left aligned)</param>
        /// <param name="backgroundColor">The forground color for the text</param>
        /// <param name="forgroundColor">The background color for the text</param>
        public TextField AddText(string text, TextJustification positioning = TextJustification.Left, ConsoleColor? backgroundColor = null, ConsoleColor? forgroundColor = null)
            => Add(new TextField(text, positioning, backgroundColor ?? this.backgroundColor, forgroundColor ?? this.foregroundColor)).Single();


        /// <summary>
        /// For asking queries to the user. The cursor will go through each question until each has a response.
        /// </summary>
        /// <param name="question">The content to display, eg "Age: "</param>
        /// <param name="parseResult">The validator to use <see cref="Validate"/></param>
        /// <param name="backgroundColor">The forground color for the text & response</param>
        /// <param name="forgroundColor">The background color for the text & response</param>
        public InputField<T> AddInput<T>(string question, Validator<T> parseResult, ConsoleColor? backgroundColor = null, ConsoleColor? forgroundColor = null)
            => Add(new InputField<T>(question, parseResult, backgroundColor ?? this.backgroundColor, forgroundColor ?? this.foregroundColor)).Single();

        /// <summary>
        /// Add a password field (a question which hides the typed response)
        /// </summary>
        /// <param name="question">The content to display, eg "Age: "</param>
        /// <param name="backgroundColor">The forground color for the text & response</param>
        /// <param name="forgroundColor">The background color for the text & response</param>
        /// <returns></returns>
        public PasswordField AddPassword(string question, ConsoleColor? backgroundColor = null, ConsoleColor? forgroundColor = null)
            => Add(new PasswordField(question, backgroundColor ?? this.backgroundColor, forgroundColor ?? this.foregroundColor)).Single();

        /// <summary>
        /// Add a simple blank line for formatting
        /// </summary>
        public void AddBlankLines(int numberOfLines = 1)
            => Add(Art.BlankLines(numberOfLines));

        /// <summary>
        /// Add a seperator
        /// </summary>
        public void AddSeperator(string pattern)
            => Add(new Renderable { Text = pattern, Repeat = true });

        /// <summary>
        /// Add arbitrary renderables
        /// </summary>
        void Add(params Renderable[] art)
            => Add(new DisplayField(art));

        T[] Add<T>(params T[] items) where T : DisplayField
        {
            regions.AddRange(items);
            return items;
        }

        /// <summary>
        /// Show the screen you have just built. Blocks until all questions have been filled.
        /// </summary>
        /// <returns>True once the user has filled all inputs with a valid response</returns>
        public bool Show()
        {
            for (var activeQuestion = GetNextInput(); activeQuestion != null; activeQuestion = GetNextInput()) // Display each question until they have all been answered
            {
                Render();

                switch (activeQuestion.ReadResponse(out var message))
                {
                    case ResponseType.Cancel: // User is sick of this screen and wants to cancel their operation
                        return false;

                    case ResponseType.Retry: // User entered an invalid response and we'd like to give them another crack at it
                        if (!string.IsNullOrEmpty(message))
                        {
                            ShowError("Error", message);
                        }
                        break;
                }
            }

            return true;
        }

        DisplayField GetNextInput()
            => regions.FirstOrDefault(r => r.AcceptsInput); // Find the first available question which is waiting for user input

        #endregion

        #region Rendering

        readonly DisplayField top = new DisplayField(Art.Top()), bottom = new DisplayField(Art.Bottom());

        /// <summary>
        /// Draw this screen by creating borders and asking all DisplayFields to render themselves
        /// </summary>
        void Render()
        {
            Console.Clear();
            var background = new Renderable { Background = this.backgroundColor, Forground = this.foregroundColor, Border = "| " };

            top.Draw(background);
            foreach (var line in regions)
            {
                line.Draw(background);
            }
            bottom.Draw(background);
        }

        #endregion

        #region Helper methods for common windows

        /// <summary>
        /// Show a basic error message.
        /// </summary>
        public static void ShowError(string header, string message)
            => ShowMessage(header, message, ConsoleColor.Black, ConsoleColor.Red);

        /// <summary>
        /// Show a message to the user, requiring them to hit enter to continue.
        /// </summary>
        public static void ShowMessage(string header, string message, ConsoleColor background = ConsoleColor.Black, ConsoleColor forground = ConsoleColor.Green)
        {
            var console = new ConsoleScreen(header, background, forground);
            console.AddText(message);
            console.AddBlankLines();
            console.AddInput("Press enter to continue...", Validate.AsString(), ConsoleColor.Green, ConsoleColor.Black);
            console.Show();
        }

        /// <summary>
        /// Ask the user for a yes/no confirmation
        /// </summary>
        /// <returns>Returns true when the user selected 'yes'</returns>
        public static bool ShowConfirmation(string header, string message, string question = "Would you like to proceed (y/n): ", ConsoleColor background = ConsoleColor.Black, ConsoleColor forground = ConsoleColor.Yellow)
        {
            var console = new ConsoleScreen(header, background, forground);
            console.AddText(message);
            console.AddBlankLines();

            var confirm = console.AddInput(question, Validate.Bool(), ConsoleColor.Black, ConsoleColor.Green);
            return console.Show() && confirm.Response;
        }

        #endregion
    }
}
