using Xunit;
using static z80vm.Value;

namespace z80vm.Tests
{
    public class ExchangeTests
    {
        [Theory]
        [InlineData(Reg16.BC, Reg16Shadow.BC)]
        [InlineData(Reg16.DE, Reg16Shadow.DE)]
        [InlineData(Reg16.HL, Reg16Shadow.HL)]
        public void EXXShouldSwapARegisterWithItsShadow(Reg16 register, Reg16Shadow shadow)
        {
            var machine = new Machine();

            machine.Registers.Set(register, 0xAAAA);
            machine.Registers.Set(shadow, 0xBBBB);
            machine.EXX();

            Assert.Equal(0xAAAA, machine.Registers.Read(shadow));
            Assert.Equal(0xBBBB, machine.Registers.Read(register));
        }

        //EX
        //Syntax: ex op1, op2
        //The values of the two operands are exchanged.
        //ex de, hl, 
        //ex af, af’. 
        //The last one naturally alters the flags (exchanges them with the shadow flags). You cannot exchange the order given, e. g.there is no exhl,de!
        [Theory]
        [InlineData(Reg16.HL)]
        [InlineData(Reg16.IX)]
        [InlineData(Reg16.IY)]
        public void EXShouldExchangeTheValueOfTheTwoOperands(Reg16 operand2)
        {
            var machine = new Machine();

            //Following the SP pointer should give us AABB
            machine.Registers.Set(Reg16.SP, 0x1000);
            machine.Memory.Set(0x1001, 0xAA);
            machine.Memory.Set(0x1000, 0xBB);

            //Which we will swap with CCDD
            machine.Registers.Set(operand2, 0xCCDD);

            machine.EX(valueAt(Reg16.SP), operand2);

            Assert.Equal(0xCC, machine.Memory.Read(0x1001));
            Assert.Equal(0xDD, machine.Memory.Read(0x1000));
            Assert.Equal(0xAABB, machine.Registers.Read(operand2));
        }

    }
}
