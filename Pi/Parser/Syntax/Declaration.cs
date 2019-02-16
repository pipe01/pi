using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax
{
    public abstract class Declaration : Node
    {
        public string Name { get; }

        protected Declaration(SourceLocation location, string name) : base(location)
        {
            this.Name = name;
        }
    }
}
