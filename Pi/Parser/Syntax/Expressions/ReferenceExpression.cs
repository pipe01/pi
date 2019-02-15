using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class ReferenceExpression : Expression
    {
        public IEnumerable<Expression> References { get; }

        public ReferenceExpression(IEnumerable<Expression> references)
        {
            this.References = references;
        }
    }
}
