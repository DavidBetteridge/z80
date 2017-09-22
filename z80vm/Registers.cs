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
        IY = 5
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
        F = 7
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
        public ushort SP { get; set; }

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
                default:
                    throw new Exception("Unknown register " + register);
            }
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
                default:
                    throw new Exception("Unknown register " + register);
            }
        }
    }
}
