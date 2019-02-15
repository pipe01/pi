using Pi;
using System;

namespace Tester
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer(@"let hell1o = asd.lol.;
let what = ""asd this is a string""[123, 2];

function hello(arg1, arg2)
{

}");
            var l = lexer.Lex();

            foreach (var error in lexer.Errors)
            {
                Console.WriteLine("{0} at line {1}, column {2}", error.Message, error.Location.Line, error.Location.Column);
            }

            if (l != null)
            {
                int prevLine = 0;

                foreach (var item in l)
                {
                    if (item.Begin.Line != prevLine)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Line " + item.Begin.Line);
                        prevLine = item.Begin.Line;
                    }

                    Console.WriteLine("{0,-15}: \"{1}\"", item.Kind, item.Content);
                }
            }

            var parser = new PiParser(l);

            var decl = parser.ParseDeclaration();

            Console.ReadKey(true);
        }
    }
}
