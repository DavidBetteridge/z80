using Xunit;

namespace z80vm.Tests
{
    public class CallTests
    {
        [Fact]
        public void The_Address_Of_The_Instruction_Immediately_Following_The_Call_Is_Saved_To_The_Stack()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;

            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_MEMORY_ADDRESS);

            machine.POP(Reg16.BC);
            Assert.Equal(0x1003, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void Execution_Is_Continued_From_The_Address_Given_By_The_Label()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;

            var machine = CreateMachine();
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_MEMORY_ADDRESS);

            Assert.Equal(ANY_MEMORY_ADDRESS, machine.Registers.Read(Reg16.PC));
        }

        [Fact]
        public void A_Label_Can_Be_Supplied_Instead_Of_An_Address()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;
            const string ANY_LABEL = "Subroutine";

            var machine = CreateMachine();
            machine.Labels.Set(ANY_LABEL, ANY_MEMORY_ADDRESS);
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_LABEL);

            machine.POP(Reg16.BC);
            Assert.Equal(0x1003, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void The_Call_Should_Happen_If_The_Condition_Is_True()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;
            const string ANY_LABEL = "Subroutine";
            const Condition ANY_CONDITION = Condition.c;

            var machine = new Machine(new FakeAlwaysTrueConditionValidator());
            machine.Labels.Set(ANY_LABEL, ANY_MEMORY_ADDRESS);
            machine.Registers.Set(Reg16.PC, 0x1000);

            machine.CALL(ANY_LABEL, ANY_CONDITION);

            machine.POP(Reg16.BC);
            Assert.Equal(0x1003, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void Is_Should_Be_Possible_To_Supply_A_Memory_Address_With_A_Condition()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;

            var machine = CreateMachine();
            machine.Registers.Set(Reg16.SP, 0xF000); //Need to lower the SP so we can POP without first PUSHing
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.Flags.Clear(Flag.C);

            machine.CALL(ANY_MEMORY_ADDRESS, Condition.c);

            machine.POP(Reg16.BC);
            Assert.NotEqual(0x1003, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void The_Call_Should_Not_Happen_If_The_Condition_Is_False()
        {
            const ushort ANY_MEMORY_ADDRESS = 0x0000;
            const string ANY_LABEL = "Subroutine";

            var machine = new Machine(new FakeAlwaysFalseConditionValidator());
            machine.Registers.Set(Reg16.SP, 0xF000); //Need to lower the SP so we can POP without first PUSHing
            machine.Labels.Set(ANY_LABEL, ANY_MEMORY_ADDRESS);
            machine.Registers.Set(Reg16.PC, 0x1000);
            machine.Flags.Clear(Flag.C);

            machine.CALL(ANY_LABEL, Condition.c);

            machine.POP(Reg16.BC);
            Assert.NotEqual(0x1003, machine.Registers.Read(Reg16.BC));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator());
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
    }
}

