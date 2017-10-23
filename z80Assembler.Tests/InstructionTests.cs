using Xunit;
using System.Linq;

namespace z80Assembler.Tests
{
    public class InstructionTests
    {
        [Theory]
        [InlineData("LD B, 100", "LD B,n")]
        [InlineData("CALL PO,102 ", "CALL PO,n")]
        public void Commands_Should_Be_Normalised(string command, string expectedResult)
        {
            var assembler = new Assembler();

            var normalised = assembler.Normalise(command);

            Assert.Equal(expectedResult, normalised);
        }

        [Fact]
        public void LD_B_100_Should_Be_Converted_To_0x06_100()
        {
            var assembler = new Assembler();
            
            var instructions = assembler.Parse("LD B, 100");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0X06, instructions.First());
            Assert.Equal(100, instructions.Skip(1).First());
        }

        [Fact]
        public void LD_B_111_Should_Be_Converted_To_0x06_111()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("LD B, 111");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0X06, instructions.First());
            Assert.Equal(111, instructions.Skip(1).First());
        }

        [Fact]
        public void LD_C_111_Should_Be_Converted_To_0x0E_111()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("LD C, 111");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0X0E, instructions.First());
            Assert.Equal(111, instructions.Skip(1).First());
        }

        [Fact]
        public void NOP_Should_Be_Converted_To_0x00()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("NOP");

            Assert.Single(instructions);
            Assert.Equal(0x00, instructions.First());
        }

        [Fact]
        public void CPL_Should_Be_Converted_To_0x2f()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("CPL");

            Assert.Single(instructions);
            Assert.Equal(0x2f, instructions.First());
        }

        [Fact]
        public void LD_nn_A_Should_Be_Converted_To_0x32()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("LD (123),A");

            Assert.Equal(3, instructions.Count());
            Assert.Equal(0x32, instructions.First());
            Assert.Equal(123, instructions.Skip(1).First());
            Assert.Equal(0, instructions.Skip(2).First());
        }

        [Fact]
        public void LD_500_A_Should_Be_Converted_To_0x32()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("LD (500),A");

            Assert.Equal(3, instructions.Count());
            Assert.Equal(0x32, instructions.First());
            Assert.Equal(0b1111_0100, instructions.Skip(1).First());
            Assert.Equal(0b0000_0001, instructions.Skip(2).First());
        }


        //DD Tests
        [Fact]
        public void ADD_IX_A_Should_Be_Converted_To_0xDD_0x09()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("ADD   IX,BC");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0xDD, instructions.First());
            Assert.Equal(0x09, instructions.Skip(1).First());
        }


        //CB Tests
        [Fact]
        public void RRC_HL_Should_Be_Converted_To_0xCB_0x0E()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("RRC  (HL)");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0xCB, instructions.First());
            Assert.Equal(0x0E, instructions.Skip(1).First());
        }

        //EDTests
        [Fact]
        public void IN_B_C_Should_Be_Converted_To_0xED_0x40()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("IN    B,(C)");

            Assert.Equal(2, instructions.Count());
            Assert.Equal(0xED, instructions.First());
            Assert.Equal(0x40, instructions.Skip(1).First());
        }

        //DDCB |[ 0d  *LD    L,RRC (IX+d)
        [Fact]
        public void LD_L_RRC_IX_D_Should_Be_Converted_To_0xDDCB_0x0D()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("LD    L,RRC (IX+d)");

            Assert.Equal(3, instructions.Count());
            Assert.Equal(0xDD, instructions.First());
            Assert.Equal(0xCB, instructions.Skip(1).First());
            Assert.Equal(0x0D, instructions.Skip(2).First());
        }

        [Fact]
        public void An_Invalid_Command_Returns_No_Instructions()
        {
            var assembler = new Assembler();

            var instructions = assembler.Parse("ADD B,A");
            Assert.Empty(instructions);
        }
    }
}

