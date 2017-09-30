﻿using System;
using Moq;
using Xunit;

namespace z80vm.Tests
{
    public class JRTests
    {
        [Theory]
        [InlineData(100, 1100)]
        [InlineData(-128, 872)]
        [InlineData(0, 1000)]
        [InlineData(127, 1127)]
        public void TheProgramCounterShouldBeAdvancedByTheSuppliedValue(sbyte offset, ushort newAddress)
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            var machine = new Machine(conditionValidator.Object);
            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(offset);

            Assert.Equal(newAddress, machine.Registers.Read(Reg16.PC));
        }

        [InlineData(0xFFFF, 1)]
        [InlineData(0x0, -1)]
        public void AnErrorShouldBeReportedIfTheInstructionJumpsOutOfTheMemorySpace(ushort start, sbyte offset)
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            var machine = new Machine(conditionValidator.Object);
            machine.Registers.Set(Reg16.PC, start);

            var exception = Record.Exception(() => machine.JR(offset));
            Assert.IsType(typeof(System.OverflowException), exception);
        }

        [Fact]
        public void TheJumpShouldHappenWhenTheConditionIsTrue()
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            conditionValidator.Setup(s => s.IsTrue(It.IsAny<Flags>(), It.IsAny<Condition>())).Returns(true);

            var machine = new Machine(conditionValidator.Object);
            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(Condition.c, 1);

            Assert.Equal(1001, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(Condition.c)]
        [InlineData(Condition.nc)]
        [InlineData(Condition.z)]
        [InlineData(Condition.nz)]
        public void TheJumpShouldNotHappenWhenTheConditionIsFalse(Condition condition)
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            conditionValidator.Setup(s => s.IsTrue(It.IsAny<Flags>(), condition)).Returns(false);

            var machine = new Machine(conditionValidator.Object);
            machine.Registers.Set(Reg16.PC, 1000);

            machine.JR(condition, 1);

            Assert.NotEqual(1001, machine.Registers.Read(Reg16.PC));
        }

        [Theory]
        [InlineData(Condition.m)]
        [InlineData(Condition.p)]
        [InlineData(Condition.pe)]
        [InlineData(Condition.po)]
        public void AnErrorShouldBeReportedIfAnUnsupportedConditionIsSupplied(Condition condition)
        {
            var conditionValidator = new Moq.Mock<IConditionValidator>();
            var machine = new Machine(conditionValidator.Object);

            var exception = Record.Exception(() => machine.JR(condition, 1));
            Assert.IsType(typeof(System.InvalidOperationException), exception);
        }

    }
}