using Xunit;
using System;

namespace z80vm.Tests
{
    public class AddTests
    {
        [Fact]
        public void AddingTwo16BitRegistersShouldPutTheResultInTheFirstRegister()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.HL, 0x1);
            machine.Registers.Set(Reg16.BC, 0x2);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(0x3, machine.Registers.Read(Reg16.HL));
        }

        [Fact]
        public void AddingTwo16BitRegistersShouldSetTheCarryFlagWhenTheResultIsOutOfRange()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.HL, 0xAAAA);
            machine.Registers.Set(Reg16.BC, 0xBBBB);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(true, machine.Flags.Read(Flag.C));
        }

        [Fact]
        public void WhenTheTotalOfTheAdditionIsOutOfRangeTheResultShouldBeReduceBy65536()
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.HL, 0xAAAA);
            machine.Registers.Set(Reg16.BC, 0xBBBB);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(0x6665, machine.Registers.Read(Reg16.HL));
        }

        [Theory]
        [InlineData(Reg16.AF, Reg16.BC)]        //Invalid operand1
        [InlineData(Reg16.BC, Reg16.BC)]
        [InlineData(Reg16.DE, Reg16.BC)]
        [InlineData(Reg16.AF2, Reg16.BC)]
        [InlineData(Reg16.BC2, Reg16.BC)]
        [InlineData(Reg16.DE2, Reg16.BC)]
        [InlineData(Reg16.PC, Reg16.BC)]
        [InlineData(Reg16.SP, Reg16.BC)]

        [InlineData(Reg16.HL, Reg16.AF)]        //Invalid operand2
        [InlineData(Reg16.HL, Reg16.PC)]

        [InlineData(Reg16.HL, Reg16.IX)]        //Invalid combination
        [InlineData(Reg16.HL, Reg16.IY)]
        [InlineData(Reg16.IX, Reg16.HL)]
        [InlineData(Reg16.IX, Reg16.IY)]
        [InlineData(Reg16.IY, Reg16.HL)]
        [InlineData(Reg16.IY, Reg16.IX)]
        public void AnErrorShouldBeThrownIfAnInvalidCombinationOf16BitRegistersAreSupplied(Reg16 register1, Reg16 register2)
        {
            var machine = new Machine();
            var ex = Assert.Throws<InvalidOperationException>(() => machine.ADD(register1, register2));
        }
    }
}
