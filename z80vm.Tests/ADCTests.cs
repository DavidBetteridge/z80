using Xunit;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class ADCTests
    {
        [Theory]
        [InlineData(100, 101)]
        [InlineData(0, 1)]
        public void AddingTwo8BitsValuesAndNoCarryShouldPutTheSumInTheFirstRegister(byte valueToAdd, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0x1);
            machine.ADC(Reg8.A, Read8BitValue(valueToAdd));

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Theory]
        [InlineData(100, 102)]
        public void AddingTwo8BitsValuesAndACarryShouldPutTheSumPlusOneInTheFirstRegister(byte valueToAdd, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.C);
            machine.Registers.Set(Reg8.A, 0x1);
            machine.ADC(Reg8.A, Read8BitValue(valueToAdd));

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void TheNFlagShouldBeCleared()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.Registers.Set(Reg8.A, 0x1);
            machine.ADC(Reg8.A, Read8BitValue(100));

            Assert.False(machine.Flags.Read(Flag.N));
        }

        [Fact]
        public void The_Flag_Evalulator_Should_Be_Called_And_All_Flags_Set()
        {
            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
            var machine = new Machine(new ConditionValidator(), fe.Object);
            fe.Setup(f => f.Evalulate(machine.Flags, 0, 2)).Callback(() =>
            {
                machine.Flags.Set(Flag.C);
                machine.Flags.Set(Flag.PV);
                machine.Flags.Set(Flag.S);
                machine.Flags.Set(Flag.Z);
                callCount++;
            });

            machine.ADC(Reg8.A, Read8BitValue(2));

            Assert.True(machine.Flags.Read(Flag.C));
            Assert.True(machine.Flags.Read(Flag.PV));
            Assert.True(machine.Flags.Read(Flag.S));
            Assert.True(machine.Flags.Read(Flag.Z));
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void The_Flag_Evalulator_Should_Be_Called_And_All_Flags_Cleared()
        {
            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
            var machine = new Machine(new ConditionValidator(), fe.Object);
            fe.Setup(f => f.Evalulate(machine.Flags, 0, 3)).Callback(() =>
            {
                machine.Flags.Clear(Flag.C);
                machine.Flags.Clear(Flag.PV);
                machine.Flags.Clear(Flag.S);
                machine.Flags.Clear(Flag.Z);
                callCount++;
            });
            machine.Flags.Set(Flag.C);
            machine.Flags.Set(Flag.PV);
            machine.Flags.Set(Flag.S);
            machine.Flags.Set(Flag.Z);

            machine.ADC(Reg8.A, Read8BitValue(2));

            Assert.False(machine.Flags.Read(Flag.C));
            Assert.False(machine.Flags.Read(Flag.PV));
            Assert.False(machine.Flags.Read(Flag.S));
            Assert.False(machine.Flags.Read(Flag.Z));
            Assert.Equal(1, callCount);
        }

        [Theory]
        [InlineData(1000, 1001)]
        public void AddingTwo16BitsValuesAndNoCarryShouldPutTheSumInTheFirstRegister(ushort valueToAdd, ushort expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.HL, 0x1);
            machine.Registers.Set(Reg16.BC, valueToAdd);
            machine.ADC(Reg16.HL, Reg16.BC);

            Assert.Equal(expectedValue, machine.Registers.Read(Reg16.HL));
        }

        [Theory]
        [InlineData(1000, 1002)]
        [InlineData(1001, 1003)]
        public void AddingTwo16BitsValuesAndACarryShouldPutTheSumPlusOneInTheFirstRegister(ushort valueToAdd, ushort expectedValue)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.C);
            machine.Registers.Set(Reg16.HL, 0x1);
            machine.Registers.Set(Reg16.BC, valueToAdd);
            machine.ADC(Reg16.HL, Reg16.BC);

            Assert.Equal(expectedValue, machine.Registers.Read(Reg16.HL));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator(), new FlagsEvaluator());
        }
    }
}
