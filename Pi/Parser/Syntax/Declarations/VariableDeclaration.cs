namespace Pi.Parser.Syntax.Declarations
{
    public sealed class VariableDeclaration : Declaration
    {
        public Expression Value { get; }

        public VariableDeclaration(SourceLocation location, string name, Expression value) : base(location, name)
        {
            this.Value = value;
        }
    }
}
