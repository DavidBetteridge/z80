using Xunit;

namespace z80vm.Tests
{
    public class FlagsEvaluatorTests
    {
        [Theory]
        [InlineData(100, -1)]
        [InlineData(0, -10)]
        public void Set_The_S_Flag_When_The_Value_Changes_From_Positive_To_Negative(sbyte from, sbyte to)
        {
            SetupEvalulator(out var fe, out var flags);

            fe.Evalulate(flags, from, to);

            Assert.True(flags.Read(Flag.S));
        }

        [Fact]
        public void Clear_The_S_Flag_When_The_Value_Does_Not_Change_From_Positive_To_Negative()
        {
            SetupEvalulator(out var fe, out var flags);
            flags.Set(Flag.S);

            fe.Evalulate(flags, 100, 50);

            Assert.False(flags.Read(Flag.S));
        }

        [Fact]
        public void Set_The_Z_Flag_When_The_New_Value_Is_Zero()
        {
            SetupEvalulator(out var fe, out var flags);

            fe.Evalulate(flags, -100, 0);

            Assert.True(flags.Read(Flag.Z));
        }

        [Fact]
        public void Clear_The_Z_Flag_When_The_New_Value_Is_Not_Zero()
        {
            SetupEvalulator(out var fe, out var flags);
            flags.Set(Flag.Z);

            fe.Evalulate(flags, -100, 1);

            Assert.False(flags.Read(Flag.Z));
        }

        [Theory]
        [InlineData(-50, 50)]
        [InlineData(50, -50)]
        public void Set_The_PV_Flag_When_An_Overflow_Occurs(sbyte from, sbyte to)
        {
            SetupEvalulator(out var fe, out var flags);

            fe.Evalulate(flags, from, to);

            Assert.True(flags.Read(Flag.PV));
        }

        [Theory]
        [InlineData(-50, -49)]
        public void Clear_The_PV_Flag_When_An_Overflow_DoesNot_Occurs(sbyte from, sbyte to)
        {
            SetupEvalulator(out var fe, out var flags);
            flags.Set(Flag.PV);

            fe.Evalulate(flags, from, to);

            Assert.False(flags.Read(Flag.PV));
        }

        [Theory]
        [InlineData(-50, 50)]
        [InlineData(50, -50)]
        public void Set_The_C_Flag_When_The_Value_Crosses_Zero(sbyte from, sbyte to)
        {
            SetupEvalulator(out var fe, out var flags);

            fe.Evalulate(flags, from, to);

            Assert.True(flags.Read(Flag.C));
        }

        [Theory]
        [InlineData(-50, 0)]
        public void Clear_The_C_Flag_When_The_Value_DoesNot_Crosses_Zero(sbyte from, sbyte to)
        {
            SetupEvalulator(out var fe, out var flags);
            flags.Set(Flag.C);

            fe.Evalulate(flags, from, to);

            Assert.False(flags.Read(Flag.C));
        }

        private static void SetupEvalulator(out FlagsEvaluator fe, out Flags flags)
        {
            fe = new FlagsEvaluator();
            var registers = new Registers();
            flags = new Flags(registers);
        }
    }
}
