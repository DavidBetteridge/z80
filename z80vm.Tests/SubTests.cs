using Xunit;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class SubTests
    {
        [Theory]
        [InlineData(101, 100, 1)]
        [InlineData(200, 100, 100)]
        public void Subtracting_A_Value_From_A_Should_Place_The_Result_In_A(byte valueInA, byte valueToSubtract, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, valueInA);
            machine.SUB(Reg8.A, Read8BitValue(valueToSubtract));

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void The_N_Flag_Should_Be_Set()
        {
            var machine = CreateMachine();
            machine.SUB(Reg8.A, Read8BitValue(1));

            Assert.Equal(true, machine.Flags.Read(Flag.N));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator(), new FlagsEvaluator());
        }
    }
}
