namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FieldDeclaration : Declaration, ITyped, IVisible
    {
        public string Type { get; }
        public string Visibility { get; }

        public FieldDeclaration(SourceLocation location, string name, string type, string visibility) : base(location, name)
        {
            this.Type = type;
            this.Visibility = visibility;
        }
    }
}
