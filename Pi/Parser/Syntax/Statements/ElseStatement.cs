using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Statements
{
    public sealed class ElseStatement : BodyStatement
    {
        private ElseStatement(SourceLocation location, BlockStatement body) : base(location, body)
        {
        }
    }
}
