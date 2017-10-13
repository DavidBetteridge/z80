using Xunit;
using static z80vm.op8;
using static z80vm.Value;

namespace z80vm.Tests
{
    public class INCTests : TestBase
    {
        /******************************************************************************************************************
        *  Tests for the 8 BIT Version of this command
        *******************************************************************************************************************/
        [Fact]
        public void AnErrorIsReportedIfThe8BITCommandIsNotValid()
        {
            var machine = CreateMachineWhereAllCommandsAreInvalid();

            var exception = Record.Exception(() => machine.INC(Read8BitValue(Reg8.A)));
            Assert.IsType<System.InvalidOperationException>(exception);
        }

        [Theory]
        [InlineData(0,1)]
        [InlineData(100,101)]
        public void TheValueOfTheOperandShouldBeIncrementedByOne(byte startingValue,  byte expectedValue)
        {
            var machine = CreateMachineWhereAllCommandsAreValid();
            var operand = new Moq.Mock<Iop8>();
            operand.Setup(o => o.Read(machine.Memory, machine.Registers)).Returns(startingValue);

            machine.INC(operand.Object);

            operand.Verify(o => o.Set(machine.Memory, machine.Registers, expectedValue), Moq.Times.Once);
        }
    }
}
