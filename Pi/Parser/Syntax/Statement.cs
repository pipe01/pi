namespace Pi.Parser.Syntax
{
    public abstract class Statement : Node
    {
        protected Statement(SourceLocation location) : base(location)
        {
        }
    }
}
