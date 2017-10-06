using System;

namespace z80vm
{
    public class op8 : Iop8
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
        public Value Value => this.value;

        public byte Read(Memory memory, Registers registers)
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

        public override string ToString()
        {
            if (immediate.HasValue)
                return "n";
            else if (memoryAddress != null)
                return memoryAddress.ToString();
            else if (value != null)
                return value.ToString();
            else
                return register.Value.ToString().ToLower();
        }

        public void Set(Memory memory, Registers registers, byte newValue)
        {
            if (immediate.HasValue)
                throw new InvalidOperationException("Immediate values cannot be updated");
            else if (memoryAddress != null)
                memory.Set(memoryAddress.MemoryLocation, newValue);
            else if (this.value != null)
                memory.Set((ushort)(registers.Read(value.Register) + value.Offset), newValue);
            else
                registers.Set(this.register.Value, newValue);
        }
    }
}
