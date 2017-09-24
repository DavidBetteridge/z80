using System;

namespace z80vm
{
    public enum Reg16
    {
        AF = 0,
        BC = 1,
        DE = 2,
        HL = 3,

        IX = 4,
        IY = 5,

        PC = 6,
        SP = 7,

        AF2 = 8,
        BC2 = 9,
        DE2 = 10,
        HL2 = 11,
    }

    public enum Reg8
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        H = 5,
        L = 6,
        F = 7,
        A2 = 8,
        B2 = 9,
        C2 = 10,
        D2 = 11,
        E2 = 12,
        F2 = 13,
        H2 = 14,
        L2 = 15,
        I = 16,
        R = 17
    }

    public class Registers
    {
        /// <summary>
        /// Accumulator
        /// </summary>
        private byte A;
        private byte B;
        private byte C;
        private byte D;
        private byte E;
        private byte H;
        private byte L;

        /// <summary>
        /// Flags
        /// </summary>
        private byte F;

        /// <summary>
        /// Alternative (shadow) registers
        /// </summary>
        private byte A2;
        private byte B2;
        private byte C2;
        private byte D2;
        private byte E2;
        private byte F2;
        private byte H2;
        private byte L2;


        //Index Register
        private ushort IX;  //Can split into IXH and IXL

        //Index Register
        private ushort IY;  //Can split into IYH and IYL

        /// <summary>
        /// Interrupt Vector
        /// </summary>
        private byte I;

        /// <summary>
        /// Refresh Register
        /// </summary>
        private byte R;

        /// <summary>
        /// Program counter
        /// </summary>
        private ushort PC;

        /// <summary>
        /// Stack pointer
        /// </summary>
        private ushort SP;

        public Registers()
        {
            this.SP = Memory.HIGHEST_ADDRESS;
        }

        public byte Read(Reg8 register)
        {
            switch (register)
            {
                case Reg8.A:
                    return A;
                case Reg8.B:
                    return B;
                case Reg8.C:
                    return C;
                case Reg8.D:
                    return D;
                case Reg8.E:
                    return E;
                case Reg8.H:
                    return H;
                case Reg8.L:
                    return L;
                case Reg8.F:
                    return F;

                case Reg8.A2:
                    return A2;
                case Reg8.B2:
                    return B2;
                case Reg8.C2:
                    return C2;
                case Reg8.D2:
                    return D2;
                case Reg8.E2:
                    return E2;
                case Reg8.H2:
                    return H2;
                case Reg8.L2:
                    return L2;
                case Reg8.F2:
                    return F2;

                case Reg8.I:
                    return I;
                case Reg8.R:
                    return R;

                default:
                    throw new Exception("Unknown register " + register);
            }
        }

        public ushort Read(Reg16 register)
        {
            var highOrderByte = default(byte);
            var lowOrderByte = default(byte);

            switch (register)
            {
                case Reg16.AF:
                    highOrderByte = A;
                    lowOrderByte = F;
                    break;
                case Reg16.BC:
                    highOrderByte = B;
                    lowOrderByte = C;
                    break;
                case Reg16.DE:
                    highOrderByte = D;
                    lowOrderByte = E;
                    break;
                case Reg16.HL:
                    highOrderByte = H;
                    lowOrderByte = L;
                    break;

                case Reg16.AF2:
                    highOrderByte = A2;
                    lowOrderByte = F2;
                    break;
                case Reg16.BC2:
                    highOrderByte = B2;
                    lowOrderByte = C2;
                    break;
                case Reg16.DE2:
                    highOrderByte = D2;
                    lowOrderByte = E2;
                    break;
                case Reg16.HL2:
                    highOrderByte = H2;
                    lowOrderByte = L2;
                    break;

                case Reg16.IX:
                    return IX;
                case Reg16.IY:
                    return IY;
                case Reg16.SP:
                    return SP;
                case Reg16.PC:
                    return PC;
                default:
                    break;
            }

            var ho = (ushort)(highOrderByte << 8);
            return (ushort)(ho | lowOrderByte);
        }

        public void Set(Reg8 register, byte value)
        {
            switch (register)
            {
                case Reg8.A:
                    A = value;
                    break;
                case Reg8.B:
                    B = value;
                    break;
                case Reg8.C:
                    C = value;
                    break;
                case Reg8.D:
                    D = value;
                    break;
                case Reg8.E:
                    E = value;
                    break;
                case Reg8.H:
                    H = value;
                    break;
                case Reg8.L:
                    L = value;
                    break;
                case Reg8.F:
                    F = value;
                    break;

                case Reg8.A2:
                    A2 = value;
                    break;
                case Reg8.B2:
                    B2 = value;
                    break;
                case Reg8.C2:
                    C2 = value;
                    break;
                case Reg8.D2:
                    D2 = value;
                    break;
                case Reg8.E2:
                    E2 = value;
                    break;
                case Reg8.H2:
                    H2 = value;
                    break;
                case Reg8.L2:
                    L2 = value;
                    break;
                case Reg8.F2:
                    F2 = value;
                    break;

                case Reg8.I:
                    I = value;
                    break;
                case Reg8.R:
                    R = value;
                    break;

                default:
                    throw new Exception("Unknown register " + register);
            }
        }

        public void Set(Reg16 register, ushort word)
        {
            var (highOrderByte, lowOrderByte) = word.Split();

            switch (register)
            {
                case Reg16.AF:
                    A = highOrderByte;
                    F = lowOrderByte;
                    break;
                case Reg16.BC:
                    B = highOrderByte;
                    C = lowOrderByte;
                    break;
                case Reg16.DE:
                    D = highOrderByte;
                    E = lowOrderByte;
                    break;
                case Reg16.HL:
                    H = highOrderByte;
                    L = lowOrderByte;
                    break;

                case Reg16.AF2:
                    A2 = highOrderByte;
                    F2 = lowOrderByte;
                    break;
                case Reg16.BC2:
                    B2 = highOrderByte;
                    C2 = lowOrderByte;
                    break;
                case Reg16.DE2:
                    D2 = highOrderByte;
                    E2 = lowOrderByte;
                    break;
                case Reg16.HL2:
                    H2 = highOrderByte;
                    L2 = lowOrderByte;
                    break;

                case Reg16.IX:
                    IX = word;
                    break;
                case Reg16.IY:
                    IY = word;
                    break;
                case Reg16.PC:
                    PC = word;
                    break;
                case Reg16.SP:
                    SP = word;
                    break;

                default:
                    throw new Exception("Unknown register " + register);
            }

        }
    }
}
