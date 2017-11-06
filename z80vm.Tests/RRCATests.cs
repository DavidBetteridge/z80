using Xunit;

namespace z80vm.Tests
{
    public class RRCATests : TestBase
    {
        [Theory]
        [InlineData(0b0000_0000, 0b0000_0000)]
        [InlineData(0b0101_0101, 0b1010_1010)]
        [InlineData(0b0000_0001, 0b1000_0000)]
        public void Given_TheARegister_It_Should_Rotate_Its_Value_To_The_Right(byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            machine.RRCA();

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Theory]
        [InlineData(0b0000_0000, false)]
        [InlineData(0b0101_0101, true)]
        public void After_A_Rotate_The_Carry_Bit_Should_Be_The_Original_Value_Of_Bit_0(byte originalValue, bool expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            machine.RRCA();

            if (expectedValue)
                Assert.True(machine.Flags.Read(Flag.C));
            else
                Assert.False(machine.Flags.Read(Flag.C));

        }


        [Fact]
        public void The_RRCA_Command_Should_Reset_The_H_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.H);
            machine.RRCA();

            Assert.False(machine.Flags.Read(Flag.H));
        }

        [Fact]
        public void The_RRCA_Command_Should_Reset_The_N_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.RRCA();

            Assert.False(machine.Flags.Read(Flag.N));
        }

    }
}
