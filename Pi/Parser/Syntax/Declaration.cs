using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax
{
    public abstract class Declaration : Node
    {
        public string Name { get; }

        protected Declaration(string name)
        {
            this.Name = name;
        }
    }
}
