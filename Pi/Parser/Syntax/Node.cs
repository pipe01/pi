namespace Pi.Parser.Syntax
{
    public abstract class Node
    {
        public SourceLocation Location { get; }

        public Node(SourceLocation location)
        {
            this.Location = location;
        }
    }
}
