using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class ClassDeclaration : Declaration
    {
        public ClassDeclaration(SourceLocation location, string name) : base(location, name)
        {
        }
    }
}
