using Xunit;

namespace z80vm.Tests
{
    public class RegisterTests : TestBase
    {
        [Theory]
        [InlineData(Reg16.AF, Reg8.A)]
        [InlineData(Reg16.BC, Reg8.B)]
        [InlineData(Reg16.DE, Reg8.D)]
        [InlineData(Reg16.HL, Reg8.H)]
        public void SettingA16BitRegisterSetsTheHighValueIntoTheFirstRegister(Reg16 register, Reg8 highRegister)
        {
            ushort A_VALUE = 0xAABB;

            var machine = CreateMachine();
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

            var machine = CreateMachine();
            machine.Registers.Set(register, A_VALUE);

            Assert.Equal(0xBB, machine.Registers.Read(lowRegister));
        }

        [Theory]
        [InlineData(Reg16.AF, Reg8.A, Reg8.F)]
        [InlineData(Reg16.BC, Reg8.B, Reg8.C)]
        [InlineData(Reg16.DE, Reg8.D, Reg8.E)]
        [InlineData(Reg16.HL, Reg8.H, Reg8.L)]
        [InlineData(Reg16.IX, Reg8.IXH, Reg8.IXL)]
        [InlineData(Reg16.IY, Reg8.IYH, Reg8.IYL)]
        public void ReadingA16BitRegisterCombinesItsHighAndLowOrderBytes(Reg16 register, Reg8 highRegister, Reg8 lowRegister)
        {
            var machine = CreateMachine();
            machine.Registers.Set(highRegister, 0xAB);
            machine.Registers.Set(lowRegister, 0xCD);

            Assert.Equal(0xABCD, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg8.A)]
        [InlineData(Reg8.B)]
        [InlineData(Reg8.C)]
        [InlineData(Reg8.D)]
        [InlineData(Reg8.E)]
        [InlineData(Reg8.F)]
        [InlineData(Reg8.H)]
        [InlineData(Reg8.I)]
        [InlineData(Reg8.L)]
        [InlineData(Reg8.R)]
        [InlineData(Reg8.IXH)]
        [InlineData(Reg8.IXL)]
        [InlineData(Reg8.IYH)]
        [InlineData(Reg8.IYL)]
        public void IsShouldBePossibleToWriteAndReadFromEvery8BitRegister(Reg8 register)
        {
            var ANY_VALUE = (byte)0xAB;

            var machine = CreateMachine();
            machine.Registers.Set(register, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.AF)]
        [InlineData(Reg16.BC)]
        [InlineData(Reg16.DE)]
        [InlineData(Reg16.HL)]
        [InlineData(Reg16.IX)]
        [InlineData(Reg16.IY)]
        [InlineData(Reg16.PC)]
        [InlineData(Reg16.SP)]
        public void IsShouldBePossibleToWriteAndReadFromEvery16BitRegister(Reg16 register)
        {
            var ANY_VALUE = (ushort)0xABCD;

            var machine = CreateMachine();
            machine.Registers.Set(register, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16Shadow.AF)]
        [InlineData(Reg16Shadow.BC)]
        [InlineData(Reg16Shadow.DE)]
        [InlineData(Reg16Shadow.HL)]
        public void IsShouldBePossibleToWriteAndReadFromEvery16BitShadowRegister(Reg16Shadow register)
        {
            var ANY_VALUE = (ushort)0xABCD;

            var machine = CreateMachine();
            machine.Registers.Set(register, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Registers.Read(register));
        }

        [Fact]
        public void SettingAShadowRegisterShouldNotChangeTheNormalRegister()
        {
            var ANY_VALUE = (ushort)0xABCD;
            var A_DIFFERENT_VALUE = (ushort)0xAAAA;

            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, ANY_VALUE);
            machine.Registers.Set(Reg16Shadow.BC, A_DIFFERENT_VALUE);

            Assert.Equal(ANY_VALUE, machine.Registers.Read(Reg16.BC));
        }

        [Theory]
        [InlineData(Reg8Shadow.A)]
        [InlineData(Reg8Shadow.B)]
        [InlineData(Reg8Shadow.C)]
        [InlineData(Reg8Shadow.D)]
        [InlineData(Reg8Shadow.E)]
        [InlineData(Reg8Shadow.F)]
        [InlineData(Reg8Shadow.H)]
        [InlineData(Reg8Shadow.L)]
        public void IsShouldBePossibleToWriteAndReadFromEvery8BitShadowRegister(Reg8Shadow register)
        {
            var ANY_VALUE = (byte)0xAB;

            var machine = CreateMachine();
            machine.Registers.Set(register, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Registers.Read(register));
        }
    }
}
