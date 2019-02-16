using System;

namespace Pi
{
    [Serializable]
    public class SyntaxException : Exception
    {
        private string _Message;
        public override string Message => _Message;

        public SourceLocation Location { get; }

        public SyntaxException(string message, SourceLocation location)
        {
            _Message = message + $" at line {location.Line}, column {location.Column}";
            this.Location = location;
        }
    }
    
    [Serializable]
    public class InterpreterException : Exception
    {
        public InterpreterException() { }
        public InterpreterException(string message) : base(message) { }
        public InterpreterException(string message, Exception inner) : base(message, inner) { }
        protected InterpreterException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
