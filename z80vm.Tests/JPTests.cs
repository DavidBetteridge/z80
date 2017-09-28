using Moq;
using Xunit;
using static z80vm.Value;
namespace z80vm.Tests
{
    public class JPTests
    {
        //        JP and JR
        //Syntax: jp/jr label or(hl)/(ix)/(iy) (unconditional) or jp/jr condition, label (conditional)
        //When arriving at any of these intructions, execution is immediately continued from the location denoted by the label given
        //(you can use numbers instead of labels of course). If the operand is a register reference(e.g.jp (hl)), 
        //it means that the value of the register will be loaded into PC directly; these jumps can be unconditional only.
        //Otherwise, if the condition is not fulfilled, execution continues as if there wasn’t any jump.The flags are preserved in all the cases.The conditions can be the following:


        [Fact]
        public void CheckThatTheProgramCounterIsChangedToTheSuppliedMemoryAddress()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(0xAAAA);

            Assert.Equal(0xAAAA, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void CheckThatTheProgramCounterIsChangedToTheMemoryAddressPointedToByTheLabel()
        {
            var machine = CreateMachine();
            machine.Labels.Set("ROUTINE", 0xBBBB);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP("ROUTINE");

            Assert.Equal(0xBBBB, machine.Registers.Read(Reg16.PC));
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

            var alwaysTrue = new Mock<IConditionValidator>();
            alwaysTrue.Setup(v => v.IsTrue(It.IsAny<Flags>(), ANY_CONDITION)).Returns(true);

            var machine = new Machine(alwaysTrue.Object);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, 0xAAAA);

            Assert.Equal(0xAAAA, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void CheckThatTheProgramCounterIsntChangedToTheSuppliedMemoryAddressWhenTheConditionIsFalse()
        {
            const Condition ANY_CONDITION = Condition.c;

            var alwaysFalse = new Mock<IConditionValidator>();
            alwaysFalse.Setup(v => v.IsTrue(It.IsAny<Flags>(), ANY_CONDITION)).Returns(false);

            var machine = new Machine(alwaysFalse.Object);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, 0xAAAA);

            Assert.NotEqual(0xAAAA, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void CheckThatTheProgramCounterIsChangedToTheMemoryAddressPointedToByTheLabelWhenTheConditionIsTrue()
        {
            const Condition ANY_CONDITION = Condition.c;

            var alwaysTrue = new Mock<IConditionValidator>();
            alwaysTrue.Setup(v => v.IsTrue(It.IsAny<Flags>(), ANY_CONDITION)).Returns(true);

            var machine = new Machine(alwaysTrue.Object);

            machine.Labels.Set("ROUTINE", 0xBBBB);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, "ROUTINE");

            Assert.Equal(0xBBBB, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void CheckThatTheProgramCounterIsntChangedToTheMemoryAddressPointedToByTheLabelWhenTheConditionIsFalse()
        {
            const Condition ANY_CONDITION = Condition.c;

            var alwaysFalse = new Mock<IConditionValidator>();
            alwaysFalse.Setup(v => v.IsTrue(It.IsAny<Flags>(), ANY_CONDITION)).Returns(false);

            var machine = new Machine(alwaysFalse.Object);

            machine.Labels.Set("ROUTINE", 0xBBBB);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.JP(ANY_CONDITION, "ROUTINE");

            Assert.NotEqual(0xBBBB, machine.Registers.Read(Reg16.PC));
        }

        private Machine CreateMachine() => new Machine(new ConditionValidator());
    }
}