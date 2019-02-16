using Pi.Parser.Syntax.Declarations;
using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class ClassModel
    {
        public ModelField[] Fields { get; }

        public ClassModel(ModelField[] fields)
        {
            this.Fields = fields;
        }

        public static ClassModel FromDeclaration(PiInterpreter interpreter, ClassDeclaration declaration)
        {
            var fields = new List<ModelField>();

            foreach (var field in declaration.Fields)
            {
                object value = null;

                if (field.DefaultValue != null)
                    value = interpreter.EvaluateExpression(field.DefaultValue);

                fields.Add(new ModelField(field.Name, value));
            }

            return new ClassModel(fields.ToArray());
        }
    }

    internal class ModelField
    {
        public string Name { get; }
        public object DefaultValue { get; }

        public ModelField(string name, object defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
        }
    }
}
