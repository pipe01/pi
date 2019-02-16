using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class Context
    {
        private IDictionary<string, object> Locals = new Dictionary<string, object>();

        public bool GetLocal(string name, out object value)
        {
            return Locals.TryGetValue(name, out value);
        }

        public object GetLocal(string name)
        {
            return Locals[name];
        }

        public void SetLocal(string name, object value)
        {
            Locals[name] = value;
        }
    }
}
