using Xunit;
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
        [InlineData("CALL PO,102", 0xE4)]
        [InlineData("LD B, 100", 0x6)]
        [InlineData("CPL", 0x2F)]
        [InlineData("LD (123),A", 0x32)]
        [InlineData("ADD   IX,BC", 0xDD09)]
        [InlineData("RRC  (HL)", 0xCB0E)]
        [InlineData("IN    B,(C)", 0xED40)]
        [InlineData("LD    L,RRC (IX+1)", 0xDDCB0D)]
        [InlineData("DJNZ -3", 0x10)]
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

        [Theory]
        [InlineData("NOP", 0, 0, 0)]
        [InlineData("LD (6),HL", 6, 0, 0)]
        [InlineData("LD B,10", 0, 10, 0)]
        [InlineData("LD BC,123", 0, 123, 0)]
        [InlineData("RLC D", 0, 0, 0)]
        [InlineData("LD E,(IX+21)", 0, 21, 0)]
        [InlineData("IN A,(C)", 0, 0, 0)]
        [InlineData("RST 8", 0, 0, 0)]
        [InlineData("LD A,SET 7,(IX+16)", 0, 0, 16)]
        [InlineData("DJNZ -3", 253, 0, 0)]  //-3 is 253 in twos compliment
        public void Given_Valid_Commands_Should_Return_Their_Operands(string program, int operand1, int operand2, int operand3)
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            var first = parsedCommands.First();
            Assert.False(first.IsInValid);
            Assert.Equal(operand1, first.Operand1);
            Assert.Equal(operand2, first.Operand2);
            Assert.Equal(operand3, first.Operand3);
        }


        [Theory]
        [InlineData("NOP", 0, 0, 0)]
        [InlineData("LD (6),HL", 2, 0, 0)]
        [InlineData("LD B,10", 0, 1, 0)]
        [InlineData("LD BC,123", 0, 2, 0)]
        [InlineData("RLC D", 0, 0, 0)]
        [InlineData("LD E,(IX+21)", 0, 1, 0)]
        [InlineData("IN A,(C)", 0, 0, 0)]
        [InlineData("RST 8", 0, 0, 0)]
        [InlineData("LD A,SET 7,(IX+16)", 0, 0, 1)]
        [InlineData("DJNZ -1", 1, 0, 0)]
        public void Given_Valid_Commands_Should_Return_The_Size_Of_The_Operands(string program, int operand1Size, int operand2Size, int operand3Size)
        {
            const ushort BASE_MEMORY_ADDRESS = 0;
            var parser = new Parser();

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            var first = parsedCommands.First();
            Assert.False(first.IsInValid);
            Assert.Equal(operand1Size, first.Operand1Length);
            Assert.Equal(operand2Size, first.Operand2Length);
            Assert.Equal(operand3Size, first.Operand3Length);
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

        [Fact]
        public void Given_A_Non_Zero_BaseMemoryAddress_The_Commands_Will_Be_Offsetted()
        {
            const ushort BASE_MEMORY_ADDRESS = 0xA000;
            var parser = new Parser();
            var program = @"CALL 100
NOP";

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Equal(0xA000, parsedCommands[0].MemoryLocation);  //CALL 100 is 3
            Assert.Equal(0xA003, parsedCommands[1].MemoryLocation);  //NOP is 1
        }

        [Fact]
        public void Labels_Can_Be_Replaced_By_Their_Memory_Addresses()
        {
            const ushort BASE_MEMORY_ADDRESS = 0xA000;
            var parser = new Parser();
            var program = @"CALL JumpTo
        NOP
        JumpTo: LD B,10
        LD BC,123
        NOP";

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Equal(0xA004, parsedCommands[0].Operand1);
            Assert.False(parsedCommands[2].IsInValid);

            //CALL 100 is 0xA000
            //NOP is 0xA003
            //LD B,10 is at 0xA004
            //LD BC,123 is at 0xA006
            //NOP is at 0xA009
        }

        [Fact]
        public void Labels_Can_Be_Replaced_By_Their_Offsets()
        {
            const ushort BASE_MEMORY_ADDRESS = 0xA000;
            var parser = new Parser();
            var program = @"LD A, 0
LD B, 10
Loop: ADD A, B
DJNZ Loop";

            var parsedCommands = parser.Parse(BASE_MEMORY_ADDRESS, program);

            Assert.Equal(-1, (sbyte)parsedCommands[3].Operand1);
            Assert.False(parsedCommands[3].IsInValid);
        }
    }
}
