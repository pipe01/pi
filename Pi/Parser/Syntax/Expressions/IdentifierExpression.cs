using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public sealed class IdentifierExpression : Expression
    {
        public string Name { get; }

        public IdentifierExpression(SourceLocation location, string name) : base(location)
        {
            this.Name = name;
        }
    }
}
