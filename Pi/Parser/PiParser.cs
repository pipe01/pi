using Pi.Lexer;
using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Expressions;
using Pi.Parser.Syntax.Statements;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Parser
{
    internal sealed partial class PiParser
    {
        private Lexeme[] Lexemes;

        private int Index;
        private Lexeme Current => Lexemes[Index];
        private SourceLocation Location => Current.Begin;

        private Lexeme NextNonWhitespace => Lexemes.Skip(Index).SkipWhile(o => o.Kind == LexemeKind.Whitespace).First();
        
        public IEnumerable<Node> Parse(IEnumerable<Lexeme> lexemes)
        {
            this.Index = 0;
            this.Lexemes = lexemes.ToArray();

            while (Current.Kind != LexemeKind.EndOfFile)
            {
                foreach (var item in ParseBlock(false))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<Node> ParseBlock(bool checkBraces)
        {
            if (checkBraces && !Take(LexemeKind.LeftBrace, out _))
                Error("Missing opening braces");

            while (true)
            {
                switch (Current.Kind)
                {
                    case LexemeKind.Keyword:
                        switch (Current.Content)
                        {
                            case "let":
                                yield return ParseVariableDeclaration();
                                break;
                            case "function":
                                yield return ParseFunctionDeclaration();
                                break;
                            case "if":
                                yield return ParseIfStatement();
                                break;
                            case "else":
                                yield return ParseElseStatement();
                                break;
                            case "class":
                                yield return ParseClassDeclaration();
                                break;
                            case "public":
                            case "private":
                                Advance();
                                break;
                            default:
                                Error($"Invalid keyword \"{Current.Content}\"");
                                break;
                        }
                        break;
                    case LexemeKind.RightBrace when checkBraces:
                    case LexemeKind.EndOfFile when !checkBraces:
                        Advance();
                        goto breakLoop;
                    case LexemeKind.Whitespace:
                    case LexemeKind.NewLine:
                        Advance();
                        break;
                    case LexemeKind.Identifier:
                        //Back();
                        var expr = ParseExpression();

                        if (expr is MethodCallExpression || (expr is BinaryExpression bin && bin.Operator == BinaryOperators.Assign))
                        {
                            if (NextNonWhitespace.Kind != LexemeKind.Semicolon)
                            {
                                Back();
                                MissingSemicolon();
                            }

                            yield return expr;
                            break;
                        }

                        goto default;
                    case LexemeKind.Semicolon:
                        Advance();
                        break;
                    default:
                        Error("Invalid lexeme found: " + Current);
                        break;
                }

            }
        breakLoop:;
        }

        private BlockStatement ParseBlock()
        {
            return new BlockStatement(Location, ParseBlock(true).ToArray());
        }

        private void Advance()
        {
            Index++;

            if (Index >= Lexemes.Length)
                Index = Lexemes.Length - 1;
        }
        
        private void AdvanceUntilNotWhitespace()
        {
            do Advance();
            while (Current.Kind == LexemeKind.Whitespace);
        }

        private void Back()
        {
            Index--;

            if (Index < 0)
                Index = 0;
        }

        private void BackUntilNotWhitespace()
        {
            do Back();
            while (Current.Kind == LexemeKind.Whitespace);
        }

        private void SkipWhitespaces(bool alsoNewlines = true)
        {
            while (Current.Kind == LexemeKind.Whitespace || (Current.Kind == LexemeKind.NewLine && alsoNewlines))
                Advance();
        }

        private void Error(string msg) => Error(msg, Location);
        private void Error(string msg, SourceLocation location)
        {
            throw new SyntaxException(msg, location);
        }

        private void MissingSemicolon() => Error("Missing semicolon");

        private Lexeme TakeAny()
        {
            var token = Current;
            Advance();
            return token;
        }

        private Lexeme Take(LexemeKind lexemeKind, bool ignoreWhitespace = true)
        {
            if (ignoreWhitespace)
                SkipWhitespaces();

            if (Current.Kind != lexemeKind)
                Error($"Unexpected lexeme: expected {lexemeKind}, found {Current}");

            return TakeAny();
        }

        private bool Take(LexemeKind lexemeKind, out Lexeme lexeme, bool ignoreWhitespace = true)
        {
            if (ignoreWhitespace)
                SkipWhitespaces();

            if (Current.Kind == lexemeKind)
            {
                lexeme = TakeAny();
                return true;
            }

            lexeme = null;
            return false;
        }

        private Lexeme TakeKeyword(string keyword, bool ignoreWhitespace = true, bool @throw = true)
        {
            if (ignoreWhitespace)
                SkipWhitespaces();

            if (Current.Kind != LexemeKind.Keyword || Current.Content != keyword)
            {
                if (@throw)
                    Error($"Unexpected lexeme: expected '{keyword}' keyword, found {Current}");
                else
                    return null;
            }

            return TakeAny();
        }

        private IEnumerable<Expression> TakeParameters()
        {
            var ret = new List<Expression>();

            do
            {
                var param = ParseExpression(false);

                if (param == null)
                    break;

                ret.Add(param);
            }
            while (Take(LexemeKind.Comma, out _));

            if (!Take(LexemeKind.RightParenthesis, out _))
                Error("Missing closing parentheses for method call");

            return ret;
        }

        private bool TakeType(out string type)
        {
            return (type = Take(LexemeKind.Identifier).Content) != null;
        }
    }
}
