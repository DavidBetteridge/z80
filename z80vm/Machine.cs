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
