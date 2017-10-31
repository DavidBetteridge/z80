using Xunit;

namespace z80vm.Tests
{
    public class HALTTests : TestBase
    {
        [Fact]
        public void Calling_HALT_Should_Do_Nothing()
        {
            var machine = this.CreateMachine();
            machine.HALT();
        }
    }
}
