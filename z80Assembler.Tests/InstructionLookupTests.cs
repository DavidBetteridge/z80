using Xunit;

namespace z80Assembler.Tests
{
    public class InstructionLookupTests
    {
        [Theory]
        [InlineData("|[09 | 009] | ADD  HL,BC | ADD  IX,BC | RRC   C | *NOP | *LD    C,RRC(IX + d) |", 0x09)]
        [InlineData("|[7b | 123] | LD   A, E | +LD   A, E | BIT   7, E | LD    SP, (nn) | *BIT   7, (IX + d) |", 0x7b)]
        public void Check_That_Hex_Codes_Can_Be_Extracted_From_Lines(string line, byte expected)
        {
            var lookup = new InstructionLookup(line);
            Assert.Equal(expected, lookup.Hex);
        }

        [Theory]
        [InlineData("|[09 | 009] | ADD  HL,BC | ADD  IX,BC | RRC   C | *NOP | *LD    C,RRC(IX + d) |", "ADD HL,BC")]
        [InlineData("|[7b | 123] | LD   A, E | +LD   A, E | BIT   7, E | LD    SP, (nn) | *BIT   7, (IX + d) |", "LD A,E")]

        public void Check_That_Normals_Can_Be_Extracted_From_Lines(string line, string expected)
        {
            var lookup = new InstructionLookup(line);
            Assert.Equal(expected, lookup.Normal);
        }

        [Fact]
        public void Check_That_256_Instructions_Are_Loaded()
        {
            var instructionLookup = new InstructionLookups();

            instructionLookup.Load();

            Assert.Equal(256, instructionLookup.Count());
        }

        [Fact]
        public void Check_That_NOP_Returns_0x00()
        {
            var instructionLookup = new InstructionLookups();
            instructionLookup.Load();

            var hex = instructionLookup.LookupHexCodeFromNormal("NOP");

            Assert.Equal(0x00, hex);
        }

        [Fact]
        public void Check_That_LD_BC_n_Returns_0x01()
        {
            var instructionLookup = new InstructionLookups();
            instructionLookup.Load();

            var hex = instructionLookup.LookupHexCodeFromNormal("LD BC,n");

            Assert.Equal(0x01, hex);
        }
    }
}
