﻿using System;
using System.Collections.Generic;
using System.Text;

namespace cshite.UI
{
    /// <summary>
    /// A basic text-display field to show to the user
    /// </summary>
    class TextField : DisplayField
    {
        protected internal TextField(string text, TextJustification positioning, ConsoleColor backgroundColour, ConsoleColor forgoundColour)
            : base(new Renderable { Text = text, Position = positioning, Background = backgroundColour, Forground = forgoundColour }) { }
    }
}
