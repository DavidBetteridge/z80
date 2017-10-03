﻿using Xunit;
using System;
using static z80vm.Value;
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
        public void Adding_2_8Bit_Values_Should_Clear_The_N_Flag()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);
            machine.ADD(Reg8.A, Read8BitValue(2));

            Assert.Equal(false, machine.Flags.Read(Flag.N));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Set_The_S_Flag_When_The_Accumlator_Changes_From_Postive_To_Negative()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 127);
            machine.ADD(Reg8.A, Read8BitValue(1));

            Assert.Equal(true, machine.Flags.Read(Flag.S));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Clear_The_S_Flag_When_The_Accumlator_Doesnt_Changes_From_Postive_To_Negative()
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.S);
            machine.Registers.Set(Reg8.A, 100);
            machine.ADD(Reg8.A, Read8BitValue(1));

            Assert.Equal(false, machine.Flags.Read(Flag.S));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Set_The_Z_Flag_When_The_Total_Is_Zero()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0);
            machine.ADD(Reg8.A, Read8BitValue(0L));

            Assert.Equal(true, machine.Flags.Read(Flag.Z));
        }

        [Fact]
        public void Adding_2_8Bit_Values_Should_Clear_The_Z_Flag_When_The_Total_Is_Not_Zero()
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, 0);
            machine.Flags.Set(Flag.Z);

            machine.ADD(Reg8.A, Read8BitValue(1));

            Assert.Equal(false, machine.Flags.Read(Flag.Z));
        }

        [Theory]
        [InlineData(0b1111,0b0001)]
        [InlineData(0b1000,0b1111)]
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


        [Theory]
        [InlineData(127, 1)]
        [InlineData(255, 1)]
        public void Adding_2_8Bit_Values_Should_Set_The_PV_Flag_When_There_Is_An_Overflow(byte lhs, byte rhs)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, lhs);
            machine.ADD(Reg8.A, Read8BitValue(rhs));

            Assert.Equal(true, machine.Flags.Read(Flag.PV));
        }

        [Theory]
        [InlineData(120, 1)]
        public void Adding_2_8Bit_Values_Should_Not_Set_The_PV_Flag_When_There_Is_Not_An_Overflow(byte lhs, byte rhs)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, lhs);
            machine.Flags.Set(Flag.PV);

            machine.ADD(Reg8.A, Read8BitValue(rhs));

            Assert.Equal(false, machine.Flags.Read(Flag.PV));
        }

        [Theory]
        [InlineData(0b1111_1111, 2)]
        public void Adding_2_8Bit_Values_Should_Set_The_C_Flag_When_The_Register_Steps_Over_Zero(byte lhs, byte rhs)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, lhs);
            machine.ADD(Reg8.A, Read8BitValue(rhs));

            Assert.Equal(true, machine.Flags.Read(Flag.C));
        }

        [Theory]
        [InlineData(1, 1)]
        public void Adding_2_8Bit_Values_Should_Clear_The_C_Flag_When_The_Register_DoesNot_Step_Over_Zero(byte lhs, byte rhs)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, lhs);
            machine.Flags.Set(Flag.C);

            machine.ADD(Reg8.A, Read8BitValue(rhs));

            Assert.Equal(false, machine.Flags.Read(Flag.C));
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
            return new Machine(new ConditionValidator());
        }
    }
}
