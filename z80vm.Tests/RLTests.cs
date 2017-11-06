using Xunit;

namespace z80vm.Tests
{
    public class RLTests : TestBase
    {

        [Theory]
        [InlineData(Reg8.A, false, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg8.A, false, 0b0000_1111, 0b0001_1110)]
        [InlineData(Reg8.B, true, 0b0000_1111, 0b0001_1111)]
        public void Given_A_Register_It_Should_Rotate_Its_Value_To_The_Left(Reg8 register, bool carrySet, byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, originalValue);
            if (carrySet) machine.Flags.Set(Flag.C);
            machine.RL(register);

            Assert.Equal(expectedValue, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.HL, false, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg16.HL, false, 0b0101_0101, 0b1010_1010)]
        [InlineData(Reg16.HL, true, 0b1000_0000, 0b0000_0001)]
        public void Given_A_MemoryAddress_It_Should_Rotate_Its_Value_To_The_Left(Reg16 register, bool carrySet, byte originalValue, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, 0x2000);
            machine.Memory.Set(0x2000, originalValue);
            if (carrySet) machine.Flags.Set(Flag.C);

            machine.RL(Value.valueAt(register));

            Assert.Equal(expectedValue, machine.Memory.ReadByte(0x2000));
        }

        [Theory]
        [InlineData(Reg16.HL, 10, false, 0b0000_0000, 0b0000_0000)]
        [InlineData(Reg16.HL, 20, false, 0b0101_0101, 0b1010_1010)]
        [InlineData(Reg16.HL, 30, true, 0b1000_0000, 0b0000_0001)]
        public void Given_A_MemoryAddress_Plus_Offset_It_Should_Rotate_Its_Value_To_The_Left(Reg16 register,sbyte offset, bool carrySet, byte originalValue, byte expectedValue)
        {
            var baseAddress = (ushort)0x2000;
            var memoryAddress = (ushort)(baseAddress + offset);
            var machine = CreateMachine();
            machine.Registers.Set(register, baseAddress);
            machine.Memory.Set(memoryAddress, originalValue);
            if (carrySet) machine.Flags.Set(Flag.C);

            machine.RL(Value.valueAt(register).Add(offset));

            Assert.Equal(expectedValue, machine.Memory.ReadByte(memoryAddress));
        }

        [Theory]
        [InlineData(0b0000_0000, false)]
        [InlineData(0b1111_1111, true)]
        public void After_A_Rotate_The_Carry_Bit_Should_Be_The_Original_Value_Of_Bit_7(byte originalValue, bool expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, originalValue);
            machine.RL(Reg8.A);

            if (expectedValue)
                Assert.True(machine.Flags.Read(Flag.C));
            else
                Assert.False(machine.Flags.Read(Flag.C));

        }
    }
}
