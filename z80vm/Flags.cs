namespace z80vm
{
    /// <summary>
    /// Specifies which flag (bit in the 'A' register) we are dealing with
    /// </summary>
    public enum Flag
    {
        /// <summary>
        /// the Carry flag. It is set when the last operation caused a register to step over zero in either direction.
        /// </summary>
        C = 0,

        /// <summary>
        /// this bit indicates subtraction.
        /// </summary>
        N = 1,

        /// <summary>
        /// The Parity/Overflow flag, which has different meanings depending on which instruction alters it. When it is Parity, it tells you whether the number of set (1) bits in the register is odd or even. When it means Overflow, it indicates overflowing (leaving the 0...127 or -128...-1 intervals) by being set. Additional, in the case of certain operations the contents of the flip-flop controlling the interrupt also appear here.
        /// </summary>
        PV = 2,

        /// <summary>
        /// undefined
        /// </summary>
        Undefined3 = 3,

        /// <summary>
        /// the BCD Half Carry flag. It is set when there is a carry transfer from bit 3 to bit 4, and it is used for packed BCD correction
        /// </summary>
        H = 4,

        /// <summary>
        /// undefined
        /// </summary>
        Undefined5 = 5,

        /// <summary>
        /// the Zero flag, which is set if one of the registers was set to zero during the preceding operation. Note that not all operations resulting in zero alter this flag!
        /// </summary>
        Z = 6,

        /// <summary>
        /// the Sign flag, which is set if one of the registers changed its value from non-negative to negative. It is zero in all the other cases.
        /// </summary>
        S = 7
    }

    /// <summary>
    /// Flags are bits in the 'F' register.  Each bit has a different meaning
    /// and is set/used by various opcodes
    /// </summary>
    public class Flags
    {
        /// <summary>
        /// We need a reference back to the registers so that we can
        /// read/set the 'F' register
        /// </summary>
        private readonly Registers registers;

        public Flags(Registers registers)
        {
            this.registers = registers;
        }

        /// <summary>
        /// Sets the corresponding bit in the 'F' register to 1
        /// </summary>
        /// <param name="flag"></param>
        public void Set(Flag flag)
        {
            var valueOfRegisterF = this.registers.Read(Reg8.F);
            valueOfRegisterF = valueOfRegisterF.SetBit((int)flag);
            this.registers.Set(Reg8.F, valueOfRegisterF);
        }

        /// <summary>
        /// Sets the corresponding bit in the 'F' register to 0
        /// </summary>
        /// <param name="flag"></param>
        public void Clear(Flag flag)
        {
            var valueOfRegisterF = this.registers.Read(Reg8.F);
            valueOfRegisterF = valueOfRegisterF.ClearBit((int)flag);
            this.registers.Set(Reg8.F, valueOfRegisterF);
        }

        /// <summary>
        /// Reads the value of the corresponding bit in the 'F' register
        /// </summary>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool Read(Flag flag)
        {
            var valueOfRegisterF = this.registers.Read(Reg8.F);
            return valueOfRegisterF.ReadBit((int)flag);
        }
    }
}
