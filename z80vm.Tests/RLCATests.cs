using Xunit;

namespace z80vm.Tests
{
    public class RLCATests : TestBase
    {

        [Theory]
        [InlineData(0b0000_0000, 0b0000_0000)]
        [InlineData(0b0101_0101, 0b1010_1010)]
        [InlineData(0b1000_0000, 0b0000_0001)]
        public void Given_TheARegister_It_Should_Rotate_Its_Value_To_The_Left(byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            machine.RLCA();

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void The_RLCA_Command_Should_Reset_The_H_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.H);
            machine.RLCA();

            Assert.False(machine.Flags.Read(Flag.H));
        }

        [Fact]
        public void The_RLCA_Command_Should_Reset_The_N_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.RLCA();

            Assert.False(machine.Flags.Read(Flag.N));
        }

    }
}
