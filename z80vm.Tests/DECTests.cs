using Xunit;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class DECTests : TestBase
    {
        /******************************************************************************************************************
        *  Tests for the 8 BIT Version of this command
        *******************************************************************************************************************/
        [Fact]
        public void AnErrorIsReportedIfThe8BITCommandIsNotValid()
        {
            var machine = CreateMachineWhereAllCommandsAreInvalid();

            var exception = Record.Exception(() => machine.DEC(Read8BitValue(Reg8.F)));
            Assert.IsType<System.InvalidOperationException>(exception);
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(100, 99)]
        public void TheValueOfThe8BitOperandShouldBeDecrementedByOne(byte startingValue, byte expectedValue)
        {
            var machine = CreateMachineWhereAllCommandsAreValid();

            var operand = new Moq.Mock<IOp8>();
            operand.Setup(o => o.Read(machine.Memory, machine.Registers)).Returns(startingValue);

            machine.DEC(operand.Object);

            operand.Verify(o => o.Set(machine.Memory, machine.Registers, expectedValue), Moq.Times.Once);
        }

        /******************************************************************************************************************
        *  Tests for the 16 BIT Version of this command
        *******************************************************************************************************************/
        [Theory]
        [InlineData(1, 0)]
        [InlineData(100, 99)]
        public void TheValueOfThe16BitOperandShouldBeDecrementedByOne(byte startingValue, byte expectedValue)
        {
            var machine = CreateMachineWhereAllCommandsAreValid();
            machine.Registers.Set(Reg16.BC, startingValue);

            machine.DEC(Reg16.BC);

            Assert.Equal(expectedValue, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void AnErrorIsReportedIfThe16BITCommandIsNotValid()
        {
            var machine = CreateMachineWhereAllCommandsAreInvalid();

            var exception = Record.Exception(() => machine.DEC(Reg16.AF));
            Assert.IsType<System.InvalidOperationException>(exception);
        }
    }
}
