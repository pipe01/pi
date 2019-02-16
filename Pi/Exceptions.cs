using System;

namespace Pi
{
    public class LocationException : Exception
    {
        private string _Message;
        public override string Message => _Message;

        public SourceLocation Location { get; }

        public LocationException(string message, SourceLocation location)
        {
            _Message = message + $" at line {location.Line}, column {location.Column}";
            this.Location = location;
        }
    }
    
    public class SyntaxException : LocationException
    {
        public SyntaxException(string message, SourceLocation location) : base(message, location)
        {
        }
    }
    
    public class InterpreterException : LocationException
    {
        public InterpreterException(string message, SourceLocation location) : base(message, location)
        {
        }
    }
}
