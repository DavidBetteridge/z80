using Xunit;

namespace z80vm.Tests
{
    public class RLCTests : TestBase
    {

        [Theory]
        [InlineData(Reg8.A, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg8.A, 0b0101_0101, 0b1010_1010)]
        [InlineData(Reg8.A, 0b1000_0000, 0b0000_0001)]
        [InlineData(Reg8.B, 0b1000_0000, 0b0000_0001)]
        public void Given_A_Register_It_Should_Rotate_Its_Value_To_The_Left(Reg8 register, byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, originalValue);
            machine.RLC(register);

            Assert.Equal(expectedValue, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.HL, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg16.HL, 0b0101_0101, 0b1010_1010)]
        [InlineData(Reg16.HL, 0b1000_0000, 0b0000_0001)]
        public void Given_A_MemoryAddress_It_Should_Rotate_Its_Value_To_The_Left(Reg16 register, byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, 0x2000);
            machine.Memory.Set(0x2000, originalValue);

            machine.RLC(Value.valueAt(register));

            Assert.Equal(expectedValue, machine.Memory.ReadByte(0x2000));
        }

        [Theory]
        [InlineData(Reg16.IX, 1, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg16.IX, 2, 0b0101_0101, 0b1010_1010)]
        [InlineData(Reg16.IY, 3, 0b1000_0000, 0b0000_0001)]
        public void Given_A_MemoryAddress_Plus_Offset_It_Should_Rotate_Its_Value_To_The_Left(Reg16 register, sbyte offset, byte originalValue, byte expectedValue)
        {
            var baseAddress = (ushort)0x2000;
            var memoryAddress = (ushort)(baseAddress + offset);
            var machine = CreateMachine();
            machine.Registers.Set(register, baseAddress);
            machine.Memory.Set(memoryAddress, originalValue);

            machine.RLC(Value.valueAt(register).Add(offset));

            Assert.Equal(expectedValue, machine.Memory.ReadByte(memoryAddress));
        }
    }
}
