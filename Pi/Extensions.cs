namespace Pi
{
    internal static class Extensions
    {
        public static bool IsValidWordChar(this char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_';
        }

        public static bool IsNumber(this object obj)
        {
            return obj is int || obj is float;
        }
    }
}
