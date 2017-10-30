using Xunit;
using z80vm;
using System.Linq;

namespace z80Assembler.Tests
{
    public class LoaderTests
    {

        [Theory]
        [InlineData("INC C", 0x0c)]
        [InlineData("INC D", 0x14)]
        public void ItShouldBePossibleToLoadASimpleCommandIntoMemory(string command, byte opcode)
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommand(command);

            Assert.Equal(opcode, machine.Memory.ReadByte(0x00));
        }

        [Fact]
        public void ItShouldBePossibleToLoadTwoSimpleCommandsIntoMemory()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommand("INC C");
            loader.LoadCommand("INC D");

            Assert.Equal(0x0c, machine.Memory.ReadByte(0x00));
            Assert.Equal(0x14, machine.Memory.ReadByte(0x01));
        }

        [Fact]
        public void ItShouldBePossibleToLoadTwoSimpleCommandsAtOnce()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommands(@"INC C
INC D");
            Assert.Equal(0x0c, machine.Memory.ReadByte(0x00));
            Assert.Equal(0x14, machine.Memory.ReadByte(0x01));
        }

        [Fact]
        public void ItShouldBePossibleToLoadACommandWithTakesAnOperand()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommands(@"CALL 100");
            Assert.Equal(0xcd, machine.Memory.ReadByte(0x00));
            Assert.Equal(100, machine.Memory.ReadWord(0x01));
        }


        [Fact]
        public void ItShouldBePossibleToReplaceTheListOfDefinedLabels()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommands(@"CALL JumpTo
JumpTo: INC C");
            Assert.Single(loader.Labels);
            Assert.Equal("JumpTo", loader.Labels.First().Key);
        }

        [Fact]
        public void ItShouldBePossibleToParseACommandWithALabelIn()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommands(@"CALL JumpTo
JumpTo: INC C");
            Assert.Equal(0xcd, machine.Memory.ReadByte(0x00));
        }

        [Fact]
        public void ItShouldBePossibleToReplaceALabelWithAMemoryAddress()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            loader.LoadCommands(@"CALL JumpTo
JumpTo: INC C");
            Assert.Equal(0xcd, machine.Memory.ReadByte(0x00));
            Assert.Equal(0x3, machine.Memory.ReadWord(0x01));
        }

        //With one operand
        //With two operands
        //Two commands with operands
    }
}
