using System;
using System.Collections.Generic;
using System.Text;

namespace cshite.UI
{
    /// <summary>
    /// A basic text-display field to show to the user
    /// </summary>
    class TextField : DisplayField
    {
        public TextField(string text, TextJustification positioning, ConsoleColor backgroundColour, ConsoleColor forgoundColour)
            : base(text, positioning, backgroundColour, forgoundColour) { }

        public override ResponseType ReadResponse(out string message)
        {
            throw new InvalidOperationException("A text only region cannot respond");
        }
    }
}
