namespace Pi.Parser.Syntax
{
    public abstract class Declaration : Node
    {
        public string Name { get; }

        protected Declaration(SourceLocation location, string name) : base(location)
        {
            this.Name = name;
        }
    }

    public interface ITyped
    {
        string Type { get; }
    }

    public interface IVisible
    {
        string Visibility { get; }
    }
}
