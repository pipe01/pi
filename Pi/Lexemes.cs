using System.Diagnostics;

namespace Pi
{
    public enum LexemeKind
    {
        NewLine,
        Whitespace,
        EndOfFile,
        Identifier,
        Keyword,
        StringLiteral,
        IntegerLiteral,
        DecimalLiteral,

        Semicolon,
        Comma,
        Dot,

        Equals,
        NotEqual,

        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,

        Plus,
        Minus,
        Div,
        Mult,

        LeftParenthesis,
        RightParenthesis,

        LeftBracket,
        RightBracket,

        LeftBrace,
        RightBrace,

        QuestionMark,
        Colon
    }

    [DebuggerDisplay("{Kind}: {Content}")]
    public sealed class Lexeme
    {
        public LexemeKind Kind { get; }
        public SourceLocation Begin { get; }
        public SourceLocation End { get; }
        public string Content { get; }

        public Lexeme(LexemeKind kind, SourceLocation begin, SourceLocation end, string content)
        {
            this.Kind = kind;
            this.Begin = begin;
            this.End = end;
            this.Content = content;
        }
    }
}
