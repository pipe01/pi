using Pi.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Interpreter
{
    internal class PiObject : IKeyed<object>
    {
        private readonly IDictionary<string, object> Fields = new Dictionary<string, object>();

        public object this[string key]
        {
            get => Get(key);
            set => Set(key, value);
        }
        
        public string[] GetFieldNames() => Fields.Keys.ToArray();

        public bool Get(string key, out object value) => Fields.TryGetValue(key, out value);

        public object Get(string key) => Fields[key];

        public void Set(string key, object value) => Fields[key] = value;
    }
}
