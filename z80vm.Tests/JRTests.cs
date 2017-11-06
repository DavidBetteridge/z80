using System;
using Moq;
using Xunit;

namespace z80vm.Tests
{
    public class JRTests : TestBase
    {
        [Theory]
        [InlineData(100, 1100)]
        [InlineData(-128, 872)]
        [InlineData(0, 1000)]
        [InlineData(127, 1127)]
        public void TheProgramCounterShouldBeAdvancedByTheSuppliedValue(sbyte offset, ushort newAddress)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(offset);

            Assert.Equal(newAddress, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(0xFFFF)]
        [InlineData(0x0)]
        public void AnErrorShouldNotBeReportedIfTheInstructionJumpsOutOfTheMemorySpace(ushort start)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, start);

            Assert.True(true);
        }

        [Fact]
        public void TheJumpShouldHappenWhenTheConditionIsTrue()
        {
            var machine = CreateMachineWhereAllConditionsAreValid();
            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(Condition.c, 1);

            Assert.Equal(1001, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(Condition.c)]
        [InlineData(Condition.nc)]
        [InlineData(Condition.z)]
        [InlineData(Condition.nz)]
        public void TheJumpShouldNotHappenWhenTheConditionIsFalse(Condition condition)
        {
            var machine = CreateMachineWhereAllConditionsAreInvalid();

            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(condition, 1);

            Assert.NotEqual(1001, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(Condition.m)]
        [InlineData(Condition.p)]
        [InlineData(Condition.pe)]
        [InlineData(Condition.po)]
        public void AnErrorShouldBeReportedIfAnUnsupportedConditionIsSupplied(Condition condition)
        {
            var machine = CreateMachine();

            var exception = Record.Exception(() => machine.JR(condition, 1));
            Assert.IsType<System.InvalidOperationException>(exception);
        }

    }
}
