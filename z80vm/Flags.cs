using System;

namespace z80vm
{
    public enum Flag
    {
        C = 0,
        N = 1,
        PV = 2,
        Undefined3 = 3,
        H = 4,
        Undefined5 = 5,
        Z = 6,
        S = 7
    }

    public class Flags
    {
        private readonly Registers registers;

        public Flags(Registers registers)
        {
            this.registers = registers;
        }

        public void Set(Flag flag)
        {
            var a = this.registers.Read(Reg8.A);

            switch (flag)
            {
                case Flag.C:
                    a |= 0b0000_0001;
                    break;
                case Flag.N:
                    a |= 0b0000_0010;
                    break;
                case Flag.PV:
                    a |= 0b0000_0100;
                    break;
                case Flag.Undefined3:
                    a |= 0b0000_1000;
                    break;
                case Flag.H:
                    a |= 0b0001_0000;
                    break;
                case Flag.Undefined5:
                    a |= 0b0010_0000;
                    break;
                case Flag.Z:
                    a |= 0b0100_0000;
                    break;
                case Flag.S:
                    a |= 0b1000_0000;
                    break;
                default:
                    break;
            }

            this.registers.Set(Reg8.A, a);

        }

        public void Clear(Flag flag)
        {
            var a = this.registers.Read(Reg8.A);

            switch (flag)
            {
                case Flag.C:
                    a &= 0b1111_1110;
                    break;
                case Flag.N:
                    a &= 0b1111_1101;
                    break;
                case Flag.PV:
                    a &= 0b1111_1011;
                    break;
                case Flag.Undefined3:
                    a &= 0b1111_0111;
                    break;
                case Flag.H:
                    a &= 0b1110_1111;
                    break;
                case Flag.Undefined5:
                    a &= 0b1101_1111;
                    break;
                case Flag.Z:
                    a &= 0b1011_1111;
                    break;
                case Flag.S:
                    a &= 0b0111_1111;
                    break;
                default:
                    break;
            }

            this.registers.Set(Reg8.A, a);

        }

        public bool Read(Flag flag)
        {
            var a = this.registers.Read(Reg8.A);

            return (a & (1 << (int)flag)) != 0;
        }
    }
}
