﻿using System.Collections.Generic;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FunctionDeclaration : Declaration
    {
        public IEnumerable<ParameterDeclaration> Parameters { get; }
        public IEnumerable<Node> Body { get; }

        public FunctionDeclaration(SourceLocation location, string name, IEnumerable<ParameterDeclaration> parameters,
            IEnumerable<Node> body) : base(location, name)
        {
            this.Parameters = parameters;
            this.Body = body;
        }
    }
}
