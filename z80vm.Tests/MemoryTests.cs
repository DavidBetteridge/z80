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

            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, ANY_VALUE);

            Assert.Equal(ANY_VALUE, machine.Memory.ReadByte(memoryAddress));
        }

        [Theory]
        [InlineData(0,0)]
        [InlineData(0xAAAA, 0xAAAA)]
        [InlineData(0xFFFE, 0xFFFF)]
        public void CanSetAndReadAWordFromMemory(ushort memoryAddress, ushort value)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, value);

            Assert.Equal(value, machine.Memory.ReadWord(memoryAddress));
        }

        private static Machine CreateMachine()
        {
            return new Machine(new ConditionValidator());
        }
    }
}
