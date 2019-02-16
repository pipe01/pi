using Pi.Parser.Syntax.Statements;

namespace Pi.Parser.Syntax
{
    public abstract class Statement : Node
    {
        protected Statement(SourceLocation location) : base(location)
        {
        }
    }

    public abstract class BodyStatement : Statement
    {
        public BlockStatement Body { get; }

        protected BodyStatement(SourceLocation location, BlockStatement body) : base(location)
        {
            this.Body = body;
        }
    }
}
