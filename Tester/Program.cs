using Pi;
using Pi.Parser.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Tester
{
    static class Program
    {
        static void Main(string[] args)
        {
            string src = @"let hello = asd.lol.nice(23, ""dab"", test.method(""calling"", 123));
let what = ""asd this is a string"";

function hello(param1, param2)
{
    let hello = ""idk"";
}

hello();";

            var lexer = new Lexer(src);
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

            IEnumerable<Node> decl = parser.Parse(l);

            if (Debugger.IsAttached)
            {
                decl = decl.ToArray();
            }
            else
            {
                try
                {
                    decl = decl.ToArray();
                }
                catch (SyntaxException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(src.Lines()[ex.Location.Line]);
                    Console.WriteLine(new string(' ', ex.Location.Column) + "^");

                    return;
                }
            }
            

            Console.WriteLine(ObjectDumper.Dump(decl, DumpStyle.Console));
            Console.ReadKey(true);
        }

        private static string[] Lines(this string str)
        {
            return str.Replace("\r", "").Split('\n');
        }
    }
}
