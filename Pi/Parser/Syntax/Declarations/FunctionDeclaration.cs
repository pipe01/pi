using System.Collections.Generic;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FunctionDeclaration : Declaration
    {
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public Statement Body { get; }

        public FunctionDeclaration(string name, IEnumerable<ParameterDeclaration> parameters, Statement body) : base(name)
        {
            this.Parameters = parameters;
            this.Body = body;
        }
    }
}
