using Pi;
using System;

namespace Tester
{
    static class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer(@"let hello = asd.lol.nice(23, ""dab"", test.method(""calling"", 123));
let what = ""asd this is a string"";

function hello(param1, param2)
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

            var parser = new PiParser();

            var decl = parser.Parse(l);

            Console.WriteLine(ObjectDumper.Dump(decl, DumpStyle.Console));
            Console.ReadKey(true);
        }
    }
}
