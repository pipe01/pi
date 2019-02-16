using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class Context
    {
        public IDictionary<string, object> Locals { get; } = new Dictionary<string, object>();
        public IDictionary<string, ClassModel> ClassModels { get; } = new Dictionary<string, ClassModel>();

        public PiType GetPiType(string name)
        {
            switch (name)
            {
                case "int":
                    return new PiInt();
                case "dec":
                    return new PiDecimal();
                case "string":
                    return new PiString();
                case "bool":
                    return new PiBoolean();
            }

            if (!ClassModels.TryGetValue(name, out var type))
                return null;

            return type;
        }
    }
}
