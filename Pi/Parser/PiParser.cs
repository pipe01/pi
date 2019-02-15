﻿using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Declarations;
using Pi.Parser.Syntax.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Pi
{
    public class PiParser
    {
        private readonly Lexeme[] Lexemes;

        private int Index;
        private Lexeme Current => Lexemes[Index];

        private Lexeme NextNonWhitespace => Lexemes.Skip(Index).SkipWhile(o => o.Kind == LexemeKind.Whitespace).First();

        public PiParser(IEnumerable<Lexeme> lexemes)
        {
            this.Lexemes = lexemes.ToArray();
        }

        private void Advance()
        {
            Index++;
        }

        private void Back()
        {
            Index--;
        }

        private void SkipWhitespaces()
        {
            while (Current.Kind == LexemeKind.Whitespace)
                Advance();
        }

        private void Error(string msg)
        {
            throw new SyntaxException(msg, Current.Begin);
        }

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

        private Lexeme TakeKeyword(string keyword, bool @throw = true)
        {
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
            while (true)
            {
                var param = ParseExpression();

                if (param == null)
                    break;

                yield return param;

                if (!Take(LexemeKind.Comma, out _))
                    break;
            }

            if (!Take(LexemeKind.RightParenthesis, out _))
            {
                Error("Missing closing parentheses for method call");
            }
        }

        public Expression ParseExpression(bool @throw = true)
        {
            if (Take(LexemeKind.StringLiteral, out var str))
                return new ConstantExpression(str.Content, ConstantKind.String);
            else if (Take(LexemeKind.IntegerLiteral, out var @int))
                return new ConstantExpression(@int.Content, ConstantKind.Integer);
            else if (Take(LexemeKind.DecimalLiteral, out var dec))
                return new ConstantExpression(dec.Content, ConstantKind.Decimal);
            else if (TakeKeyword("true", false) != null)
                return new ConstantExpression("true", ConstantKind.Boolean);
            else if (TakeKeyword("false", false) != null)
                return new ConstantExpression("false", ConstantKind.Boolean);

            var references = new List<Expression>();

            //Take identifiers and dots
            while (Take(LexemeKind.Identifier, out var identifier))
            {
                references.Add(new IdentifierExpression(identifier.Content));

                if (!Take(LexemeKind.Dot, out _)) //Found a non-dot, break
                    break;

                if (NextNonWhitespace.Kind != LexemeKind.Identifier) //Found a dot withot an identifier next to it, error
                    Error("Expected identifier after dot");
            }

            if (references.Count > 0)
            {
                var reference = new ReferenceExpression(references);

                if (Take(LexemeKind.LeftParenthesis, out _))
                    return new MethodCallExpression(TakeParameters(), reference);

                return reference;
            }

            if (@throw)
                Error($"Expected expression, got {NextNonWhitespace}");

            return null;
        }

        public Declaration ParseDeclaration()
        {
            var let = TakeKeyword("let");
            var name = Take(LexemeKind.Identifier);
            
            if (Take(LexemeKind.EqualsAssign, out var eq))
            {
                var value = ParseExpression();

                return new VariableDeclaration(name.Content, value);
            }

            return new VariableDeclaration(name.Content, null);
        }
    }
}
