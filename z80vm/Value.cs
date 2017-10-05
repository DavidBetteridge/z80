﻿namespace z80vm
{
    public class MemoryAddress
    {
        public ushort MemoryLocation { get; private set; }
        public MemoryAddress(ushort memoryLocation)
        {
            MemoryLocation = memoryLocation;
        }

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

        public override string ToString()
        {
            if (Offset == 0)
                return $"({Register.ToString().ToLower()})";
            else
                return $"({Register.ToString().ToLower()}+n)";
        }
    }
}
