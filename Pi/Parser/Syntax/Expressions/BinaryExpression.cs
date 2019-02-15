using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public BinaryOperators Operator { get; }
        public Expression Right { get; }

        public BinaryExpression(Expression left, Expression right, BinaryOperators @operator)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
    }

    public enum BinaryOperators
    {
        Assign,
        Add,
        Subtract,
        Multiply,
        Divide
    }
}
