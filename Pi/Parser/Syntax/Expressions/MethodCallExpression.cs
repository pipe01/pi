using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class MethodCallExpression : Expression
    {
        public IEnumerable<Expression> Arguments { get; }
        public Expression Reference { get; }

        public MethodCallExpression(SourceLocation location, IEnumerable<Expression> arguments, Expression reference) : base(location)
        {
            this.Arguments = arguments;
            this.Reference = reference;
        }
    }
}
