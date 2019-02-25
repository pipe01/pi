namespace Pi.Parser.Syntax.Expressions
{
    internal sealed class UnaryExpression : Expression
    {
        public UnaryOperators Operator { get; }
        public Expression Argument { get; }

        public UnaryExpression(SourceLocation location, UnaryOperators @operator, Expression argument) : base(location)
        {
            this.Operator = @operator;
            this.Argument = argument;
        }
    }

    public enum UnaryOperators
    {
        LogicalNegation
    }
}
