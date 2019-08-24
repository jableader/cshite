using System;
using System.Text;

namespace cshite.UI
{
    /// <summary>
    /// A field for password input, complete with hidden character typing
    /// </summary>
    public class PasswordField : InputField<string>
    {
        const char PasswordChar = '*';

        protected override string DisplayResponse => new string(PasswordChar, Response?.Length ?? 0); // Don't display the password, just '*'s

        protected internal PasswordField(string label, ConsoleColor background, ConsoleColor forground)
            : base(label, Validate.AsString(), background, forground)
        {
        }

        /// <summary>
        /// Intercept the keys from the console to display '*'
        /// </summary>
        protected override string ReadLine()
        {
            var password = new StringBuilder();

            for (var key = Console.ReadKey(true); key.Key != ConsoleKey.Enter; key = Console.ReadKey(true)) // Keep reading until an Enter key
            {
                if (key.Key == ConsoleKey.Backspace && password.Length > 0) // Since we are manually managing keys we need to check for backspace
                {
                    password.Remove(password.Length - 1, 1); // Remove the last char
                    Console.Write("\b \b"); // Move the buffer back and blank out the previous PasswordChar
                }
                else if (IsValidPasschar(key.KeyChar))
                {
                    password.Append(key.KeyChar); // Add our users key and write a '*' to the output
                    Console.Write(PasswordChar);
                }
            }

            return password.ToString();
        }

        /// <summary>
        /// Helper method for what characters we would like to be valid for a password so we don't accidently add things like control characters
        /// </summary>
        bool IsValidPasschar(char c)
            => char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsWhiteSpace(c);
    }
}
