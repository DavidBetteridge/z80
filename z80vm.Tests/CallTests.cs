using Xunit;

namespace z80vm.Tests
{
    public class CallTests
    {
        [Fact]
        public void The_Address_Of_The_Instruction_Immediately_Following_The_Call_Is_Saved_To_The_Stack()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;

            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_MEMORY_ADDRESS);

            machine.POP(Reg16.BC);
            Assert.Equal(0x1003, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void Execution_Is_Continued_From_The_Address_Given_By_The_Label()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;

            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_MEMORY_ADDRESS);

            Assert.Equal(ANY_MEMORY_ADDRESS, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void A_Label_Can_Be_Supplied_Instead_Of_An_Address()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;
            const string ANY_LABEL = "Subroutine";

            var machine = new Machine();
            machine.Labels.Set(ANY_LABEL, ANY_MEMORY_ADDRESS);
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_LABEL);

            machine.POP(Reg16.BC);
            Assert.Equal(0x1003, machine.Registers.Read(Reg16.BC));
        }
    }
}
