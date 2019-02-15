using System.Collections.Generic;

namespace Pi.Parser.Syntax.Statements
{
    public sealed class BlockStatement : Statement
    {
        public IEnumerable<Node> Contents { get; }

        public BlockStatement(IEnumerable<Node> contents)
        {
            this.Contents = contents;
        }
    }
}
