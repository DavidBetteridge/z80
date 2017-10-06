using Xunit;

namespace z80vm.Tests
{
    public class LDIRTests : TestBase
    {
        [Fact]
        public void BC_Should_Be_Set_To_Zero()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, 0x1234);

            machine.LDIR();

            Assert.Equal(0, machine.Registers.Read(Reg16.BC));
        }

        [Fact]
        public void HL_is_increased_by_the_value_of_BC()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, 10);
            machine.Registers.Set(Reg16.HL, 20);

            machine.LDIR();

            Assert.Equal(30, machine.Registers.Read(Reg16.HL));
        }

        [Fact]
        public void DE_is_increased_by_the_value_of_BC()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.BC, 10);
            machine.Registers.Set(Reg16.DE, 20);

            machine.LDIR();

            Assert.Equal(30, machine.Registers.Read(Reg16.DE));
        }

        [Theory]
        [InlineData(0, 200, 201)]
        [InlineData(1, 100, 201)]
        [InlineData(2, 100, 101)]
        public void BC_number_of_memory_addresses_should_be_copied_from_HL_to_DE(ushort initialValueOfBC, byte finalValue1,  byte finalValue2)
        {
            var machine = CreateMachine();

            // Place a memory address in both registers
            machine.Registers.Set(Reg16.HL, 0x1000);
            machine.Registers.Set(Reg16.DE, 0x2000);
            machine.Registers.Set(Reg16.BC, initialValueOfBC);

            // Place a value in each of the memory addresses
            machine.Memory.Set(0x1000, 100);  //Pointed to by HL
            machine.Memory.Set(0x1001, 101);  //Pointed to by HL+1
            machine.Memory.Set(0x2000, 200);  //Pointed to by DE
            machine.Memory.Set(0x2001, 201);  //Pointed to by DE+1

            machine.LDIR();

            Assert.Equal(finalValue1, machine.Memory.ReadByte(0x2000));
            Assert.Equal(finalValue2, machine.Memory.ReadByte(0x2001));
        }

        [Theory]
        [InlineData(Flag.H)]
        [InlineData(Flag.N)]
        [InlineData(Flag.PV)]
        public void The_H_N_and_PV_Flags_Should_Be_Cleared(Flag flag)
        {
            var machine = CreateMachine();
            machine.Flags.Set(flag);
            machine.LDIR();

            Assert.Equal(false, machine.Flags.Read(flag));
        }

    }
}
