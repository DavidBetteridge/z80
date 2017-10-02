namespace z80vm
{
    public class Value
    {
        public Reg16 Register { get; private set; }
        public sbyte Offset { get; set; }

        public static Value valueAt(Reg16 register)
        {
            return new Value() { Register = register, Offset = 0 };
        }

        public static Value valueAt(AddValue addValue)
        {
            return new Value() { Register = addValue.Register, Offset = addValue.n };
        }
    }
}
