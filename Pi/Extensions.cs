using System;
using System.Collections.Generic;
using System.Text;

namespace Pi
{
    internal static class Extensions
    {
        public static bool IsValidWordChar(this char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_';
        }
    }
}
