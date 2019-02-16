using System.Collections.Generic;

namespace Pi.Parser.Syntax.Statements
{
    public sealed class BlockStatement : Statement
    {
        public IEnumerable<Node> Contents { get; }

        public BlockStatement(SourceLocation location, IEnumerable<Node> contents) : base(location)
        {
            this.Contents = contents;
        }
    }
}
