using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class ParameterDeclaration : Declaration
    {
        public ParameterDeclaration(SourceLocation location, string name) : base(location, name)
        {
        }
    }
}
