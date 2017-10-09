using Xunit;

namespace z80vm.Tests
{
    public class DAATests : TestBase
    {
        [Fact]
        public void Given_00000100_Then_No_Correction_Is_Needed()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0b0000_0100);

            machine.DAA();

            Assert.Equal(0b0000_0100, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void Given_00001010_Should_Be_Correct_To_10000()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0b0000_1010);

            machine.DAA();

            Assert.Equal(0b0001_0000, machine.Registers.Read(Reg8.A));
        }
    }
}
