using Xunit;
namespace z80vm.Tests
{
    public class JPTests : TestBase
    {
        [Fact]
        public void CheckThatTheProgramCounterIsChangedToTheSuppliedMemoryAddress()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(0xAAAA);

            Assert.Equal(0xAAAA, machine.Registers.Read(Reg16.PC));
        }


        [Theory]
        [InlineData(Reg16.HL)]
        public void CheckThatTheProgramCounterIsChangedToTheMemoryAddressPointedToByTheContentsOfTheRegister(Reg16 register)
        {
            var machine = CreateMachine();
            machine.Registers.Set(register, 0x2000);

            machine.JP(register);

            Assert.Equal(0x2000, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(Reg16.AF)]
        [InlineData(Reg16.BC)]
        [InlineData(Reg16.DE)]
        [InlineData(Reg16.PC)]
        [InlineData(Reg16.SP)]
        public void CheckThatOnlyLimitedRegistersAreAccepted(Reg16 register)
        {
            var machine = CreateMachine();

            var exception = Record.Exception(() => machine.JP(register));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }


        [Fact]
        public void CheckThatTheProgramCounterIsChangedToTheSuppliedMemoryAddressWhenTheConditionIsTrue()
        {
            const Condition ANY_CONDITION = Condition.c;

            var machine = CreateMachineWhereAllConditionsAreValid();

            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, 0xAAAA);

            Assert.Equal(0xAAAA, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void CheckThatTheProgramCounterIsntChangedToTheSuppliedMemoryAddressWhenTheConditionIsFalse()
        {
            const Condition ANY_CONDITION = Condition.c;

            var machine = CreateMachineWhereAllConditionsAreInvalid();
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, 0xAAAA);

            Assert.NotEqual(0xAAAA, machine.Registers.Read(Reg16.PC));
        }


    }
}