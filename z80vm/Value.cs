namespace z80vm
{
    public class MemoryAddress
    {
        public ushort MemoryLocation { get; private set; }
        public MemoryAddress(ushort memoryLocation)
        {
            MemoryLocation = memoryLocation;
        }
    }

    public class Value
    {
        public Reg16 Register { get; private set; }
        public sbyte Offset { get; private set; }

        public Value Add(sbyte offset)
        {
            this.Offset = offset;
            return this;
        }

        public static Value valueAt(Reg16 register)
        {
            return new Value() { Register = register, Offset = 0 };
        }

        public static Value valueAt(AddValue addValue)
        {
            return new Value() { Register = addValue.Register, Offset = addValue.n };
        }

        public static MemoryAddress valueAt(ushort memoryLocation)
        {
            return new MemoryAddress(memoryLocation);
        }
    }
}
