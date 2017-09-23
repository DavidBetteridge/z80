using Xunit;

namespace z80vm.Tests
{
    public class RegisterTests
    {
        [Theory]
        [InlineData(Reg16.AF, Reg8.A)]
        [InlineData(Reg16.BC, Reg8.B)]
        [InlineData(Reg16.DE, Reg8.D)]
        [InlineData(Reg16.HL, Reg8.H)]
        public void SettingA16BitRegisterSetsTheHighValueIntoTheFirstRegister(Reg16 register, Reg8 highRegister)
        {
            ushort A_VALUE = 0xAABB;

            var machine = new Machine();
            machine.Registers.Set(register, A_VALUE);

            Assert.Equal(0xAA, machine.Registers.Read(highRegister));
        }

        [Theory]
        [InlineData(Reg16.AF, Reg8.F)]
        [InlineData(Reg16.BC, Reg8.C)]
        [InlineData(Reg16.DE, Reg8.E)]
        [InlineData(Reg16.HL, Reg8.L)]
        public void SettingA16BitRegisterSetsTheLowValueIntoTheSecondRegister(Reg16 register, Reg8 lowRegister)
        {
            ushort A_VALUE = 0xAABB;

            var machine = new Machine();
            machine.Registers.Set(register, A_VALUE);

            Assert.Equal(0xBB, machine.Registers.Read(lowRegister));
        }

        [Theory]
        [InlineData(Reg16.AF, Reg8.A, Reg8.F)]
        [InlineData(Reg16.BC, Reg8.B, Reg8.C)]
        [InlineData(Reg16.DE, Reg8.D, Reg8.E)]
        [InlineData(Reg16.HL, Reg8.H, Reg8.L)]
        public void ReadingA16BitRegisterCombinesItsHighAndLowOrderBytes(Reg16 register, Reg8 highRegister, Reg8 lowRegister)
        {
            var machine = new Machine();
            machine.Registers.Set(highRegister, 0xAB);
            machine.Registers.Set(lowRegister, 0xCD);

            Assert.Equal(0xABCD, machine.Registers.Read(register));
        }
    }
}
