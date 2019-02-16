using System.Collections.Generic;
using System.Linq;

namespace Pi.Parser.Syntax.Statements
{
    public sealed class IfStatement : BodyStatement
    {
        public Expression Condition { get; }
        public ElseStatement[] Elses { get; }

        public IfStatement(SourceLocation location, Expression condition, BlockStatement body,
            IEnumerable<ElseStatement> elses) : base(location, body)
        {
            this.Condition = condition;
            this.Elses = elses.ToArray();
        }
    }
}
