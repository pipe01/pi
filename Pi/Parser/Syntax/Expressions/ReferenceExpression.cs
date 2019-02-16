using System.Collections.Generic;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class ReferenceExpression : Expression
    {
        public IEnumerable<Expression> References { get; }

        public ReferenceExpression(SourceLocation location, IEnumerable<Expression> references) : base(location)
        {
            this.References = references;
        }
    }
}
