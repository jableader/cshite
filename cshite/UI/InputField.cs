using System;
using System.Collections;
using System.Collections.Generic;

namespace cshite.UI
{
    /// <summary>
    /// Used for any field which requires a response from the user
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InputField<T> : DisplayField
    {
        /// <summary>
        /// The validated response from the user (null/default before Show has been called)
        /// </summary>
        public T Response => response.Value;

        readonly string prompt;
        readonly Validator<T> validate;
        readonly ConsoleColor forground, background;

        Validated<T> response;
        int top, left;

        public override bool AcceptsInput => !response.IsValid; // Don't accept input when the user has already responded
        protected override IEnumerable<Renderable> Pieces => new [] { new Renderable { Text = prompt + DisplayResponse, Background = background, Forground = forground } }; // Include the response in the text so they are rendered together
        protected virtual string DisplayResponse => response.IsValid ? Response.ToString() : string.Empty; // The text to display for the users response

        protected internal InputField(string prompt, Validator<T> validate, ConsoleColor background, ConsoleColor forground)
        {
            this.prompt = prompt;
            this.validate = validate;
            this.forground = forground;
            this.background = background;
        }

        /// <summary>
        /// Read the response from the user and run it through the validator
        /// </summary>
        public override ResponseType ReadResponse(out string message)
        {
            Console.SetCursorPosition(left, top);
            response = validate(ReadLine());
            message = response.ErrorMessage;
            return response.Type;
        }

        /// <summary>
        /// Override the console text to keep track of where the user input should be
        /// </summary>
        protected override void DrawConsoleText(string s)
        {
            base.DrawConsoleText(s);

            top = Console.CursorTop;
            left = Console.CursorLeft;
        }

        /// <summary>
        /// Reads the response from the user
        /// </summary>
        protected virtual string ReadLine()
            => Console.ReadLine();
    }
}
