using Xunit;

namespace z80vm.Tests
{
    public class LDDRTests
    {
        [Fact]
        public void BC_Should_Be_Set_To_Zero()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.DE, 0xFFFF);
            machine.Registers.Set(Reg16.HL, 0xFFFF);

            machine.Registers.Set(Reg16.BC, 0x1234);

            machine.LDDR();

            Assert.Equal(0, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void HL_is_decreased_by_the_value_of_BC()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.DE, 0xFFFF);
            machine.Registers.Set(Reg16.BC, 10);
            machine.Registers.Set(Reg16.HL, 30);

            machine.LDDR();

            Assert.Equal(20, machine.Registers.Read(Reg16.HL));
        }

        [Fact]
        public void DE_is_decreased_by_the_value_of_BC()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, 10);
            machine.Registers.Set(Reg16.DE, 30);
            machine.Registers.Set(Reg16.HL, 0xFFFF);

            machine.LDDR();

            Assert.Equal(20, machine.Registers.Read(Reg16.DE));
        }

        [Theory]
        [InlineData(0, 201, 200)]
        [InlineData(1, 101, 200)]
        [InlineData(2, 101, 100)]
        public void BC_number_of_memory_addresses_below_HL_to_copy_below_DE(ushort initialValueOfBC, byte finalValue1,  byte finalValue2)
        {
            var machine = CreateMachine();

            // Place a memory address in both registers
            machine.Registers.Set(Reg16.HL, 0x1001);
            machine.Registers.Set(Reg16.DE, 0x2001);
            machine.Registers.Set(Reg16.BC, initialValueOfBC);

            // Place a value in each of the memory addresses
            machine.Memory.Set(0x1001, 101);  //Pointed to by HL
            machine.Memory.Set(0x1000, 100);  //Pointed to by HL-1
            machine.Memory.Set(0x2001, 201);  //Pointed to by DE
            machine.Memory.Set(0x2000, 200);  //Pointed to by DE-1

            machine.LDDR();

            Assert.Equal(finalValue1, machine.Memory.Read(0x2001));
            Assert.Equal(finalValue2, machine.Memory.Read(0x2000));
        }

        [Theory]
        [InlineData(Flag.H)]
        [InlineData(Flag.N)]
        [InlineData(Flag.PV)]
        public void The_H_N_and_PV_Flags_Should_Be_Cleared(Flag flag)
        {
            var machine = CreateMachine();
            machine.Flags.Set(flag);
            machine.LDDR();

            Assert.Equal(false, machine.Flags.Read(flag));
        }

        private static Machine CreateMachine()
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            return new Machine(conditionValidator.Object);
        }
    }
}
