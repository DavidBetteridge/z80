using Xunit;
using static z80vm.Value;

namespace z80vm.Tests
{
    public class op8Tests : TestBase
    {
        [Theory]
        [InlineData(Reg8.A, 100, "a")]
        [InlineData(Reg8.B, 0x24, "b")]
        public void ReadAValueFromARegister(Reg8 register, byte value, string textVersion)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, value);

            var op = op8.Read8BitValue(register);
            var actualValue = op.Read(machine.Memory, machine.Registers);

            Assert.Equal(value, actualValue);
            Assert.Equal(textVersion, op.ToString());
        }

        [Theory]
        [InlineData(0x0, "n")]
        [InlineData(0xFF, "n")]
        public void ReadAnImmediateValue(byte value, string textVersion)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0xAB);

            var op = op8.Read8BitValue(value);
            var actualValue = op.Read(machine.Memory, machine.Registers);

            Assert.Equal(value, actualValue);
            Assert.Equal(textVersion, op.ToString());

        }

        [Theory]
        [InlineData(0xFFFF, 0xFF, "(nn)")]
        public void ReadAValueFromAMemoryAddress(ushort memoryAddress, byte value, string textVersion)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, value);

            var op = op8.Read8BitValue(valueAt(memoryAddress));
            var actualValue = op.Read(machine.Memory, machine.Registers);

            Assert.Equal(value, actualValue);
            Assert.Equal(textVersion, op.ToString());
        }

        [Theory]
        [InlineData(Reg16.IX, 0xFFFF, 0xFF, "(ix)")]
        [InlineData(Reg16.IY, 0x0000, 0x00, "(iy)")]
        public void ReadAValueFromAMemoryAddressPointedToByARegister(Reg16 register, ushort memoryAddress, byte value, string textVersion)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, value);
            machine.Registers.Set(register, memoryAddress);

            var op = op8.Read8BitValue(valueAt(register));
            var actualValue = op.Read(machine.Memory, machine.Registers);

            Assert.Equal(value, actualValue);
            Assert.Equal(textVersion, op.ToString());

        }

        [Theory]
        [InlineData(Reg16.IX, 0xF000, 100, 10, "(ix+n)")]
        public void ReadAValueFromAMemoryAddressPointedToByARegisterPlusAnOffset(Reg16 register, ushort memoryAddress, byte value, sbyte offset, string textVersion)
        {
            var machine = CreateMachine();
            machine.Memory.Set((ushort)(memoryAddress + offset), value);
            machine.Registers.Set(register, memoryAddress);

            var op = op8.Read8BitValue(valueAt(register).Add(offset));
            var actualValue = op.Read(machine.Memory, machine.Registers);

            Assert.Equal(value, actualValue);
            Assert.Equal(textVersion, op.ToString());
        }

        [Theory]
        [InlineData(Reg8.A, 100)]
        public void UpdateAValueInARegister(Reg8 register, byte value)
        {
            var machine = CreateMachine();

            var op = op8.Read8BitValue(register);
            op.Set(machine.Memory, machine.Registers, value);

            Assert.Equal(value, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(0xFFFF, 0xFF)]
        public void UpdateAValueInAMemoryAddress(ushort memoryAddress, byte value)
        {
            var machine = CreateMachine();

            var op = op8.Read8BitValue(valueAt(memoryAddress));
            op.Set(machine.Memory, machine.Registers, value);

            Assert.Equal(value, machine.Memory.ReadByte(memoryAddress));
        }

        [Theory]
        [InlineData(Reg16.IX, 0xFFFF, 0xFF)]
        public void UpdateAValueInAMemoryAddressPointedToByARegister(Reg16 register, ushort memoryAddress, byte value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, memoryAddress);

            var op = op8.Read8BitValue(valueAt(register));
            op.Set(machine.Memory, machine.Registers, value);

            Assert.Equal(value, machine.Memory.ReadByte(memoryAddress));
        }


        [Theory]
        [InlineData(Reg16.IX, 0xF000, 100, 10)]
        public void UpdateAValueInAMemoryAddressPointedToByARegisterPlusAnOffset(Reg16 register, ushort memoryAddress, byte value, sbyte offset)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, memoryAddress);

            var op = op8.Read8BitValue(valueAt(register).Add(offset));
            op.Set(machine.Memory, machine.Registers, value);

            Assert.Equal(value, machine.Memory.ReadByte((ushort)(memoryAddress + offset)));
        }

        [Fact]
        public void UpdatingAnImmediateValueThrowsAnException()
        {
            var machine = CreateMachine();

            var op = op8.Read8BitValue(123);

            var exception = Record.Exception(() => op.Set(machine.Memory, machine.Registers, 1));
            Assert.IsType<System.InvalidOperationException>(exception);
        }
    }
}
