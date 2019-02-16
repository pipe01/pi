namespace Pi.Parser.Syntax.Statements
{
    public sealed class ElseIfStatement : BodyStatement
    {
        public Expression Condition { get; }

        public ElseIfStatement(SourceLocation location, Expression condition, BlockStatement body) : base(location, body)
        {
            this.Condition = condition;
        }
    }
}
