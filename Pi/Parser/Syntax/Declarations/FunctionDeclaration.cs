using System.Collections.Generic;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FunctionDeclaration : Declaration
    {
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public IEnumerable<Node> Body { get; }
        public string ReturnType { get; }

        public FunctionDeclaration(SourceLocation location, string name, IEnumerable<ParameterDeclaration> parameters,
            IEnumerable<Node> body, string returnType) : base(location, name)
        {
            this.Parameters = parameters;
            this.Body = body;
            this.ReturnType = returnType;
        }
    }
}
