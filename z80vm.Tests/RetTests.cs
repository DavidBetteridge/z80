using Xunit;

namespace z80vm.Tests
{
    public class RetTests
    {
        [Fact]
        public void TheAddressOfNextInstructionShouldBeSetToTheWordOnTheTopOfStack()
        {
            var machine = new Machine();

            // Put any address on the stack
            machine.Registers.Set(Reg16.BC, 0xABCD);
            machine.PUSH(Reg16.BC);

            machine.RET();

            Assert.Equal(0xABCD, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void TheStackPointerShouldBeIncreasedBy2()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.SP, 0x1000);

            machine.RET();

            Assert.Equal(0x1002, machine.Registers.Read(Reg16.SP));
        }
        

    }
}
