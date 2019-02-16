namespace Pi.Parser.Syntax.Statements
{
    public sealed class IfStatement : BodyStatement
    {
        public Expression Condition { get; }

        public IfStatement(SourceLocation location, Expression condition, BlockStatement body) : base(location, body)
        {
            this.Condition = condition;
        }
    }
}
