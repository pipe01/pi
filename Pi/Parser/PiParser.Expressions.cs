using Pi.Lexer;
using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Expressions;
using System.Collections.Generic;

namespace Pi.Parser
{
    internal sealed partial class PiParser
    {
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
    }
}
