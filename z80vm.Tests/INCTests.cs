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

            var exception = Record.Exception(() => machine.INC(Read8BitValue(100)));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

        [Theory]
        [InlineData(Reg8.A,0,1)]
        [InlineData(Reg8.B,100,101)]
        public void TheValueOfTheOperandRegisterShouldBeIncrementedByOne(Reg8 register, byte startingValue,  byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, startingValue);

            machine.INC(Read8BitValue(register));

            Assert.Equal(expectedValue, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.HL, 0xFFFF, 0, 1)]
        public void TheValueHeldInTheReferencedMemoryShouldBeIncrementedByOne(Reg16 register, ushort memoryAddress, byte startingValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, startingValue);
            machine.Registers.Set(register, memoryAddress);

            machine.INC(Read8BitValue(valueAt(register)));

            Assert.Equal(expectedValue, machine.Memory.ReadByte(memoryAddress));
        }
    }
}
