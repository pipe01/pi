using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Expressions
{
    public enum ConstantKind
    {
        String,
        Integer,
        Decimal,
        Boolean
    }

    public sealed class ConstantExpression : Expression
    {
        public string Value { get; }
        public ConstantKind Kind { get; }

        public ConstantExpression(SourceLocation location, string value, ConstantKind kind) : base(location)
        {
            this.Value = value;
            this.Kind = kind;
        }
    }
}
