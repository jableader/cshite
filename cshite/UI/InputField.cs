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
        protected override IEnumerable<Renderable> Pieces => new [] { new Renderable { text = prompt + DisplayResponse, background = background, forground = forground } }; // Include the response in the text so they are rendered together
        protected virtual string DisplayResponse => response.IsValid ? Response.ToString() : string.Empty; // The text to display for the users response

        public InputField(string prompt, Validator<T> parseResult, ConsoleColor background, ConsoleColor forground)
            : base()
        {
            this.prompt = prompt;
            this.validate = parseResult;
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


        protected override void DrawConsoleText(string s)
        {
            base.DrawConsoleText(s);

            top = Console.CursorTop;
            left = Console.CursorLeft;
        }

        protected virtual string ReadLine()
            => Console.ReadLine();
    }
}
