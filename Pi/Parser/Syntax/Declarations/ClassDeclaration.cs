using System.Collections.Generic;
using System.Linq;

namespace Pi.Parser.Syntax.Declarations
{
    public sealed class ClassDeclaration : Declaration, IVisible
    {
        public FieldDeclaration[] Fields { get; }
        public FunctionDeclaration[] Functions { get; }

        public string Visibility { get; }

        public ClassDeclaration(SourceLocation location, string name, IEnumerable<FieldDeclaration> fields,
            IEnumerable<FunctionDeclaration> functions, string visibility) : base(location, name)
        {
            this.Fields = fields.ToArray();
            this.Functions = functions.ToArray();
            this.Visibility = visibility;
        }
    }
}
