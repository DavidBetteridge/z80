﻿using Xunit;
using System.Linq;

namespace z80Assembler.Tests
{
    public class ParserTests
    {
        [Fact]
        public void Given_No_Command_Should_Return_Nothing()
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var program = "";

            var parser = new Parser();
            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Empty(parsedCommands);
        }

        [Fact]
        public void Given_Two_Commands_Should_Return_TwoParsedCommands()
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var program = @"CALL 100
CALL 100";

            var parser = new Parser();
            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Equal(2, parsedCommands.Count);
        }

        [Theory]
        [InlineData("NOP", 0x00)]
        [InlineData("LD BC,123", 0x01)]
        [InlineData("RLC D", 0xCB02)]
        [InlineData("LD (1),HL", 0x22)]
        [InlineData("LD E,(IX+1)", 0xDD5e)]
        [InlineData("IN A,(C)", 0xED78)]
        [InlineData("RST 8", 0xCF)]
        [InlineData("LD A,SET 7,(IX+16)", 0xDDCBFF)]
        public void Given_Valid_Commands_Should_Return_Their_OpCodes(string program, int opCode)
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            var first = parsedCommands.First();
            Assert.False(first.IsInValid);
            Assert.Equal(opCode, first.OpCode);
        }

        [Theory]
        [InlineData("RST 7")]
        [InlineData("ABCD")]
        public void Given_An_Invalid_Command_Their_InValidCommandFlag_Should_Be_Set(string program)
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            var first = parsedCommands.First();
            Assert.True(first.IsInValid);
        }

        [Theory]
        [InlineData("NOP", 1)]
        [InlineData("LD B,10", 2)]
        [InlineData("LD BC,123", 3)]
        [InlineData("RLC D", 2)]  //Has a CB prefix
        [InlineData("LD (1),HL", 3)]
        [InlineData("LD E,(IX+1)", 3)] //Has a DD prefix
        [InlineData("IN A,(C)", 2)]  //Has a ED prefiex
        [InlineData("RST 8", 1)]
        [InlineData("LD A,SET 7,(IX+16)", 4)]  //Has a DDCB prefiex
        public void Given_Valid_Commands_Should_Return_Their_Total_Lengths(string program, int lengthInBytes)
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            var first = parsedCommands.First();
            Assert.False(first.IsInValid);
            Assert.Equal(lengthInBytes, first.TotalLength);
        }

        [Fact]
        public void Given_Valid_Commands_Their_Memory_Locations_Should_Be_Defined()
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();
            var program = @"CALL 100
NOP
LD B,10
LD BC,123
NOP";

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Equal(0, parsedCommands[0].MemoryLocation);  //CALL 100 is 3
            Assert.Equal(3, parsedCommands[1].MemoryLocation);  //NOP is 1
            Assert.Equal(4, parsedCommands[2].MemoryLocation);  //LD B,10 is 2
            Assert.Equal(6, parsedCommands[3].MemoryLocation);  //LD BC,123 is 3
            Assert.Equal(9, parsedCommands[4].MemoryLocation);
        }
    }
}
