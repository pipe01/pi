namespace Pi.Parser.Syntax.Declarations
{
    public sealed class VariableDeclaration : Declaration
    {
        public Expression Value { get; }

        public VariableDeclaration(string name, Expression value) : base(name)
        {
            this.Value = value;
        }
    }
}
