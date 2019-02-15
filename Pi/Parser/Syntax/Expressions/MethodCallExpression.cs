using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class MethodCallExpression : Expression
    {
        public IEnumerable<Expression> Arguments { get; }
        public Expression Reference { get; }

        public MethodCallExpression(IEnumerable<Expression> arguments, Expression reference)
        {
            this.Arguments = arguments;
            this.Reference = reference;
        }
    }
}
