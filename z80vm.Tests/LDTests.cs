using System;
using Xunit;
using static z80vm.Value;
using static z80vm.op8;

namespace z80vm.Tests
{
    public class LDTests : TestBase
    {
        //*************************************  8-BIT COMBINATIONS ************************************************

        [Theory]
        [InlineData(Reg8.A)]
        [InlineData(Reg8.B)]
        [InlineData(Reg8.C)]
        [InlineData(Reg8.D)]
        [InlineData(Reg8.E)]
        [InlineData(Reg8.H)]
        [InlineData(Reg8.L)]
        public void Load_An_Eight_Bit_Register(Reg8 register)
        {
            var machine = CreateMachine();
            machine.LD(register, Read8BitValue(100));
            
            Assert.Equal(100, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.HL, 0xBB)]
        public void Load_A_Memory_Address_Specified_By_A_Register(Reg16 target, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0xEEEE;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, ANY_MEMORY_LOCATION);

            machine.LD(valueAt(target), Read8BitValue(value));

            Assert.Equal(value, machine.Memory.ReadByte(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg16.IX, 10, 0xCC)]
        [InlineData(Reg16.IY, -10, 0xAA)]
        public void Load_A_Memory_Address_Specified_By_A_Register_Plus_An_Offset(Reg16 target, sbyte offset, byte value)
        {
            const ushort ANY_MEMORY_LOCATION = 0x1010;
            var machine = CreateMachine();
            machine.Memory.Set(ANY_MEMORY_LOCATION, 0);
            machine.Registers.Set(target, (ushort)(ANY_MEMORY_LOCATION - offset));

            machine.LD(valueAt(target).Add(offset), Read8BitValue(value));

            Assert.Equal(value, machine.Memory.ReadByte(ANY_MEMORY_LOCATION));
        }

        [Theory]
        [InlineData(Reg8.A, 0x1000, 0xCC)]
        public void Load_Directly_Into_A_Memory_Location(Reg8 source, ushort memoryLocation, byte value)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryLocation, 0);
            machine.Registers.Set(source, value);

            machine.LD(memoryLocation, Read8BitValue(source));

