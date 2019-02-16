namespace Pi.Parser.Syntax.Statements
{
    public sealed class ElseIfStatement : ElseStatement
    {
        public Expression Condition { get; }

        public ElseIfStatement(SourceLocation location, Expression condition, BlockStatement body) : base(location, body)
        {
            this.Condition = condition;
        }
    }
}
