using Pi.Utils;
using System.Collections.Generic;

namespace Pi.Interpreter
{
    internal class Context : IKeyed<object>
    {
        private IDictionary<string, object> Locals = new Dictionary<string, object>();
        
        public object this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }

        public bool Get(string key, out object value)
        {
            return Locals.TryGetValue(key, out value);
        }

        public object Get(string key)
        {
            return Locals[key];
        }

        public void Set(string key, object value)
        {
            Locals[key] = value;
        }
    }
}
