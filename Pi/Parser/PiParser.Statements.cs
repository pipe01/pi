using Pi.Lexer;
using Pi.Parser.Syntax;
using Pi.Parser.Syntax.Statements;
using System.Collections.Generic;

namespace Pi.Parser
{
    internal sealed partial class PiParser
    {
        private Statement ParseIfStatement(bool elseIf = false)
        {
            var @if = TakeKeyword("if");
            Take(LexemeKind.LeftParenthesis);

            var cond = ParseExpression();

            Take(LexemeKind.RightParenthesis);

            var body = ParseBlock();

            var elses = new List<ElseStatement>();

            if (!elseIf)
            {
                Lexeme @else = null;
                bool addedElse = false;

                while ((@else = TakeKeyword("else", @throw: false)) != null)
                {
                    var elseStatement = (ElseStatement)ParseElseStatement();

                    if (elseStatement is ElseIfStatement && addedElse)
                        Error("Else-if must come before else", @else.Begin);

                    if (elseStatement is ElseStatement && addedElse)
                        Error("Only one else is allowed per if statement", @else.Begin);

                    elses.Add(elseStatement);

                    if (elseStatement is ElseStatement && !(elseStatement is ElseIfStatement))
                        addedElse = true;

                    Advance();
                }
            }

            if (elseIf)
                return new ElseIfStatement(Location, cond, body);

            return new IfStatement(Location, cond, body, elses);
        }

        private Statement ParseElseStatement()
        {
            Advance();
            var @if = TakeKeyword("if", @throw: false);

            if (@if != null)
            {
                Back();
                return ParseIfStatement(true);
            }

            return new ElseStatement(Location, ParseBlock());
        }
    }
}