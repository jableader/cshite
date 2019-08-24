using System;
using System.Collections.Generic;
using System.Linq;

namespace cshite.UI
{
    /// <summary>
    /// A centralised location for ascii art and formatting
    /// </summary>
    public static class Art
    {
        /// <summary>
        /// Adds a pretty little border around the supplied values
        /// </summary>
        public static IEnumerable<Renderable> AsHeader(string text, Renderable[] art = null)
        {
            var header = new[] { new Renderable { Text = text, Position = TextJustification.Center } }.Concat(BlankLines()).Concat(BorderHeading).Concat(BlankLines());
            return art?.Concat(header) ?? header;
        }

        /// <summary>
        /// The default top-line border
        /// </summary>
        public static Renderable[] Top() => BlankLines().Prepend(new Renderable { Text = "=", Border = "+", Repeat = true }).ToArray();

        /// <summary>
        /// The default bottom-line border
        /// </summary>
        public static Renderable[] Bottom() => BlankLines().Append(new Renderable { Text = "=", Border = "+", Repeat = true }).ToArray();

        /// <summary>
        /// The main divider between a heading and the screens content
        /// </summary>
        public static Renderable[] BorderHeading => new[] { new Renderable { Repeat = true, Text = 
@"
  _ 
_/ \".TrimStart(new [] { '\r', '\n' }) } };

        /// <summary>
        /// Simple, blank lines for formatting purposes
        /// </summary>
        public static Renderable[] BlankLines(int lines = 1) => new[] { new Renderable { Repeat = true, Text = string.Join("\r\n", Enumerable.Repeat(" ", lines)) } };

        /// <summary>
        /// A sexy ascii logo
        /// </summary>
        public static Renderable[] BitCoin
            => new[] { new Renderable { Forground = ConsoleColor.Yellow, Position = TextJustification.Center, Text = @"
     ===    
   |  $  |  
     ===    
            " } };

        /// <summary>
        /// A person sadly waving off a customer
        /// </summary>
        public static Renderable[] Goodbye
            => new[] { new Renderable { Position=TextJustification.Center, Text = @"
 \0 
  |\
 / \" } };

        /// <summary>
        /// A welcoming face
        /// </summary>
        public static Renderable[] Hello
            => new[] { new Renderable {  Position=TextJustification.Center,Text = @"
  0  0 
   <   
 \____/
" } };
    }
}
