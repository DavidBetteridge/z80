using Xunit;

namespace z80vm.Tests
{
    public class RLATests : TestBase
    {

        [Theory]
        [InlineData(false, 0b0000_0000, 0b0000_0000)]
        [InlineData(false, 0b0000_1111, 0b0001_1110)]
        [InlineData(true, 0b0000_1111, 0b0001_1111)]
        public void Given_A_Register_It_Should_Rotate_Its_Value_To_The_Left(bool carrySet, byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            if (carrySet) machine.Flags.Set(Flag.C);
            machine.RLA();

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }


        [Theory]
        [InlineData(0b0000_0000, false)]
        [InlineData(0b1111_1111, true)]
        public void After_A_Rotate_The_Carry_Bit_Should_Be_The_Original_Value_Of_Bit_7(byte originalValue, bool expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            machine.RLA();

            if (expectedValue)
                Assert.True(machine.Flags.Read(Flag.C));
            else
                Assert.False(machine.Flags.Read(Flag.C));

        }
    }
}
