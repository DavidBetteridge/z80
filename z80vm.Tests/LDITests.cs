using Xunit;

namespace z80vm.Tests
{
    public class LDITests
    {
        [Fact]
        public void The_Contents_At_HL_Should_Be_Copied_To_The_Contents_Of_DE()
        {
            var machine = CreateMachine();

            // Place a memory address in both registers
            machine.Registers.Set(Reg16.HL, 0x1000);
            machine.Registers.Set(Reg16.DE, 0x2000);

            // Place a value in each of the memory addresses
            machine.Memory.Set(0x1000, 100);  //Pointed to by HL
            machine.Memory.Set(0x2000, 200);  //Pointed to by DE

            machine.LDI();

            Assert.Equal(100, machine.Memory.ReadByte(0x2000));
        }

        [Fact]
        public void HL_Should_Advance_To_The_Next_Byte()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.HL, 0x1000);

            machine.LDI();

            Assert.Equal(0x1001, machine.Registers.Read(Reg16.HL));
        }

        [Fact]
        public void DE_Should_Advance_To_The_Next_Byte()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.DE, 0x1000);

            machine.LDI();

            Assert.Equal(0x1001, machine.Registers.Read(Reg16.DE));
        }

        [Theory]
        [InlineData(0x1005, 0x1004)]
        [InlineData(0x0, 0xFFFF)]  //boundary - overflow
        public void BC_Should_Be_Decreased(ushort from, ushort expected)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, from);

            machine.LDI();

            Assert.Equal(expected, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void The_PV_Flag_Should_Be_Set_If_Decreasing_BC_Causes_An_Overflow()
        {
            var machine = CreateMachine();
            machine.Flags.Clear(Flag.PV);
            machine.LDI();

            Assert.Equal(true, machine.Flags.Read(Flag.PV));
        }

        [Fact]
        public void The_PV_Flag_Should_Be_Clear_If_Decreasing_BC_Doesnt_Cause_An_Overflow()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, 1);
            machine.Flags.Set(Flag.PV);
            machine.LDI();

            Assert.Equal(false, machine.Flags.Read(Flag.PV));
        }

        [Fact]
        public void The_H_Flag_Should_Be_Cleared()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.H);
            machine.LDI();

            Assert.Equal(false, machine.Flags.Read(Flag.H));
        }

        [Fact]
        public void The_N_Flag_Should_Be_Cleared()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.LDI();

            Assert.Equal(false, machine.Flags.Read(Flag.N));
        }

        private static Machine CreateMachine()
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            return new Machine(conditionValidator.Object, new FlagsEvaluator());
        }
    }
}
