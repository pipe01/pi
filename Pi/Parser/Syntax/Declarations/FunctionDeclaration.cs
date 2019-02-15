using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FunctionDeclaration : Declaration
    {
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public Statement Body { get; }

        public FunctionDeclaration(IEnumerable<ParameterDeclaration> parameters, Statement body, string name) : base(name)
        {
            this.Parameters = parameters;
            this.Body = body;
        }
    }
}
