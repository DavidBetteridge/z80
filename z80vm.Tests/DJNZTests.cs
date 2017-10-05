using Xunit;

namespace z80vm.Tests
{
    public class DJNZTests
    {
        [Fact]
        public void BShouldBeDecreasedByOne()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.B, 10);

            machine.DJNZ(0x000);

            Assert.Equal(9, machine.Registers.Read(Reg8.B));
        }

        [Fact]
        public void IfBIs0ThenBShouldBecome255()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.B, 0);

            machine.DJNZ(0x000);

            Assert.Equal(255, machine.Registers.Read(Reg8.B));
        }

        [Theory]
        [InlineData(10)]
        [InlineData(0)]
        public void AJumpShouldHappenIfBIsNotZero(byte initialB)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.B, initialB);
            machine.Registers.Set(Reg16.PC, 1000);

            machine.DJNZ(100);

            Assert.Equal(1100, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void AJumpShouldNotHappenIfBIsZero()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.B, 1);
            machine.Registers.Set(Reg16.PC, 1000);

            machine.DJNZ(100);

            Assert.Equal(1000, machine.Registers.Read(Reg16.PC));
        }

        private static Machine CreateMachine()
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            return new Machine(conditionValidator.Object, new FlagsEvaluator());
        }
    }
}
