using System;
using System.Collections.Generic;
using System.Linq;

namespace cshite.UI
{
    public static class Art
    {
        public static IEnumerable<Renderable> AsHeader(string text, Renderable[] art = null)
        {
            var header = new[] { new Renderable { text = text, position = TextJustification.Center } }.Concat(BlankLines()).Concat(BorderHeading).Concat(BlankLines());
            return art?.Concat(header) ?? header;
        }

        public static Renderable[] Top() => BlankLines().Prepend(new Renderable { text = "=", border = "+", repeat = true }).ToArray();
        public static Renderable[] Bottom() => BlankLines().Append(new Renderable { text = "=", border = "+", repeat = true }).ToArray();
        public static Renderable[] BorderHeading => new[] { new Renderable { repeat = true, text = 
@"
  _ 
_/ \".TrimStart(new [] { '\r', '\n' }) } };
        public static Renderable[] BlankLines(int lines = 1) => new[] { new Renderable { repeat = true, text = string.Join("\r\n", Enumerable.Repeat(" ", lines)) } };
        public static Renderable[] BitCoin
            => new[] { new Renderable { forground = ConsoleColor.Yellow, position = TextJustification.Center, text = @"
     ===    
   |  $  |  
     ===    
            " } };

        public static Renderable[] Goodbye
            => new[] { new Renderable { position=TextJustification.Center, text = @"
\ 0 
  |\
 / \" } };

        public static Renderable[] Hello
            => new[] { new Renderable {  position=TextJustification.Center,text = @"
  0  0 
   <   
 \____/
" } };
    }
}
