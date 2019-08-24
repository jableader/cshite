using System;

namespace cshite.UI
{
    /// <summary>
    /// A container type for any element to be rendered to the screen
    /// </summary>
    public struct Renderable
    {
        public bool Repeat { get; set; }
        public string Border { get; set; }
        public string Text { get; set; }
        public TextJustification Position { get; set; }
        public ConsoleColor? Background { get; set; }
        public ConsoleColor? Forground { get; set; }
    }
}
