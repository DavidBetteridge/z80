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
            var machine = CreateMachine();

            machine.Registers.Set(register, 0xAAAA);
            machine.Registers.Set(shadow, 0xBBBB);
            machine.EXX();

            Assert.Equal(0xAAAA, machine.Registers.Read(shadow));
            Assert.Equal(0xBBBB, machine.Registers.Read(register));
        }
        
        [Theory]
        [InlineData(Reg16.HL)]
        [InlineData(Reg16.IX)]
        [InlineData(Reg16.IY)]
        public void EXShouldExchangeTheValueOfTheTwoOperands(Reg16 operand2)
        {
            var machine = CreateMachine();

            //Following the SP pointer should give us AABB
            machine.Registers.Set(Reg16.SP, 0x1000);
            machine.Memory.Set(0x1001, 0xAA);
            machine.Memory.Set(0x1000, 0xBB);

            //Which we will swap with CCDD
            machine.Registers.Set(operand2, 0xCCDD);

            machine.EX(valueAt(Reg16.SP), operand2);

            Assert.Equal(0xCC, machine.Memory.ReadByte(0x1001));
            Assert.Equal(0xDD, machine.Memory.ReadByte(0x1000));
            Assert.Equal(0xAABB, machine.Registers.Read(operand2));
        }


        [Theory]
        [InlineData(Reg16.BC)]
        public void EXShouldErrorIfNotGivenValueAtSP(Reg16 operand1)
        {
            var machine = CreateMachine();
            var exception = Record.Exception(() => machine.EX(valueAt(operand1), Reg16.HL));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

        [Theory]
        [InlineData(Reg16.BC)]
        public void EXShouldErrorIfNotGivenHL_IX_IY(Reg16 operand2)
        {
            var machine = CreateMachine();
            var exception = Record.Exception(() => machine.EX(valueAt(Reg16.SP), operand2));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }


        [Fact]
        public void EXShouldExchangeDEandHL()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.DE, 0x1000);
            machine.Registers.Set(Reg16.HL, 0x2000);

            machine.EX(Reg16.DE, Reg16.HL);

            Assert.Equal(0x2000, machine.Registers.Read(Reg16.DE));
            Assert.Equal(0x1000, machine.Registers.Read(Reg16.HL));
        }

        [Theory]
        [InlineData(Reg16.BC, Reg16.HL)]
        [InlineData(Reg16.DE, Reg16.BC)]
        public void EXShouldErrorIfNotGivenDEandHL(Reg16 operand1, Reg16 operand2)
        {
            var machine = CreateMachine();
            machine.Registers.Set(operand1, 0x1000);
            machine.Registers.Set(operand2, 0x2000);

            var exception = Record.Exception(() => machine.EX(operand1, operand2));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

        [Fact]
        public void EXShouldExchangeAFandItsShadow()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.AF, 0x1000);
            machine.Registers.Set(Reg16Shadow.AF, 0x2000);

            machine.EX(Reg16.AF, Reg16Shadow.AF);

            Assert.Equal(0x1000, machine.Registers.Read(Reg16Shadow.AF));
            Assert.Equal(0x2000, machine.Registers.Read(Reg16.AF));
        }

        [Theory]
        [InlineData(Reg16Shadow.BC)]
        [InlineData(Reg16Shadow.DE)]
        [InlineData(Reg16Shadow.HL)]
        public void EXShouldErrorIfGivenAnyShadowRegisterOtherThanAF(Reg16Shadow register)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.AF, 0x1000);
            machine.Registers.Set(register, 0x2000);

            var exception = Record.Exception(() => machine.EX(Reg16.AF, register));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

        [Theory]
        [InlineData(Reg16.BC)]
        [InlineData(Reg16.DE)]
        [InlineData(Reg16.HL)]
        public void EXShouldErrorIfGivenAnyRegisterOtherThanAF(Reg16 register)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, 0x1000);
            machine.Registers.Set(Reg16Shadow.AF, 0x2000);

            var exception = Record.Exception(() => machine.EX(register, Reg16Shadow.AF));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }
        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator(), new FlagsEvaluator());
        }
    }
}
