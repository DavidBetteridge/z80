using Xunit;

namespace z80vm.Tests
{
    public class MemoryTests : TestBase
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
        [InlineData(0, 0)]
        [InlineData(0xAAAA, 0xAAAA)]
        [InlineData(0xFFFE, 0xFFFF)]
        public void CanSetAndReadAWordFromMemory(ushort memoryAddress, ushort value)
        {
            var machine = CreateMachine();
            machine.Memory.Set(memoryAddress, value);

            Assert.Equal(value, machine.Memory.ReadWord(memoryAddress));
        }

        [Fact]
        public void AnEventShouldBeRaisedWhenAByteMemoryAddressIsChanged()
        {
            const ushort ANY_ADDRESS = 0xAAAA;
            const byte ANY_VALUE = 123;

            ushort address = 0;
            byte newValue = 0;

            var machine = CreateMachine();
            machine.Memory.ValueChanged += (s, e) => { address = e.address; newValue = e.newValue; };
            machine.Memory.Set(ANY_ADDRESS, ANY_VALUE);

            Assert.Equal(ANY_ADDRESS, address);
            Assert.Equal(ANY_VALUE, newValue);
        }

        [Fact]
        public void TwoEventsShouldBeRaisedWhenAWordMemoryAddressIsChanged()
        {
            const ushort ANY_ADDRESS = 0xAAAA;
            const ushort ANY_VALUE = 300;

            ushort address = 0;
            byte newValue = 0;
            int eventCount = 0;

            var machine = CreateMachine();
            machine.Memory.ValueChanged += (s, e) =>
            {
                address = e.address;
                newValue = e.newValue;
                eventCount++;
            };
            machine.Memory.Set(ANY_ADDRESS, ANY_VALUE);

            Assert.Equal(ANY_ADDRESS + 1, address);
            Assert.Equal(ANY_VALUE >> 8, newValue);
            Assert.Equal(2, eventCount);
        }
    }
}
