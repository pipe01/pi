using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class ParameterDeclaration : Declaration, ITyped
    {
        public string Type { get; }

        public ParameterDeclaration(SourceLocation location, string name, string type) : base(location, name)
        {
            this.Type = type;
        }
    }
}
