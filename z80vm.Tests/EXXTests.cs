using Xunit;

namespace z80vm.Tests
{
    public class EXXTests : TestBase
    {
        [Theory]
        [InlineData(Reg16.BC, Reg16Shadow.BC)]
        [InlineData(Reg16.DE, Reg16Shadow.DE)]
        [InlineData(Reg16.HL, Reg16Shadow.HL)]
        public void EXXShouldSwapARegisterWithItsShadow(Reg16 register, Reg16Shadow shadow)
        {
            var machine = CreateMachine();

            machine.Registers.Set(register, 0xAAAA);
            machine.Registers.Set(shadow, 0xBBBB);
            machine.EXX();

            Assert.Equal(0xAAAA, machine.Registers.Read(shadow));
            Assert.Equal(0xBBBB, machine.Registers.Read(register));
        }
        
    }
}
