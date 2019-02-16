using System.Collections.Generic;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FunctionDeclaration : Declaration, ITyped, IVisible
    {
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public IEnumerable<Node> Body { get; }
        public string Type { get; }
        public string Visibility { get; }

        public FunctionDeclaration(SourceLocation location, string name, IEnumerable<ParameterDeclaration> parameters,
            IEnumerable<Node> body, string type, string visibility) : base(location, name)
        {
            this.Parameters = parameters;
            this.Body = body;
            this.Type = type;
            this.Visibility = visibility;
        }
    }
}
