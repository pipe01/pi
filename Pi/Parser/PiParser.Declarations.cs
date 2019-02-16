using Pi.Lexer;
using Pi.Parser.Syntax.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Parser
{
    internal sealed partial class PiParser
    {
        private IEnumerable<ParameterDeclaration> TakeParameterDeclarations()
        {
            var ret = new List<ParameterDeclaration>();

            do
            {
                if (!Take(LexemeKind.Identifier, out var param))
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

        private string TakeVisibility()
        {
            string visibility = null;

            int prevIndex = Index;
            BackUntilNotWhitespace();

            if (Take(LexemeKind.Keyword, out var k) && k.IsVisibilityModifier())
            {
                visibility = k.Content;
            }

            Index = prevIndex;

            return visibility;
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            TakeKeyword("let");
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

        private FunctionDeclaration ParseFunctionDeclaration(string visibility = null)
        {
            visibility = visibility ?? TakeVisibility();
            TakeKeyword("function");
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

            return new FunctionDeclaration(Location, name.Content, @params, body, type, visibility);
        }

        private ClassDeclaration ParseClassDeclaration()
        {
            var visibility = TakeVisibility();
            TakeKeyword("class");
            var name = Take(LexemeKind.Identifier);

            Take(LexemeKind.LeftBrace);

            var (fields, funcs) = ParseClassMemberDeclarations();

            Take(LexemeKind.RightBrace);

            return new ClassDeclaration(Location, name.Content, fields, funcs, visibility);
        }

        private (List<FieldDeclaration>, List<FunctionDeclaration>) ParseClassMemberDeclarations()
        {
            var fields = new List<FieldDeclaration>();
            var funcs = new List<FunctionDeclaration>();

            while (NextNonWhitespace.Kind != LexemeKind.RightBrace)
            {
                if (NextNonWhitespace.IsVisibilityModifier())
                {
                    Advance();

                    string visibility = NextNonWhitespace.Content;

                    Advance();

                    if (TakeKeyword("function", @throw: false) != null)
                    {
                        BackUntilNotWhitespace();

                        funcs.Add(ParseFunctionDeclaration(visibility));
                    }
                    else if (Take(LexemeKind.Identifier, out var identifier))
                    {
                        BackUntilNotWhitespace();
                        fields.Add(ParseFieldDeclaration(visibility));
                    }
                }

                Advance();
            }

            return (fields, funcs);
        }

        private FieldDeclaration ParseFieldDeclaration(string visibility)
        {
            var name = Take(LexemeKind.Identifier);
            FieldDeclaration ret;
            string type = null;

            if ((!Take(LexemeKind.Colon, out _) || !TakeType(out type)) && NextNonWhitespace.Kind != LexemeKind.EqualsAssign)
                Error("Missing field type");

            if (Take(LexemeKind.EqualsAssign, out _))
            {
                var value = ParseExpression();

                ret = new FieldDeclaration(Location, name.Content, type, value, visibility);
            }
            else
            {
                if (Current.Kind != LexemeKind.Semicolon)
                    Error("Invalid field name");

                ret = new FieldDeclaration(Location, name.Content, type, null, visibility);
            }

            if (!Take(LexemeKind.Semicolon, out _))
                MissingSemicolon();

            return ret;
        }
    }
}