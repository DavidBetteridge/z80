using Xunit;

namespace z80vm.Tests
{
    public class CommandValidatorTests
    {
        [Fact]
        public void TheCommand_Add_a_a_Is_Valid()
        {
            var cv = new CommandValidator();
            cv.EnsureCommandIsValid(Reg8.A, Reg8.A, "ADD");

            Assert.True(true);
        }

        [Fact]
        public void TheCommand_Add_b_a_Is_NotValid()
        {
            var cv = new CommandValidator();
            var exception = Record.Exception(() => cv.EnsureCommandIsValid(Reg8.B, Reg8.A, "ADD"));
            Assert.IsType<System.InvalidOperationException>(exception);
        }

        [Fact]
        public void TheCommand_INC_a_Is_Valid()
        {
            var cv = new CommandValidator();
            cv.EnsureCommandIsValid(Reg8.A, "INC");

            Assert.True(true);
        }

        [Fact]
        public void TheCommand_INC_AddressInSP_Is_NotValid()
        {
            var cv = new CommandValidator();
            var exception = Record.Exception(() => cv.EnsureCommandIsValid(Value.valueAt(Reg16.SP), "INC"));
            Assert.IsType<System.InvalidOperationException>(exception);
        }
    }
}
