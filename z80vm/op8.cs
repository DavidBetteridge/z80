using System;

namespace z80vm
{
    public class op8
    {
        private Reg8? register;
        private byte? immediate;
        private MemoryAddress memoryAddress;
        private Value value;

        public static op8 Read8BitValue(Reg8 reg)
        {
            return new op8() { register = reg };
        }

        public static op8 Read8BitValue(byte immediate)
        {
            return new op8() { immediate = immediate };
        }

        public static op8 Read8BitValue(MemoryAddress memoryAddress)
        {
            return new op8() { memoryAddress = memoryAddress };
        }

        public static op8 Read8BitValue(Value value)
        {
            return new op8() { value = value };
        }

        public Reg8? Register => this.register;

        public byte Resolve(Memory memory, Registers registers)
        {
            if (immediate.HasValue)
                return immediate.Value;
            else if (memoryAddress != null)
                return memory.ReadByte(memoryAddress.MemoryLocation);
            else if (value != null)
                return memory.ReadByte((ushort)(registers.Read(value.Register) + value.Offset));
            else
                return registers.Read(this.register.Value);
        }


    }
}
