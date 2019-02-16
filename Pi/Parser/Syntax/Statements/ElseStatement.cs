using System;
using System.Collections.Generic;
using System.Text;

namespace Pi.Parser.Syntax.Statements
{
    public class ElseStatement : BodyStatement
    {
        public ElseStatement(SourceLocation location, BlockStatement body) : base(location, body)
        {
        }
    }
}
