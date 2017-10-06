namespace z80vm
{
    public class MemoryAddress
    {
        public ushort MemoryLocation { get; private set; }
        public MemoryAddress(ushort memoryLocation)
        {
            MemoryLocation = memoryLocation;
        }

        /// <summary>
        /// Returns the text version of this op code used for command validation.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => "(nn)";
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

        public static MemoryAddress valueAt(ushort memoryLocation)
        {
            return new MemoryAddress(memoryLocation);
        }

        /// <summary>
        /// Returns the text version of this op code used for command validation.  For example (SP+n) .
        /// Note it returns 'n' not the actual number
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Offset == 0)
                return $"({Register.ToString().ToLower()})";
            else
                return $"({Register.ToString().ToLower()}+n)";
        }
    }
}
