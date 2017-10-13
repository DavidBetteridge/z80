using Xunit;

namespace z80vm.Tests
{
    public class NOPTests : TestBase
    {
        [Fact]
        public void Calling_NOP_Should_Do_Nothing()
        {
            var machine = this.CreateMachine();
            machine.NOP();
        }
    }
}
