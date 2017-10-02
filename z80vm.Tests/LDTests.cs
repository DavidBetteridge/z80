using Xunit;
using static z80vm.Value;

namespace z80vm.Tests
{
    public class LDTests
    {
        [Theory]
        [InlineData(Reg8.A, Reg8.B, 123)]
        [InlineData(Reg8.C, Reg8.D, 0xFF)]
        [InlineData(Reg8.D, Reg8.IXH, 0xEE)]
        public void ItShouldBeAbleToCopyToTheContentsOfOneRegisterToAnother(Reg8 source, Reg8 target, byte value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.Registers.Set(target, 0);

            machine.LD(target, source);

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(Reg16.BC, Reg8.A, 0xCC)]
        [InlineData(Reg16.DE, Reg8.B, 0xFF)]
        public void ItShouldBeAbleToCopyTheContentsOfTheAddressPointedToByOperand2IntoOperand1(Reg16 source, Reg8 target, byte value)
        {
            var machine = CreateMachine();

            machine.Memory.Set(0xEEEE, value);
            machine.Registers.Set(source, 0xEEEE);
            machine.Registers.Set(target, 0);

            machine.LD(target, valueAt(source));

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(0xEEEE, Reg8.A, 0xCC)]
        [InlineData(0xAAAA, Reg8.B, 0xDD)]
        public void ItShouldBePossibleToLoadARegisterWithTheContentsOfAMemoryAddress(ushort memoryAddress, Reg8 target, byte value)
        {
            var machine = CreateMachine();

            machine.Memory.Set(memoryAddress, value);
            machine.LD(target, memoryAddress);

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(Reg16.IX, Reg8.A, 10, 0xCC)]
        [InlineData(Reg16.IY, Reg8.B, 127, 0xEE)]
        public void ItShouldBeAbleToCopyTheContentsOfTheAddressPointedToByOperand2PlusAnOffsetIntoOperand1(Reg16 source, Reg8 target, sbyte offset, byte value)
        {
            var machine = CreateMachine();

            machine.Registers.Set(source, 1000);
            machine.Memory.Set((ushort)(1000 + offset), value);

            //(IX + n)
            machine.LD(target, valueAt(source.Add(offset)));

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(Reg8.A, Reg16.BC, 0xCC)]
        [InlineData(Reg8.A, Reg16.DE, 0xAA)]
        [InlineData(Reg8.B, Reg16.HL, 0xBB)]
        public void ItShouldBePossibleToLoadIntoTheMemoryAddressPointedToByTheFirstOperandTheContentsOfARegister(Reg8 source, Reg16 target, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0xEEEE;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, ANY_MEMORY_LOCATION);
            machine.Registers.Set(source, value);

            machine.LD(valueAt(target), source);

            Assert.Equal(value, machine.Memory.Read(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg16.HL, 0xBB)]
        public void ItShouldBePossibleToLoadIntoTheMemoryAddressPointedToByTheFirstOperandAnImmediateValue(Reg16 target, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0xEEEE;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, ANY_MEMORY_LOCATION);

            machine.LD(valueAt(target), value);

            Assert.Equal(value, machine.Memory.Read(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg8.A, Reg16.IX, 10, 0xCC)]
        [InlineData(Reg8.B, Reg16.IY, -10, 0xFF)]
        public void ItShouldBePossibleToLoadIntoAnyOffsetedMemoryAddressPointedToByTheFirstOperandTheContentsOfARegister(Reg8 source, Reg16 target, sbyte offset, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0x1010;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, (ushort)(ANY_MEMORY_LOCATION - offset));
            machine.Registers.Set(source, value);

            machine.LD(valueAt(target.Add(offset)), source);

            Assert.Equal(value, machine.Memory.Read(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg16.IX, 10, 0xCC)]
        [InlineData(Reg16.IY, -10, 0xAA)]
        public void ItShouldBePossibleToLoadIntoAnyOffsetedMemoryAddressPointedToByTheFirstOperandAnImmediateValue(Reg16 target, sbyte offset, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0x1010;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, (ushort)(ANY_MEMORY_LOCATION - offset));

            machine.LD(valueAt(target.Add(offset)), value);

            Assert.Equal(value, machine.Memory.Read(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg8.A, 0x1000, 0xCC)]
        public void ItShouldBePossibleToLoadTheContentsOfARegisterIntoAMemoryLocation(Reg8 source, ushort memoryLocation, byte value)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryLocation, 0);
            machine.Registers.Set(source, value);

            machine.LD(memoryLocation, source);

            Assert.Equal(value, machine.Memory.Read(memoryLocation));
        }


        [Theory]
        [InlineData(Reg8.A, 100)]
        [InlineData(Reg8.B, 101)]
        [InlineData(Reg8.C, 102)]
        [InlineData(Reg8.D, 103)]
        [InlineData(Reg8.E, 104)]
        [InlineData(Reg8.H, 105)]
        [InlineData(Reg8.L, 106)]
        public void ItShouldBePossibleToLoadAn8BitRegistersWithhImmediateValue(Reg8 register, byte value)
        {
            var machine = CreateMachine();
            machine.LD(register, value);

            Assert.Equal(value, machine.Registers.Read(register));
        }

        [Fact]
        public void LoadingOneRegisterShouldNotChangeAnotherRegister()
        {
            var machine = CreateMachine();

            var currentValue = machine.Registers.Read(Reg8.B);
            machine.LD(Reg8.C, 123);

            Assert.Equal(currentValue, machine.Registers.Read(Reg8.B));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator());
        }
    }
}
