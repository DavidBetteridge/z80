using System;

namespace z80vm
{
    public class Machine
    {
        public Registers Registers { get; private set; }

        public Machine()
        {
            this.Registers = new Registers();
        }

        public void PUSH(Reg16 register)
        {
            this.Registers.SP -= 2;
        }

        public void LD(Reg8 register, byte value)
        {
            this.Registers.Set(register, value);
        }
    }
}
