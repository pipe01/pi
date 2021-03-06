﻿using Pi;
using Pi.Interpreter;
using Pi.Lexer;
using Pi.Parser;
using Pi.Parser.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Tester
{
    static class Program
    {
        static void Main(string[] args)
        {
            string src = File.ReadAllText("./src.pi");

            var lexer = new PiLexer(src);
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
            else
            {
                goto exit;
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
                    var line = src.Lines()[ex.Location.Line];
                    int tabCount = line.TakeWhile(o => o == '\t').Count();
                    line = new string(' ', tabCount) + line.TrimStart('\t');

                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(line);
                    Console.WriteLine(new string(' ', ex.Location.Column - 1) + "^");

                    goto exit;
                }
            }

            string dump = ObjectDumper.Dump(decl, DumpStyle.Console);
            Console.WriteLine(dump);
            File.WriteAllText("./dump.txt", dump);

            var inter = new PiInterpreter(decl);
            inter.Run();

            Console.WriteLine();
            Console.WriteLine("Done!");

        exit:
            Console.ReadKey(true);
        }

        private static string[] Lines(this string str)
        {
            return str.Replace("\r", "").Split('\n');
        }
    }
}
