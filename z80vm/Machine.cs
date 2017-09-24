using System;

namespace z80vm
{
    public class Machine
    {
        public Registers Registers { get; private set; }
        public Memory Memory { get; private set; }

        public Machine()
        {
            this.Registers = new Registers();
            this.Memory = new Memory();
        }

        public void PUSH(Reg16 register)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            var value = this.Registers.Read(register);
            var (highOrderByte, lowOrderByte) = value.Split();

            var sp = this.Registers.Read(Reg16.SP);
            this.Memory.Set(sp, highOrderByte);
            this.Memory.Set((ushort)(sp - 1), lowOrderByte);
            this.Registers.Set(Reg16.SP, (ushort)(sp - 2));
        }

        public void LD(Reg8 register, byte value)
        {
            this.Registers.Set(register, value);
        }
    }
}
