using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace z80vm.Tests
{
    public class NEGTests :TestBase
    {
        [Theory]
        [InlineData(100, -100)]
        [InlineData(-100, 100)]
        public void The_Value_In_Accumulator_Should_Be_Negated(sbyte startingValue, sbyte expectedValue)
        {
            var machine = CreateMachine();
            machine.Registers.Set(Reg8.A, (byte)startingValue);

            machine.NEG();

            Assert.Equal((byte)expectedValue, machine.Registers.Read(Reg8.A));
        }
    }
}
