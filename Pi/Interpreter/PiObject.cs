using Pi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pi.Interpreter
{
    internal class PiObject
    {
        public IDictionary<string, object> Fields { get; } = new Dictionary<string, object>();

        public ClassModel Model { get; }

        public PiObject(ClassModel model)
        {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
