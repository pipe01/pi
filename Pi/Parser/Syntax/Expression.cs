namespace Pi.Parser.Syntax
{
    public abstract class Expression : Node
    {
        protected Expression(SourceLocation location) : base(location)
        {
        }
    }
}