            Assert.Equal(value, machine.Memory.ReadByte(memoryLocation));
        }

        [Theory]
        [InlineData(Reg8.I)]
        [InlineData(Reg8.R)]
        public void Loading_The_Contents_Of_I_or_R_Should_Reset_The_H_Flag(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.H);

            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.False(machine.Flags.Read(Flag.H));
        }

        [Theory]
        [InlineData(Reg8.A)]
        [InlineData(Reg8.L)]
        public void Loading_The_Contents_Of_Anything_Other_Than_I_or_R_Should_Preserve_The_H_Flag(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.H);

            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.H));
        }

        [Theory]
        [InlineData(Reg8.I)]
        [InlineData(Reg8.R)]
        public void Loading_The_Contents_Of_I_or_R_Should_Reset_The_N_Flag(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);

            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.False(machine.Flags.Read(Flag.N));
        }

        [Theory]
        [InlineData(Reg8.A)]
        [InlineData(Reg8.L)]
        public void Loading_The_Contents_Of_Anything_Other_Than_I_or_R_Should_Preserve_The_N_Flag(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.N);

            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.N));
        }

        [Theory]
        [InlineData(Reg8.I)]
        [InlineData(Reg8.R)]
        public void When_I_or_R_Contains_Zero_And_Are_Loaded_Into_A_Register_Then_Z_Should_Be_Set(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, 0);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.Z));
        }

        [Theory]
        [InlineData(Reg8.I)]
        [InlineData(Reg8.R)]
        public void When_I_or_R_Dont_Contain_Zero_And_Are_Loaded_Into_A_Register_Then_Z_Should_Be_Cleared(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.Z);
            machine.Registers.Set(source, 100);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.False(machine.Flags.Read(Flag.Z));
        }

        [Theory]
        [InlineData(Reg8.A)]
        [InlineData(Reg8.B)]
        public void When_A_Source_Other_Then_I_or_R_Contains_Zero_And_Are_Loaded_Into_A_Register_Then_Z_Should_Be_Preserved(Reg8 source)
        {
            var machine = CreateMachine();
            machine.Flags.Set(Flag.Z);

            machine.Registers.Set(source, 10);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.Z));
        }

        [Theory]
        [InlineData(Reg8.I, 10)]
        [InlineData(Reg8.R, 0)]
        public void When_I_or_R_Contains_A_Positive_Value_And_Are_Loaded_Into_A_Register_Then_S_Should_Be_Set(Reg8 source, byte value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.S));
        }

        [Theory]
        [InlineData(Reg8.I, 0b1111_1111)]
        [InlineData(Reg8.R, 0b1000_0000)]
        public void When_I_or_R_Contains_A_Negative_Value_And_Are_Loaded_Into_A_Register_Then_S_Should_Be_Cleared(Reg8 source, byte value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.Flags.Set(Flag.S);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.False(machine.Flags.Read(Flag.S));
        }

        [Theory]
        [InlineData(Reg8.A, 0b1111_1111)]
        [InlineData(Reg8.B, 0b1000_0000)]
        public void When_A_Register_Other_Than_I_or_R_Contains_A_Negative_Value_And_Are_Loaded_Into_A_Register_Then_S_Should_Be_Preserved(Reg8 source, byte value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.Flags.Set(Flag.S);
            machine.LD(Reg8.A, Read8BitValue(source));

            Assert.True(machine.Flags.Read(Flag.S));
        }

        //*************************************  16-BIT COMBINATIONS ************************************************
        [Theory]
        [InlineData(Reg16.BC, 0x1000)]
        [InlineData(Reg16.DE, 0x1001)]
        [InlineData(Reg16.HL, 0x1002)]
        [InlineData(Reg16.IX, 0x1003)]
        [InlineData(Reg16.IY, 0x1004)]
        [InlineData(Reg16.SP, 0x1005)]
        public void Load_16BIT_Register_With_An_Immediate_Value(Reg16 target, ushort value)
        {
            var machine = CreateMachine();
            machine.LD(target, value);

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(Reg16.BC, 0x1000, 0x20)]
        [InlineData(Reg16.DE, 0x1001, 0x21)]
        [InlineData(Reg16.HL, 0x1002, 0x22)]
        [InlineData(Reg16.IX, 0x1003, 0x23)]
        [InlineData(Reg16.IY, 0x1004, 0x24)]
        [InlineData(Reg16.SP, 0x1005, 0x25)]
        public void Load_16BIT_Register_With_The_Contents_Of_A_Memory_Location(Reg16 target, ushort memoryLocation, byte value)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryLocation, value);
            machine.LD(target, valueAt(memoryLocation));

            Assert.Equal(value, machine.Registers.Read(target));
        }

        [Theory]
        [InlineData(Reg16.SP, Reg16.HL, 0xAAAA)]
        [InlineData(Reg16.SP, Reg16.IX, 0xAAAA)]
        [InlineData(Reg16.SP, Reg16.IY, 0xAAAA)]
        public void Load_16BIT_Register_With_The_Contents_Of_A_16Bit_Register(Reg16 target, Reg16 source, ushort value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.LD(target, source);

            Assert.Equal(value, machine.Registers.Read(target));
        }


        [Theory]
        [InlineData(0x1000, Reg16.BC, 0xAAAA)]
        public void Load_A_Memory_Address_With_The_Contents_Of_A_16Bit_Register(ushort memoryLocation, Reg16 source, ushort value)
        {
            var machine = CreateMachine();
            machine.Registers.Set(source, value);
            machine.LD(valueAt(memoryLocation), source);

            Assert.Equal(value, machine.Memory.ReadWord(memoryLocation));
        }

        //*************************************  MISC ************************************************


        [Fact]
        public void LoadingOneRegisterShouldNotChangeAnotherRegister()
        {
            var machine = CreateMachine();

            var currentValue = machine.Registers.Read(Reg8.B);
            machine.LD(Reg8.C, Read8BitValue(123));

            Assert.Equal(currentValue, machine.Registers.Read(Reg8.B));
        }

    }
}
