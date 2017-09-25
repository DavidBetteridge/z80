using System;

namespace z80vm
{
    /// <summary>
    /// Used to specify a 16bit register
    /// </summary>
    public enum Reg16
    {
        /// <summary>
        /// A and F combined.   (A is the high order byte)
        /// Not normally used because of the F, which is used to store flags
        /// </summary>
        AF = 0,

        /// <summary>
        /// B and C combined.   (B is the high order byte)
        /// Used by instructions and code sections that operate on streams of bytes as a byte counter. Is also used as a 16 bit counter. 
        /// holds the address of a memory location that is a destination.
        /// </summary>
        BC = 1,

        /// <summary>
        /// D and E combined.   (E is the high order byte)
        /// Holds the address of a memory location that is a destination.
        /// </summary>
        DE = 2,

        /// <summary>
        /// H and L combined.   (H is the high order byte)
        /// The general 16 bit register, it's used pretty much everywhere you use 16 bit registers. It's most common uses are for 16 bit arithmetic and storing the addresses of stuff (strings, pictures, labels, etc.). 
        /// Note that HL usually holds the original address while DE holds the destination address.
        /// </summary>
        HL = 3,

        /// <summary>
        /// Is called an index register. It's use is similar to HL, but it's use should be limited as it has other purposes, and also runs slower than HL.
        /// </summary>
        IX = 4,

        /// <summary>
        /// Is an index register.It holds the location of the system flags and is used when you want to change a certain flag.For now, we won't do anything to it.
        /// </summary>
        IY = 5,

        /// <summary>
        /// The program counter. It hold the point in memory that the processor is executing code from.No function can change PC except by actually jumping to a different location in memory.
        /// </summary>
        PC = 6,

        /// <summary>
        /// The stack pointer. It holds the current address of the top of the stack.
        /// </summary>
        SP = 7,
    }

    /// <summary>
    /// Used to specify a 16bit shadow register
    /// </summary>
    public enum Reg16Shadow
    {
        /// <summary>
        /// A and F combined.   (A is the high order byte)
        /// Not normally used because of the F, which is used to store flags
        /// </summary>
        AF = 0,

        /// <summary>
        /// B and C combined.   (B is the high order byte)
        /// Used by instructions and code sections that operate on streams of bytes as a byte counter. Is also used as a 16 bit counter. 
        /// holds the address of a memory location that is a destination.
        /// </summary>
        BC = 1,

        /// <summary>
        /// D and E combined.   (E is the high order byte)
        /// Holds the address of a memory location that is a destination.
        /// </summary>
        DE = 2,

        /// <summary>
        /// H and L combined.   (H is the high order byte)
        /// The general 16 bit register, it's used pretty much everywhere you use 16 bit registers. It's most common uses are for 16 bit arithmetic and storing the addresses of stuff (strings, pictures, labels, etc.). 
        /// Note that HL usually holds the original address while DE holds the destination address.
        /// </summary>
        HL = 3
    }

    /// <summary>
    /// Used to specify an 8bit register
    /// </summary>
    public enum Reg8
    {
        /// <summary>
        /// Accumulator
        /// It is the primary register for arithmetic operations and accessing memory
        /// </summary>
        A = 0,

        /// <summary>
        /// Cmmonly used as an 8-bit counter
        /// </summary>
        B = 1,

        /// <summary>
        /// Used to interface with hardware ports
        /// </summary>
        C = 2,

        /// <summary>
        /// Not normally used in its 8-bit form. Instead, it is used in conjuncture with E
        /// </summary>
        D = 3,

        /// <summary>
        /// Not normally used in its 8-bit form. Instead, it is used in conjuncture with D
        /// </summary>
        E = 4,

        /// <summary>
        /// Not normally used in its 8-bit form. Instead, it is used in conjuncture with L
        /// </summary>
        H = 5,

        /// <summary>
        /// Not normally used in its 8-bit form. Instead, it is used in conjuncture with H
        /// </summary>
        L = 6,

        /// <summary>
        /// Flags
        /// It is the one register you cannot mess with on the byte level
        /// </summary>
        F = 7,

        /// <summary>
        /// Interrupt Vector
        /// </summary>
        I = 8,

        /// <summary>
        /// Refresh Register
        /// </summary>
        R = 9,

        /// <summary>
        /// High-order byte of IX the register
        /// </summary>
        IXH = 10,

        /// <summary>
        /// Lower order byte of IX the register
        /// </summary>
        IXL = 11,

        /// <summary>
        /// High order byte of the IY register 
        /// </summary>
        IYH = 12,

