using Xunit;

namespace z80vm.Tests
{
    public class ConditionValidatorTests
    {
        [Theory]
        [InlineData(Condition.c, Flag.C, true)]
        [InlineData(Condition.m, Flag.S, true)]
        [InlineData(Condition.z, Flag.Z, true)]
        [InlineData(Condition.pe, Flag.PV, true)]
        [InlineData(Condition.nc, Flag.C, false)]
        [InlineData(Condition.nz, Flag.Z, false)]
        [InlineData(Condition.p, Flag.S, false)]
        [InlineData(Condition.po, Flag.PV, false)]
        public void TheValidatorShouldReturnTrueForConditionIfTheFlagIsSet(Condition condition, Flag flag, bool setFlag)
        {
            var registers = new Registers();
            var flags = new Flags(registers);
            var conditionValidator = new ConditionValidator();

            if (setFlag)
                flags.Set(flag);

            var conditionHolds = conditionValidator.IsTrue(flags, condition);

            Assert.True(conditionHolds);
        }

        [Theory]
        [InlineData(Condition.c, Flag.C, true)]
        [InlineData(Condition.m, Flag.S, true)]
        [InlineData(Condition.z, Flag.Z, true)]
        [InlineData(Condition.pe, Flag.PV, true)]
        [InlineData(Condition.nc, Flag.C, false)]
        [InlineData(Condition.nz, Flag.Z, false)]
        [InlineData(Condition.p, Flag.S, false)]
        [InlineData(Condition.po, Flag.PV, false)]
        public void TheValidatorShouldReturnFalseForConditionIfTheFlagIsNotSet(Condition condition, Flag flag, bool clearFlag)
        {
            var registers = new Registers();
            var flags = new Flags(registers);
            var conditionValidator = new ConditionValidator();

            if (!clearFlag)
                flags.Set(flag);

            var conditionHolds = conditionValidator.IsTrue(flags, condition);

            Assert.False(conditionHolds);
        }

    }
}
