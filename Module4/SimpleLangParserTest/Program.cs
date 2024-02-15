using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleLangParser;
using SimpleLexer;

namespace SimpleLangParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileContents = @"~~ comment
set x = ENV(""PATH"")

<//  large
comment
... //>

sub Clear #
set exts = (""*.pub"", ""*.htaccess"") #
if [@1 is """"]                      #
  ""environment is missing"" | echo #
else                              #
  set paths = split(x, ';')       #
  0 | i                           #
  set N = len(paths)              #
  while [i < N]                   #
     set p = paths@i              #
	 exec(""cd "" + p)              #
	 exts | exec(""rm -rf ./"" + @@1)    #
  end while                       #
end if #
end sub

Clear(x)

sub Fib #
   if [@1 is 0 or @1 is 1] #
      return 1 #
   end if      #
   return Fib(@1 - 1) + Fib(@1 - 2) #
end sub

Fib(25) | echo
";
            TextReader inputReader = new StringReader(fileContents);
            Lexer l = new Lexer(inputReader);
            Parser p = new Parser(l);
            try
            {
                p.Program();
                if (l.LexKind == Tok.EOF)
                {
                    Console.WriteLine("Program successfully recognized");
                }
                else
                {
                    p.SyntaxError("end of file was expected");
                }
            }
            catch (ParserException e)
            {
                Console.WriteLine("lexer error: " + e.Message);
            }
            catch (LexerException le)
            {
                Console.WriteLine("parser error: " + le.Message);
            }
        }
    }
}
