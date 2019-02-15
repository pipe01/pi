﻿using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Declarations;
using Pi.Parser.Syntax.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Pi
{
    public class PiParser
    {
        private Lexeme[] Lexemes;

        private int Index;
        private Lexeme Current => Lexemes[Index];

        private Lexeme NextNonWhitespace => Lexemes.Skip(Index).SkipWhile(o => o.Kind == LexemeKind.Whitespace).First();
        
        public IEnumerable<Node> Parse(IEnumerable<Lexeme> lexemes)
        {
            this.Index = 0;
            this.Lexemes = lexemes.ToArray();

            while (Current.Kind != LexemeKind.EndOfFile)
            {
                yield return ParseVariableDeclaration();
            }
        }

        private void Advance()
        {
            Index++;
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
                var param = ParseExpression();

                if (param == null)
                    break;

                ret.Add(param);
            }
            while (Take(LexemeKind.Comma, out _));

            if (!Take(LexemeKind.RightParenthesis, out _))
                Error("Missing closing parentheses for method call");

            return ret;
        }

        public Expression ParseExpression(bool @throw = true)
        {
            if (Take(LexemeKind.StringLiteral, out var str))
                return new ConstantExpression(str.Content, ConstantKind.String);
            else if (Take(LexemeKind.IntegerLiteral, out var @int))
                return new ConstantExpression(@int.Content, ConstantKind.Integer);
            else if (Take(LexemeKind.DecimalLiteral, out var dec))
                return new ConstantExpression(dec.Content, ConstantKind.Decimal);
            else if (TakeKeyword("true", @throw: false) != null)
                return new ConstantExpression("true", ConstantKind.Boolean);
            else if (TakeKeyword("false", @throw: false) != null)
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

        public VariableDeclaration ParseVariableDeclaration()
        {
            var let = TakeKeyword("let");
            var name = Take(LexemeKind.Identifier);
            VariableDeclaration ret;

            if (Take(LexemeKind.EqualsAssign, out _))
            {
                var value = ParseExpression();

                ret = new VariableDeclaration(name.Content, value);
            }
            else
            {
                if (Current.Kind != LexemeKind.Semicolon)
                    Error("Invalid variable name");

                ret = new VariableDeclaration(name.Content, null);
            }

            if (!Take(LexemeKind.Semicolon, out _))
                Error("Missing semicolon");

            return ret;
        }
    }
}