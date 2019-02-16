using System.Diagnostics;

namespace Pi.Lexer
{
    internal enum LexemeKind
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
        Colon,
        Comma,
        Dot,
        FatArrow,

        EqualsAssign,
        EqualsCompare,
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

        QuestionMark
    }

    [DebuggerDisplay("{Kind}: {Content}")]
    internal sealed class Lexeme
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

        public override string ToString() => $"\"{Content}\" ({Kind})";
    }
}
