using System;
using Xunit;

namespace z80vm.Tests
{
    public class RetTests
    {
        [Fact]
        public void TheAddressOfNextInstructionShouldBeSetToTheWordOnTheTopOfStack()
        {
            var machine = CreateMachine();

            // Put any address on the stack
            machine.Registers.Set(Reg16.BC, 0xABCD);
            machine.PUSH(Reg16.BC);

            machine.RET();

            Assert.Equal(0xABCD, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void TheStackPointerShouldBeIncreasedBy2()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.SP, 0x1000);

            machine.RET();

            Assert.Equal(0x1002, machine.Registers.Read(Reg16.SP));
        }

        [Fact]
        public void TheRETCommandShouldOnlyBeCarriedOutIfTheConditionIsTrue()
        {
            //Arrange
            const Condition ANY_CONDITION = Condition.c;
            const ushort ANY_ADDRESS = 0xABCD;

            var conditionValidator = new FakeAlwaysTrueConditionValidator();
            var machine = new Machine(conditionValidator);
            machine.Registers.Set(Reg16.BC, ANY_ADDRESS);
            machine.PUSH(Reg16.BC);

            //Act
            machine.RET(ANY_CONDITION);

            //Assert
            Assert.Equal(ANY_ADDRESS, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void TheRETCommandShouldNotBeCarriedOutIfTheConditionIsFalse()
        {
            //Arrange
            const Condition ANY_CONDITION = Condition.c;
            const ushort ANY_ADDRESS = 0xABCD;
            const ushort ANY_OTHER_ADDRESS = 0xEEEE;

            var conditionValidator = new FakeAlwaysFalseConditionValidator();
            var machine = new Machine(conditionValidator);
            machine.Registers.Set(Reg16.BC, ANY_ADDRESS);
            machine.Registers.Set(Reg16.PC, ANY_OTHER_ADDRESS);
            machine.PUSH(Reg16.BC);

            //Act
            machine.RET(ANY_CONDITION);

            //Assert
            Assert.Equal(ANY_OTHER_ADDRESS, machine.Registers.Read(Reg16.PC));
        }

        private class FakeAlwaysTrueConditionValidator : IConditionValidator
        {
            public bool IsTrue(Flags flags, Condition condition)
            {
                return true;
            }
        }

        private class FakeAlwaysFalseConditionValidator : IConditionValidator
        {
            public bool IsTrue(Flags flags, Condition condition)
            {
                return false;
            }
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator());
        }
    }
}
