using Xunit;

namespace z80vm.Tests
{
    public class LDTests
    {
        [Theory]
        [InlineData(Reg8.A, 100)]
        [InlineData(Reg8.B, 101)]
        [InlineData(Reg8.C, 102)]
        [InlineData(Reg8.D, 103)]
        [InlineData(Reg8.E, 104)]
        [InlineData(Reg8.H, 105)]
        [InlineData(Reg8.L, 106)]
        public void ItShouldBePossibleToLoadAn8BitRegistersWithhImmediateValue(Reg8 register, byte value)
        {
            var machine = new Machine();
            machine.LD(register, value);

            Assert.Equal(value, machine.Registers.Read(register));
        }

        [Fact]
        public void LoadingOneRegisterShouldNotChangeAnotherRegister()
        {
            var machine = new Machine();

            var currentValue = machine.Registers.Read(Reg8.B);
            machine.LD(Reg8.C, 123);

            Assert.Equal(currentValue, machine.Registers.Read(Reg8.B));
        }
    }
}
