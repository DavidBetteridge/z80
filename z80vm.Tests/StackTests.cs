using Xunit;

namespace z80vm.Tests
{
    public class StackTests
    {
        [Fact]
        public void OnANewMachineTheSPRegisterShouldContainTheHighestPossibleMemoryAddress()
        {
            var machine = new Machine();
            Assert.Equal(Memory.HIGHEST_ADDRESS, machine.Registers.SP);
        }

        [Fact]
        public void PushingAValueOnToTheStackShouldDecreaseTheSPRegisterByTwo()
        {
            var machine = new Machine();
            var currentValueOfSP = machine.Registers.SP;

            machine.PUSH(Reg16.AF);

            Assert.Equal(currentValueOfSP - 2, machine.Registers.SP);
        }

        [Theory]
        [InlineData(Reg16.BC)]
        public void PushingAValueOnToTheStackCopiesTheValueOfTheRegisterIntoTheMemoryLocationPointedToBySP(Reg16 register)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            var machine = new Machine();

            var currentValueOfSP = machine.Registers.SP;
            machine.Registers.Set(register, 0xABCD);  
            machine.PUSH(register);

            Assert.Equal(machine.Memory.Read(currentValueOfSP), 0xAB);
            Assert.Equal(machine.Memory.Read(--currentValueOfSP), 0xCD);
        }



    }
}
