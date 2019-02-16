namespace Pi.Parser.Syntax.Declarations
{
    public sealed class VariableDeclaration : Declaration
    {
        public Expression Value { get; }
        public string Type { get; }

        public VariableDeclaration(SourceLocation location, string name, Expression value, string type) : base(location, name)
        {
            this.Value = value;
            this.Type = type;
        }
    }
}
