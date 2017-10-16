using Xunit;
using z80vm;

namespace z80Assembler.Tests
{
    public class RunnerTests 
    {
        [Theory]
        [InlineData(0x00, "NOP")]
        [InlineData(0x01, "LD BC,nn")]
        [InlineData(0xDD09, "ADD IX,BC")]
        [InlineData(0xCB01, "RLC C")]
        [InlineData(0xED40, "IN B,(C)")]
        [InlineData(0xDDCB06, "RLC (IX+d)")]
        public void GivenAHexCodeShouldReturnTheCommand(int hexCode, string expectedCommand)
        {
            var instructionLookups = new InstructionLookups();
            instructionLookups.Load();

            var actualCommand = instructionLookups.LookupCommandFromHexCode(hexCode);

            Assert.Equal(expectedCommand, actualCommand);
        }

        [Fact]
        public void NOP_Should_Invoke_The_NOP_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);
            commandRunner.RunCommand("NOP");
        }

        [Fact]
        public void ADD_A_B_Should_Invoke_The_Add_A_B_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            machine.Registers.Set(Reg8.A, 10);
            machine.Registers.Set(Reg8.B, 20);
            commandRunner.RunCommand("ADD A,B");

            Assert.Equal(30, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void ADD_A_C_Should_Invoke_The_Add_A_C_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            machine.Registers.Set(Reg8.A, 10);
            machine.Registers.Set(Reg8.C, 20);
            commandRunner.RunCommand("ADD A,C");

            Assert.Equal(30, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void INC_B_Should_Invoke_The_INC_B_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            machine.Registers.Set(Reg8.B, 10);
            commandRunner.RunCommand("INC B");

            Assert.Equal(11, machine.Registers.Read(Reg8.B));
        }

        [Fact]
        public void DEC_C_Should_Invoke_The_DEC_C_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            machine.Registers.Set(Reg8.C, 10);
            commandRunner.RunCommand("DEC C");

            Assert.Equal(9, machine.Registers.Read(Reg8.C));
        }

        [Fact]
        public void ADD_A_123_Should_Invoke_The_ADD_A_n_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            machine.Registers.Set(Reg8.A, 10);
            commandRunner.RunCommand("ADD A,123");

            Assert.Equal(133, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void CALL_Should_Invoke_The_CALL_Command()
        {
            var machine = new Machine();
            var commandRunner = new CommandRunner(machine);

            commandRunner.RunCommand("CALL 100");

            Assert.Equal(100, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void Read_THE_NOP_Command_From_Memory()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 100);
            machine.Memory.Set(100, 0x00);

            var commandRunner = new CommandRunner(machine);
            commandRunner.RunNextCommand();

            Assert.Equal(101, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void Read_THE_DEC_C_Command_From_Memory()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 100);
            machine.Memory.Set(100, 0x0d);
            machine.Registers.Set(Reg8.C, 10);

            var commandRunner = new CommandRunner(machine);
            commandRunner.RunNextCommand();

            Assert.Equal(9, machine.Registers.Read(Reg8.C));
        }

        [Fact]
        public void Read_THE_CALL_100_Command_From_Memory()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 12);
            machine.Memory.Set(12, 0xcd);
            machine.Memory.Set(13, 1000);

            var commandRunner = new CommandRunner(machine);
            commandRunner.RunNextCommand();

            Assert.Equal(1000, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void Read_THE_ADD_A_123_Command_From_Memory()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 12);
            machine.Memory.Set(12, 0xc6);
            machine.Memory.Set(13, 123);
            machine.Registers.Set(Reg8.A, 10);

            var commandRunner = new CommandRunner(machine);
            commandRunner.RunNextCommand();

            Assert.Equal(133, machine.Registers.Read(Reg8.A));
        }
    }
}
