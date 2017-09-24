using Xunit;

namespace z80vm.Tests
{
    public class FlagTests
    {
        [Theory]
        [InlineData(Flag.C)]
        [InlineData(Flag.N)]
        [InlineData(Flag.PV)]
        [InlineData(Flag.Undefined3)]
        [InlineData(Flag.H)]
        [InlineData(Flag.Undefined5)]
        [InlineData(Flag.Z)]
        [InlineData(Flag.S)]
        public void IsShouldBePossibleToSetAndReadEveryFlag(Flag flag)
        {
            var machine = new Machine();

            machine.Flags.Set(flag);

            Assert.True(machine.Flags.Read(flag));
        }

        [Theory]
        [InlineData(Flag.C)]
        [InlineData(Flag.N)]
        [InlineData(Flag.PV)]
        [InlineData(Flag.Undefined3)]
        [InlineData(Flag.H)]
        [InlineData(Flag.Undefined5)]
        [InlineData(Flag.Z)]
        [InlineData(Flag.S)]
        public void IsShouldBePossibleToClearAndReadEveryFlag(Flag flag)
        {
            var machine = new Machine();

            machine.Flags.Set(flag);
            machine.Flags.Clear(flag);

            Assert.False(machine.Flags.Read(flag));
        }

        [Fact]
        public void SettingOneFlagShouldNotChangeAnother()
        {
            var machine = new Machine();

            machine.Flags.Set(Flag.C);

            Assert.Equal(false, machine.Flags.Read(Flag.H));
        }

        [Theory]
        [InlineData(Flag.C, 0b0000_0001)]
        [InlineData(Flag.N, 0b0000_0010)]
        [InlineData(Flag.PV, 0b0000_0100)]
        [InlineData(Flag.Undefined3, 0b0000_1000)]
        [InlineData(Flag.H, 0b0001_0000)]
        [InlineData(Flag.Undefined5, 0b0010_0000)]
        [InlineData(Flag.Z, 0b0100_0000)]
        [InlineData(Flag.S, 0b1000_0000)]
        public void SettingAFlagShouldUpdateTheARegister(Flag flag, byte valueOfAregister)
        {
            var machine = new Machine();

            machine.Flags.Set(flag);

            Assert.Equal(valueOfAregister, machine.Registers.Read(Reg8.A));
        }

        [Theory]
        [InlineData(Flag.C, Flag.PV, 0b0000_0101)]
        public void SettingTwoFlagShouldUpdateTheARegister(Flag flag1, Flag flag2, byte valueOfAregister)
        {
            var machine = new Machine();

            machine.Flags.Set(flag1);
            machine.Flags.Set(flag2);

            Assert.Equal(valueOfAregister, machine.Registers.Read(Reg8.A));
        }

        [Theory]
        [InlineData(Flag.C, 0b0000_0001)]
        [InlineData(Flag.N, 0b0000_0010)]
        [InlineData(Flag.PV, 0b0000_0100)]
        [InlineData(Flag.Undefined3, 0b0000_1000)]
        [InlineData(Flag.H, 0b0001_0000)]
        [InlineData(Flag.Undefined5, 0b0010_0000)]
        [InlineData(Flag.Z, 0b0100_0000)]
        [InlineData(Flag.S, 0b1000_0000)]
        public void SettingTheARegisterShouldUpdateTheFlags(Flag flag, byte valueOfAregister)
        {
            var machine = new Machine();
            machine.Registers.Set(Reg8.A, valueOfAregister);
            
            Assert.Equal(true, machine.Flags.Read(flag));
        }
    }
}