        /// <summary>
        /// Lower order byte of the IY register
        /// </summary>
        IYL = 13
    }

    public class Registers
    {
        // All 8 bit registers
        private byte A;
        private byte B;
        private byte C;
        private byte D;
        private byte E;
        private byte F;
        private byte H;
        private byte L;
        private byte I;
        private byte R;

        // Alternative (shadow) registers
        private byte A2;
        private byte B2;
        private byte C2;
        private byte D2;
        private byte E2;
        private byte F2;
        private byte H2;
        private byte L2;

        // 16bit registers
        private ushort IX;
        private ushort IY;
        private ushort PC;
        private ushort SP;

        public Registers()
        {
            this.SP = Memory.HIGHEST_ADDRESS;
        }

        /// <summary>
        /// Reads the value of an 8bit register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
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
                case Reg8.I:
                    return I;
                case Reg8.R:
                    return R;

                case Reg8.IXH:
                    var (hx, _) = IX.Split();
                    return hx;

                case Reg8.IXL:
                    var (_, lx) = IX.Split();
                    return lx;

                case Reg8.IYH:
                    var (hy, _) = IY.Split();
                    return hy;

                case Reg8.IYL:
                    var (_, ly) = IY.Split();
                    return ly;

                default:
                    throw new Exception("Unknown register " + register);
            }
        }

        /// <summary>
        /// Reads the value of a 16 bit register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
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

            return MakeWord(highOrderByte, lowOrderByte);
        }

        /// <summary>
        /// Sets the value into a 16bit shadow register
        /// </summary>
        /// <param name="register"></param>
        /// <param name="word"></param>
        public void Set(Reg16Shadow register, ushort word)
        {
            var (highOrderByte, lowOrderByte) = word.Split();

            switch (register)
            {
                case Reg16Shadow.AF:
                    A2 = highOrderByte;
                    F2 = lowOrderByte;
                    break;
                case Reg16Shadow.BC:
                    B2 = highOrderByte;
                    C2 = lowOrderByte;
                    break;
                case Reg16Shadow.DE:
                    D2 = highOrderByte;
                    E2 = lowOrderByte;
                    break;
                case Reg16Shadow.HL:
                    H2 = highOrderByte;
                    L2 = lowOrderByte;
                    break;

                default:
                    throw new Exception("Unknown register " + register);
            }
        }

        /// <summary>
        /// Reads the value of a 16bit shadow register
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public ushort Read(Reg16Shadow register)
        {
            var highOrderByte = default(byte);
            var lowOrderByte = default(byte);

            switch (register)
            {
                case Reg16Shadow.AF:
                    highOrderByte = A2;
                    lowOrderByte = F2;
                    break;
                case Reg16Shadow.BC:
                    highOrderByte = B2;
                    lowOrderByte = C2;
                    break;
                case Reg16Shadow.DE:
                    highOrderByte = D2;
                    lowOrderByte = E2;
                    break;
                case Reg16Shadow.HL:
                    highOrderByte = H2;
                    lowOrderByte = L2;
                    break;
                default:
                    throw new Exception("Unknown register " + register);
            }

            return MakeWord(highOrderByte, lowOrderByte);
        }

        /// <summary>
        /// Sets the value of an 8bit register
        /// </summary>
        /// <param name="register"></param>
        /// <param name="value"></param>
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
                case Reg8.I:
                    I = value;
                    break;
                case Reg8.R:
                    R = value;
                    break;

                case Reg8.IXH:
                    var (_, lx) = IX.Split();
                    IX = MakeWord(value, lx);
                    break;

                case Reg8.IXL:
                    var (hx, _) = IX.Split();
                    IX = MakeWord(hx, value);
                    break;

                case Reg8.IYH:
                    var (_, ly) = IY.Split();
                    IY = MakeWord(value, ly);
                    break;

                case Reg8.IYL:
                    var (hy, _) = IY.Split();
                    IY = MakeWord(hy, value);
                    break;

                default:
                    throw new Exception("Unknown register " + register);
            }
        }

        /// <summary>
        /// Combines two bytes to make a word
        /// </summary>
        /// <param name="highOrderByte"></param>
        /// <param name="lowOrderByte"></param>
        /// <returns></returns>
        private ushort MakeWord(byte highOrderByte, byte lowOrderByte) 
            => (ushort)(((ushort)(highOrderByte << 8)) | lowOrderByte);


        /// <summary>
        /// Sets the value of a 16bit register
        /// </summary>
        /// <param name="register"></param>
        /// <param name="word"></param>
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
