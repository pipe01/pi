using Pi.Parser.Syntax.Declarations;
using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class ClassModel : PiType
    {
        public override string Name { get; }
        public ModelField[] Fields { get; }

        public ClassModel(string name, ModelField[] fields)
        {
            this.Name = name;
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

                PiType fieldType = interpreter.Context.GetPiType(field.Type);

                if (fieldType == null)
                    throw new InterpreterException($"Type \"{field.Type}\" not found", field.Location);

                fields.Add(new ModelField(field.Name, value, fieldType));
            }

            return new ClassModel(declaration.Name, fields.ToArray());
        }
    }

    internal class ModelField
    {
        public PiType Type { get; }   
        public string Name { get; }
        public object DefaultValue { get; }

        public ModelField(string name, object defaultValue, PiType type)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;
            this.Type = type;
        }
    }
}
