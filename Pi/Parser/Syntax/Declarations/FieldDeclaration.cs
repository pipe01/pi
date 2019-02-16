namespace Pi.Parser.Syntax.Declarations
{
    public sealed class FieldDeclaration : Declaration, ITyped, IVisible
    {
        public string Type { get; }
        public string Visibility { get; }
        public Expression DefaultValue { get; }

        public FieldDeclaration(SourceLocation location, string name, string type, Expression defaultValue,
            string visibility) : base(location, name)
        {
            this.Type = type;
            this.Visibility = visibility;
            this.DefaultValue = defaultValue;
        }
    }
}
