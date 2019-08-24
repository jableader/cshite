using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cshite.UI
{
    public struct Renderable
    {
        public bool repeat;
        public string border;
        public string text;
        public TextJustification position;
        public ConsoleColor? background;
        public ConsoleColor? forground;

        public int LongestLine(string border)
            => Console.WindowWidth - (2 * (this.border ?? border ?? string.Empty).Length);

        public static implicit operator Renderable((bool repeat, string pattern) tpl)
            => new Renderable { repeat=tpl.repeat, text=tpl.pattern };
    }
}
