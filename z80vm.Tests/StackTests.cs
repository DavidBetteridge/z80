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
        [InlineData(Reg16.AF, 100)]
        public void PushingAValueOnToTheStackCopiesTheValueOfTheRegisterIntoTheMemoryLocationPointedToBySP(Reg16 register, int value)
        {
            var machine = new Machine();

            var currentValueOfSP = machine.Registers.SP;
            //machine.Registers.Set(register, value);  //Needs to change to a LD
            machine.PUSH(register);

            Assert.True(false);
//            Assert.Equal(machine.Memory.Read(currentValueOfSP), value);
        }



    }
}
