using Xunit;
using z80vm;
using System.Collections.Generic;

namespace z80Assembler.Tests
{
    public class LoaderTests
    {
        [Theory]
        [InlineData(0x0c)]
        [InlineData(0x14)]
        public void Given_A_Single_OneByte_Command_It_Should_Be_Loaded_Into_Memory(int opCode)
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand = new ParsedCommand()
            {
                OpCode = opCode,
                TotalLength = 1
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(opCode, machine.Memory.ReadByte(0x00));
        }


        [Fact]
        public void Given_A_Single_Command_With_Operand1_It_Should_Be_Loaded_Into_Memory()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand = new ParsedCommand()
            {
                OpCode = 0xAA,
                Operand1 = 0xBBCC,
                Operand1Length = 2,
                TotalLength = 3
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(0xAA, machine.Memory.ReadByte(0x00));
            Assert.Equal(0xBBCC, machine.Memory.ReadWord(0x01));
        }

        [Fact]
        public void Given_A_Single_Command_With_Operand2_It_Should_Be_Loaded_Into_Memory()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand = new ParsedCommand()
            {
                OpCode = 0xAA,
                Operand2 = 0xBBCC,
                Operand2Length = 2,
                TotalLength = 3
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(0xAA, machine.Memory.ReadByte(0x00));
            Assert.Equal(0xBBCC, machine.Memory.ReadWord(0x01));
        }


        [Fact]
        public void Given_A_Single_Command_With_Operand3_It_Should_Be_Loaded_Into_Memory()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand = new ParsedCommand()
            {
                OpCode = 0xAA,
                Operand3 = 0xBBCC,
                Operand3Length = 2,
                TotalLength = 3
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(0xAA, machine.Memory.ReadByte(0x00));
            Assert.Equal(0xBBCC, machine.Memory.ReadWord(0x01));
        }

        [Fact]
        public void Given_Multiple_OneByte_Commands_They_Should_Be_Loaded_Into_Memory_One_After_Another()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand1 = new ParsedCommand()
            {
                OpCode = 0x0a,
                TotalLength = 1
            };
            var parsedCommand2 = new ParsedCommand()
            {
                OpCode = 0x0b,
                TotalLength = 1
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand1, parsedCommand2 };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(0x0a, machine.Memory.ReadByte(0x00));
            Assert.Equal(0x0b, machine.Memory.ReadByte(0x01));
        }


        [Fact]
        public void Given_Multiple_Varying_Size_Commands_They_Should_Be_Loaded_Into_Memory_One_After_Another()
        {
            var machine = new Machine();
            var loader = new Loader(machine);
            var parsedCommand1 = new ParsedCommand()
            {
                OpCode = 0xCCBBAA,
                TotalLength = 3
            };
            var parsedCommand2 = new ParsedCommand()
            {
                OpCode = 0xEEDD,
                TotalLength = 2
            };
            var parsedCommands = new List<ParsedCommand>() { parsedCommand1, parsedCommand2 };

            loader.LoadCommands(parsedCommands);

            Assert.Equal(0xAA, machine.Memory.ReadByte(0x00));
            Assert.Equal(0xBB, machine.Memory.ReadByte(0x01));
            Assert.Equal(0xCC, machine.Memory.ReadByte(0x02));
            Assert.Equal(0xDD, machine.Memory.ReadByte(0x03));
            Assert.Equal(0xEE, machine.Memory.ReadByte(0x04));
        }
    }
}
