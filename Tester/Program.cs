using Pi;
using System;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer(@"let hell1o = 12.1;
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

            Console.ReadKey(true);
        }
    }
}
