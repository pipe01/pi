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
            string visibility = null;

            int prevIndex = Index;
            BackUntilNotWhitespace();

            if (Take(LexemeKind.Keyword, out var k) && (k.Content == "public" || k.Content == "private"))
            {
                visibility = k.Content;
            }

            Index = prevIndex;


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

            return new FunctionDeclaration(Location, name.Content, @params, body, type, visibility);
        }

        private ClassDeclaration ParseClassDeclaration()
        {
            throw new NotImplementedException();
        }
    }
}