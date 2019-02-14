using System;
using System.Collections.Generic;
using System.Linq;

namespace Pi
{
    public class Lexer
    {
        private static readonly string[] Keywords = { "let", "function" };

        private string Source;

        private int Column;
        private int Index;
        private int Line = 1;
        private char Char => Source[Index];

        private SourceLocation Location => new SourceLocation(Line, Column, Index);

        private bool IsEOF => Char == '\0';
        private bool IsLetter => char.IsLetter(Char);
        private bool IsDigit => char.IsDigit(Char);
        private bool IsNewLine => Char == '\n';
        private bool IsSymbol => "<>{}()[]!%^&*+-=/.,?;:|".Contains(Char);
        private bool IsWhitespace => Char == ' ';
        private bool IsKeyword => Keywords.Contains(Buffer);

        private string Buffer = "";

        public ErrorSink Errors { get; } = new ErrorSink();

        public Lexer(string source)
        {
            this.Source = source.Replace("\r\n", "\n");

            if (this.Source.Last() != '\0')
                this.Source += "\0";
        }

        public LinkedList<Lexeme> Lex()
        {
            var lexList = new LinkedList<Lexeme>();

            while (!IsEOF)
            {
                var lexeme = GetLexeme();

                if (lexeme == null)
                    return null;

                lexList.AddLast(lexeme);
            }

            lexList.AddLast(Lexeme(LexemeKind.EndOfFile));

            return lexList;
        }

        private void Advance()
        {
            Index++;
            Column++;
        }

        private void NewLine()
        {
            Line++;
            Column = 0;
            Index++;
        }

        private void Consume()
        {
            Buffer += Char;
            Advance();
        }

        private void Back()
        {
            if (Buffer.Length > 0)
                Buffer = Buffer.Substring(0, Buffer.Length - 1);
            Index--;
            Column--;

            if (Column < 0)
            {
                Column = 0;
                Line--;
            }
        }

        private Lexeme Lexeme(LexemeKind kind, string content = null)
        {
            content = content ?? Buffer;
            Buffer = "";

            var begin = Location;
            var end = new SourceLocation(Line, Column + content.Length, Index + content.Length);

            return new Lexeme(kind, begin, end, content);
        }

        private void Error(string msg, Severity severity)
        {
            Errors.Add(new Error(Location, msg, severity));
        }


        private Lexeme GetLexeme()
        {
            if (IsDigit)
            {
                return DoNumber();
            }
            else if (IsNewLine)
            {
                var lexeme = Lexeme(LexemeKind.NewLine, "");
                NewLine();
                return lexeme;
            }
            else if (IsWhitespace)
            {
                return DoWhitespace();
            }
            else if (Char == '"')
            {
                return DoStringLiteral();
            }
            else if (IsSymbol)
            {
                return DoSymbol();
            }

            return DoWord();
        }

        private Lexeme DoNumber()
        {
            bool isDecimal = false;

            while (IsDigit)
            {
                Consume();

                if (Char == '.')
                {
                    isDecimal = true;
                    Consume();

                    if (!IsDigit)
                    {
                        Error("Must contain digits after '.'", Severity.Error);
                        return null;
                    }
                }
            }

            return Lexeme(isDecimal ? LexemeKind.DecimalLiteral : LexemeKind.IntegerLiteral);
        }

        private Lexeme DoWhitespace()
        {
            while (IsWhitespace)
                Consume();

            return Lexeme(LexemeKind.Whitespace);
        }

        private Lexeme DoStringLiteral()
        {
            Advance();

            while (Char != '"')
            {
                if (Char == '\0')
                {
                    Error("Unexpected end of file", Severity.Error);
                    return null;
                }

                Consume();
            }

            if (Char != '"')
            {
                Error("Missing end quote for string literal", Severity.Error);
                return null;
            }

            Advance();

            return Lexeme(LexemeKind.StringLiteral);
        }

        private Lexeme DoSymbol()
        {
            char ch = Char;
            Consume();

            switch (ch)
            {
                case ';':
                    return Lexeme(LexemeKind.Semicolon);
                case ',':
                    return Lexeme(LexemeKind.Comma);
                case '.':
                    return Lexeme(LexemeKind.Dot);
                case '=':
                    return Lexeme(LexemeKind.Equals);
                case '!':
                    Consume();
                    if (Char == '=')
                        return Lexeme(LexemeKind.NotEqual);

                    Back();
                    break;
                case '(':
                    return Lexeme(LexemeKind.LeftParenthesis);
                case ')':
                    return Lexeme(LexemeKind.RightParenthesis);
                case '[':
                    return Lexeme(LexemeKind.LeftBracket);
                case ']':
                    return Lexeme(LexemeKind.RightBracket);
                case '{':
                    return Lexeme(LexemeKind.LeftBrace);
                case '}':
                    return Lexeme(LexemeKind.RightBrace);
                case '?':
                    return Lexeme(LexemeKind.QuestionMark);
                case ':':
                    return Lexeme(LexemeKind.Colon);
                case '+':
                    return Lexeme(LexemeKind.Plus);
                case '-':
                    return Lexeme(LexemeKind.Minus);
                case '/':
                    return Lexeme(LexemeKind.Div);
                case '*':
                    return Lexeme(LexemeKind.Mult);
                case '<':
                    Consume();
                    if (Char == '=')
                        return Lexeme(LexemeKind.LessThanOrEqual);

                    Back();
                    return Lexeme(LexemeKind.LessThan);
                case '>':
                    Consume();
                    if (Char == '=')
                        return Lexeme(LexemeKind.GreaterThanOrEqual);

                    Back();
                    return Lexeme(LexemeKind.GreaterThan);
            }

            Back();
            Error("Invalid symbol", Severity.Error);
            return null;
        }

        private Lexeme DoWord()
        {
            int i = 0;
            while (IsLetter || (i > 0 && IsDigit)) //Don't allow a digit as the first character
            {
                Consume();
                i++;
            }

            return Lexeme(IsKeyword ? LexemeKind.Keyword : LexemeKind.Identifier);
        }
    }
}
