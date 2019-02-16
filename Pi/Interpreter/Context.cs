using Pi.Utils;
using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class Context
    {
        public IDictionary<string, object> Locals { get; } = new Dictionary<string, object>();
        public IDictionary<string, ClassModel> ClassModels { get; } = new Dictionary<string, ClassModel>();
    }
}
