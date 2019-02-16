namespace Pi.Interpreter
{
    internal class PiInt : PiType
    {
        public override string Name => "int";
    }

    internal class PiDecimal : PiType
    {
        public override string Name => "dec";
    }

    internal class PiString : PiType
    {
        public override string Name => "string";
    }

    internal class PiBoolean : PiType
    {
        public override string Name => "bool";
    }
}
