﻿using System;

namespace Pi
{
    [Serializable]
    public class SyntaxException : Exception
    {
        private string _Message;
        public override string Message => _Message;

        public SyntaxException(string message, SourceLocation location)
        {
            _Message = message + $" at line {location.Line}, column {location.Column}";
        }
    }
}