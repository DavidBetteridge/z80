using System;

namespace z80vm
{
    public class Machine
    {
        public Registers Registers { get; private set; }
        public Memory Memory { get; private set; }
        public Flags Flags { get; private set; }

        public Machine()
        {
            this.Registers = new Registers();
            this.Memory = new Memory();
            this.Flags = new Flags(this.Registers);
        }

        /// <summary>
        /// There are no operands. This instruction exchanges BC with BC’, DE with DE’ and HL with HL’ at the same time. 
        /// </summary>
        public void EXX()
        {
            void swap(Reg16 register, Reg16Shadow shadow)
            {
                var temp = this.Registers.Read(register);
                this.Registers.Set(register, this.Registers.Read(shadow));
                this.Registers.Set(shadow, temp);
            }

            swap(Reg16.BC, Reg16Shadow.BC);
            swap(Reg16.DE, Reg16Shadow.DE);
            swap(Reg16.HL, Reg16Shadow.HL);
        }

        /// <summary>
        /// Usage: Adds 2 numbers together and stores the result in the first operand
        /// Flags: Preserves the S, Z and P/V flags, and H is undefined. Rest of flags modified by definition.
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        public void ADD(Reg16 reg1, Reg16 reg2)
        {
            switch (reg1)
            {
                case Reg16.HL:
                case Reg16.IX:
                case Reg16.IY:
                    break;

                default:
                    throw new InvalidOperationException();
            }

            switch (reg2)
            {
                case Reg16.AF:
                case Reg16.PC:
                    throw new InvalidOperationException();
            }

            if ((reg1 == Reg16.HL && reg2 == Reg16.IX) ||
                (reg1 == Reg16.HL && reg2 == Reg16.IY) ||
                (reg1 == Reg16.IX && reg2 == Reg16.HL) ||
                (reg1 == Reg16.IX && reg2 == Reg16.IY) ||
                (reg1 == Reg16.IY && reg2 == Reg16.HL) ||
                (reg1 == Reg16.IY && reg2 == Reg16.IX))
            {
                throw new InvalidOperationException();
            }

            var value1 = this.Registers.Read(reg1);
            var value2 = this.Registers.Read(reg2);
            var total = (value1 + value2);

            if (total > ushort.MaxValue)
            {
                this.Flags.Set(Flag.C);
                total = total - 65536;
            }
            else
                this.Flags.Clear(Flag.C);

            this.Registers.Set(reg1, (ushort)total);
        }

        /// <summary>
        /// SP is decreased by two and the value of reg16 is copied to the memory location pointed by the new value of SP. 
        /// It does not affect the flags.
        /// </summary>
        /// <param name="register"></param>
        public void PUSH(Reg16 register)
        {
            var value = this.Registers.Read(register);
            var (highOrderByte, lowOrderByte) = value.Split();

            var stackPointer = this.Registers.Read(Reg16.SP);
            stackPointer = (ushort)(stackPointer - 2);
            this.Registers.Set(Reg16.SP, stackPointer);

            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            this.Memory.Set((ushort)(stackPointer + 1), highOrderByte);
            this.Memory.Set((ushort)(stackPointer), lowOrderByte);
        }

        public void LD(Reg8 register, byte value)
        {
            this.Registers.Set(register, value);
        }

        /// <summary>
        /// The value of the word found at the memory location pointed by SP is copied into reg16, then SP is increased by 2. 
        /// No flags are affected (except for the case of popping into AF).
        /// </summary>
        /// <param name="register"></param>
        public void POP(Reg16 register)
        {
            var stackPointer = this.Registers.Read(Reg16.SP);

            var highOrderByte = this.Memory.Read((ushort)(stackPointer + 1));
            var lowOrderByte = this.Memory.Read((ushort)(stackPointer));
            var fullByte = (ushort)((ushort)(highOrderByte << 8) | lowOrderByte);

            this.Registers.Set(Reg16.SP, (ushort)(stackPointer + 2));
            this.Registers.Set(register, fullByte);

        }
    }
}
