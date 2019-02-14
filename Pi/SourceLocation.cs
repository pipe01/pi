namespace Pi
{
    public struct SourceLocation
    {
        public int Line { get; }
        public int Column { get; }
        public int Index { get; }

        public SourceLocation(int line, int column, int index)
        {
            this.Line = line;
            this.Column = column;
            this.Index = index;
        }
    }
}
