using Xunit;

namespace z80vm.Tests
{
    public class MemoryTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(0xAAAA)]
        [InlineData(0xFFFF)]
        public void CanSetAndReadAValueFromMemory(ushort memoryAddress)
        {
            const byte ANY_VALUE = 123;

            var machine = new Machine();
            machine.Memory.Set(memoryAddress, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Memory.Read(memoryAddress));
        }
    }
}
