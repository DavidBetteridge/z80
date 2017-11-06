using Xunit;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class ADCTests : TestBase
    {
        /******************************************************************************************************************
        *  Tests for the 8 BIT Version of this command
        *******************************************************************************************************************/
        [Fact]
        public void AnErrorIsReportedIfThe8BITCommandIsNotValid()
        {
            var machine = CreateMachineWhereAllCommandsAreInvalid();

            var exception = Record.Exception(() => machine.ADC(Reg8.B, Read8BitValue(1)));
            Assert.IsType<System.InvalidOperationException>(exception);
        }

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
            var machine = CreateMachine();

            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
            fe.Setup(f => f.Evalulate(machine.Flags, 0, 2)).Callback(() =>
            {
                machine.Flags.Set(Flag.C);
                machine.Flags.Set(Flag.PV);
                machine.Flags.Set(Flag.S);
                machine.Flags.Set(Flag.Z);
                callCount++;
            });
            machine.SetFlagsEvaluator(fe.Object);

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
            var machine = CreateMachine();

            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
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

            machine.SetFlagsEvaluator(fe.Object);

            machine.ADC(Reg8.A, Read8BitValue(2));

            Assert.False(machine.Flags.Read(Flag.C));
            Assert.False(machine.Flags.Read(Flag.PV));
            Assert.False(machine.Flags.Read(Flag.S));
            Assert.False(machine.Flags.Read(Flag.Z));
            Assert.Equal(1, callCount);
        }

        /********************************************************************************************************************
         *  Tests for the 16 BIT Version of this command
         *******************************************************************************************************************/
        [Fact]
        public void AnErrorIsReportedIfThe16BITCommandIsNotValid()
        {
            var machine = CreateMachineWhereAllCommandsAreInvalid();

            var exception = Record.Exception(() => machine.ADC(Reg16.AF, Reg16.AF));
            Assert.IsType<System.InvalidOperationException>(exception);
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


    }
}
