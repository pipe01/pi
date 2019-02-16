using Pi.Lexer;
using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Declarations;
using Pi.Parser.Syntax.Expressions;
using Pi.Parser.Syntax.Statements;
using System.Collections.Generic;
using System.Linq;

namespace Pi
{
    internal class PiParser
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

        private void SkipWhitespaces(bool alsoNewlines = true)
        {
            while (Current.Kind == LexemeKind.Whitespace || (Current.Kind == LexemeKind.NewLine && alsoNewlines))
                Advance();
        }

        private void Error(string msg)
        {
            throw new SyntaxException(msg, Current.Begin);
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

        private IEnumerable<ParameterDeclaration> TakeParameterDeclarations()
        {
            var ret = new List<ParameterDeclaration>();

            do
            {
                var param = Take(LexemeKind.Identifier);

                if (param == null)
                    break;

                if (!Take(LexemeKind.Colon, out _) || !TakeType(out var type))
                    Error("Missing type for parameter");
                else
                    ret.Add(new ParameterDeclaration(Location, param.Content, type));
            }
            while (Take(LexemeKind.Comma, out _));

            if (!Take(LexemeKind.RightParenthesis, out _))
                Error("Missing closing parentheses for method declaration");

            return ret;
        }

        private Expression ParseExpression(bool @throw = true)
        {
            bool startsWithParenthesis = Take(LexemeKind.LeftParenthesis, out _);

            Expression ret = null;

            if (startsWithParenthesis)
            {
                ret = ParseExpression();

                if (NextNonWhitespace.Kind != LexemeKind.RightParenthesis)
                    Error("Missing closing parentheses");

                Advance();

                goto exit;
            }

            //Try to parse literal
            if (Take(LexemeKind.StringLiteral, out var str))
                ret = new ConstantExpression(Location, str.Content, ConstantKind.String);
            else if (Take(LexemeKind.IntegerLiteral, out var @int))
                ret = new ConstantExpression(Location, @int.Content, ConstantKind.Integer);
            else if (Take(LexemeKind.DecimalLiteral, out var dec))
                ret = new ConstantExpression(Location, dec.Content, ConstantKind.Decimal);
            else if (TakeKeyword("true", @throw: false) != null)
                ret = new ConstantExpression(Location, "true", ConstantKind.Boolean);
            else if (TakeKeyword("false", @throw: false) != null)
                ret = new ConstantExpression(Location, "false", ConstantKind.Boolean);

            if (ret != null)
                goto exit;
            
            //Try to parse method call
            var references = new List<Expression>();

            //Take identifiers and dots
            while (Take(LexemeKind.Identifier, out var identifier))
            {
                references.Add(new IdentifierExpression(Location, identifier.Content));

                if (!Take(LexemeKind.Dot, out _)) //Found a non-dot, break
                    break;

                if (NextNonWhitespace.Kind != LexemeKind.Identifier) //Found a dot withot an identifier next to it, error
                    Error("Expected identifier after dot");
            }

            if (references.Count > 0)
            {
                ret = new ReferenceExpression(Location, references);

                goto exit;
            }

            if (@throw)
                Error($"Expected expression, got {NextNonWhitespace}");

        exit:
            //Check for binary operator
            BinaryOperators? op = null;

            switch (NextNonWhitespace.Kind)
            {
                case LexemeKind.Plus:
                    op = BinaryOperators.Add;
                    break;
                case LexemeKind.Minus:
                    op = BinaryOperators.Subtract;
                    break;
                case LexemeKind.Mult:
                    op = BinaryOperators.Multiply;
                    break;
                case LexemeKind.Div:
                    op = BinaryOperators.Divide;
                    break;
                case LexemeKind.EqualsAssign:
                    op = BinaryOperators.Assign;
                    break;
            }

            if (op != null)
            {
                Advance();
                AdvanceUntilNotWhitespace();

                var left = ret;
                var right = ParseExpression();

                return new BinaryExpression(Location, left, right, op.Value);
            }

            if (ret is ReferenceExpression reference && Take(LexemeKind.LeftParenthesis, out _))
            {
                ret = new MethodCallExpression(Location, TakeParameters(), reference);
            }

            return ret;
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            var let = TakeKeyword("let");
            var name = Take(LexemeKind.Identifier);
            VariableDeclaration ret;
            string type = null;

            if ((!Take(LexemeKind.Colon, out _) || !TakeType(out type)) && NextNonWhitespace.Kind != LexemeKind.EqualsAssign)
                Error("Missing variable type");

            if (Take(LexemeKind.EqualsAssign, out _))
            {
                var value = ParseExpression();

                ret = new VariableDeclaration(Location, name.Content, value, type);
            }
            else
            {
                if (Current.Kind != LexemeKind.Semicolon)
                    Error("Invalid variable name");

                ret = new VariableDeclaration(Location, name.Content, null, type);
            }

            if (!Take(LexemeKind.Semicolon, out _))
                MissingSemicolon();

            return ret;
        }

        private FunctionDeclaration ParseFunctionDeclaration()
        {
            var func = TakeKeyword("function");
            var name = Take(LexemeKind.Identifier);

            if (!Take(LexemeKind.LeftParenthesis, out _))
                Error("Missing opening parenthesis for method declaration");

            var @params = TakeParameterDeclarations();
            string type = null;

            if (Take(LexemeKind.Colon, out _) && !TakeType(out type))
            {
                Error("Type expected after colon");
            }

            var body = ParseBlock(true).ToArray();

            return new FunctionDeclaration(Location, name.Content, @params, body, type);
        }

        private IfStatement ParseIfStatement()
        {
            var @if = TakeKeyword("if");
            Take(LexemeKind.LeftParenthesis);

            var cond = ParseExpression();

            Take(LexemeKind.RightParenthesis);

            return new IfStatement(Location, cond, new BlockStatement(Location, ParseBlock(true).ToArray()));
        }
    }
}
