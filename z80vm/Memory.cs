namespace z80vm
{
    public class Memory
    {
        public const ushort HIGHEST_ADDRESS = 0xFFFF;

        private byte[] data = new byte[HIGHEST_ADDRESS + 1];

        public byte Read(ushort address)
        {
            return data[address];
        }

        public void Set(ushort address, byte value)
        {
            data[address] = value;
        }
    }
}
