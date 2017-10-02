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
