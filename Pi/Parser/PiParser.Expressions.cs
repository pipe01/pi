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

            if (Take(LexemeKind.ExclamationMark, out _))
            {
                return new UnaryExpression(Location, UnaryOperators.LogicalNegation, ParseExpression());
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
            BinaryOperators? binOp = null;

            switch (NextNonWhitespace.Kind)
            {
                case LexemeKind.Plus:
                    binOp = BinaryOperators.Add;
                    break;
                case LexemeKind.Minus:
                    binOp = BinaryOperators.Subtract;
                    break;
                case LexemeKind.Mult:
                    binOp = BinaryOperators.Multiply;
                    break;
                case LexemeKind.Div:
                    binOp = BinaryOperators.Divide;
                    break;
                case LexemeKind.EqualsAssign:
                    binOp = BinaryOperators.Assign;
                    break;
            }

            if (binOp != null)
            {
                Advance();
                AdvanceUntilNotWhitespace();

                var left = ret;
                var right = ParseExpression();

                return new BinaryExpression(Location, left, right, binOp.Value);
            }

            if (Take(LexemeKind.Dot, out _))
                ret = new ReferenceExpression(Location, new Expression[] { ret, ParseExpression() });

            if (ret is ReferenceExpression reference && Take(LexemeKind.LeftParenthesis, out _))
                ret = new MethodCallExpression(Location, TakeParameters(), reference);

            return ret;
        }
    }
}
