using System;
using System.Collections.Generic;
using System.Text;

namespace cshite.UI
{
    /// <summary>
    /// A function which handles the user response and returns a validated result
    /// </summary>
    public delegate Validated<T> Validator<T>(string response);

    /// <summary>
    /// The result type for any validation attempt
    /// </summary>
    /// <typeparam name="T">The type of result expected</typeparam>
    public struct Validated<T>
    {
        public T Value { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool IsValid => Type == ResponseType.Valid;
        public ResponseType Type { get; private set; }

        public static Validated<T> Error(string message = null)
            => new Validated<T> { Type = ResponseType.Retry, ErrorMessage = message };

        public static Validated<T> Success(T result)
            => new Validated<T> { Type = ResponseType.Valid, Value = result };

        public static Validated<T> Cancel()
            => new Validated<T> { Type = ResponseType.Cancel };
    }
}
