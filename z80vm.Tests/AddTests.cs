using Xunit;
using System;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class AddTests
    {
        [Theory]
        [InlineData(100, 101)]
        [InlineData(0, 1)]
        public void AddingTwo8BitsValuesShouldPutTheResultInTheFirstRegister(byte valueToAdd, byte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0x1);
            machine.ADD(Reg8.A, Read8BitValue(valueToAdd));

            Assert.Equal(expectedValue, machine.Registers.Read(Reg8.A));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Call_The_Flag_Evaluator_And_Set_All_The_Flags()
        {
            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
            var machine = new Machine(new ConditionValidator(), fe.Object);
            fe.Setup(f => f.Evalulate(machine.Flags, 0, 2)).Callback(() => 
            {
                machine.Flags.Set(Flag.C);
                machine.Flags.Set(Flag.PV);
                machine.Flags.Set(Flag.S);
                machine.Flags.Set(Flag.Z);
                callCount++;
            });

            machine.ADD(Reg8.A, Read8BitValue(2));

            Assert.True(machine.Flags.Read(Flag.C));
            Assert.True(machine.Flags.Read(Flag.PV));
            Assert.True(machine.Flags.Read(Flag.S));
            Assert.True(machine.Flags.Read(Flag.Z));
            Assert.Equal(1, callCount);
        }


        [Fact]
        public void Adding_2_8Bit_Values_Should_Call_The_Flag_Evaluator_And_Clear_All_The_Flags()
        {
            var callCount = 0;
            var fe = new Moq.Mock<IFlagsEvaluator>();
            var machine = new Machine(new ConditionValidator(), fe.Object);
            fe.Setup(f => f.Evalulate(machine.Flags, 0, 2)).Callback(() =>
            {
                machine.Flags.Clear(Flag.C);
                machine.Flags.Clear(Flag.PV);
                machine.Flags.Clear(Flag.S);
                machine.Flags.Clear(Flag.Z);
                callCount++;
            });

            machine.ADD(Reg8.A, Read8BitValue(2));

            Assert.False(machine.Flags.Read(Flag.C));
            Assert.False(machine.Flags.Read(Flag.PV));
            Assert.False(machine.Flags.Read(Flag.S));
            Assert.False(machine.Flags.Read(Flag.Z));
            Assert.Equal(1, callCount);
        }


        [Fact]
        public void Adding_2_8Bit_Values_Should_Clear_The_N_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.ADD(Reg8.A, Read8BitValue(2));

            Assert.Equal(false, machine.Flags.Read(Flag.N));
        }

        [Theory]
        [InlineData(0b1111, 0b0001)]
        [InlineData(0b1000, 0b1111)]
        public void Adding_2_8Bit_Values_Should_Set_The_H_Flag_When_There_Is_Carry_From_Bit3_To_Bit4(byte lhs, byte rhs)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, lhs);
            machine.ADD(Reg8.A, Read8BitValue(rhs));

            Assert.Equal(true, machine.Flags.Read(Flag.H));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Clear_The_H_Flag_When_There_Is_Not_A_Carry_From_Bit3_To_Bit4()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 1);
            machine.Flags.Set(Flag.H);

            machine.ADD(Reg8.A, Read8BitValue(1));

            Assert.Equal(false, machine.Flags.Read(Flag.H));
        }
        

        [Fact]
        public void AddingTwo16BitRegistersShouldPutTheResultInTheFirstRegister()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.HL, 0x1);
            machine.Registers.Set(Reg16.BC, 0x2);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(0x3, machine.Registers.Read(Reg16.HL));
        }

        [Fact]
        public void AddingTwo16BitRegistersShouldSetTheCarryFlagWhenTheResultIsOutOfRange()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.HL, 0xAAAA);
            machine.Registers.Set(Reg16.BC, 0xBBBB);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(true, machine.Flags.Read(Flag.C));
        }

        [Fact]
        public void WhenTheTotalOfTheAdditionIsOutOfRangeTheResultShouldBeReduceBy65536()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg16.HL, 0xAAAA);
            machine.Registers.Set(Reg16.BC, 0xBBBB);
            machine.ADD(Reg16.HL, Reg16.BC);

            Assert.Equal(0x6665, machine.Registers.Read(Reg16.HL));
        }

        [Theory]
        [InlineData(Reg16.AF, Reg16.BC)]        //Invalid operand1
        [InlineData(Reg16.BC, Reg16.BC)]
        [InlineData(Reg16.DE, Reg16.BC)]
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
            var machine = CreateMachine();
            var ex = Assert.Throws<InvalidOperationException>(() => machine.ADD(register1, register2));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator(), new FlagsEvaluator());
        }
    }
}
