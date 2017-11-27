using System;

namespace z80vm
{
    public class MemoryValueChangedEventArgs : EventArgs
    {
        public ushort address { get; set; }
        public byte newValue { get; set; }
    }
    public class Memory
    {
        public const ushort HIGHEST_ADDRESS = 0xFFFF;

        private byte[] data = new byte[HIGHEST_ADDRESS + 1];

        public event EventHandler<MemoryValueChangedEventArgs> ValueChanged;

        public byte ReadByte(ushort address)
        {
            return data[address];
        }

        public ushort ReadWord(ushort address)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            return MakeWord(data[address + 1], data[address]);
        }

        public void Set(ushort address, byte value)
        {
            var currentValue = data[address];
            data[address] = value;

            if (value != currentValue)
                ValueChanged?.Invoke(this, new MemoryValueChangedEventArgs() { address = address, newValue = value });
        }

        public void Set(ushort address, ushort value)
        {
            var currentValue = ReadWord(address);

            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            data[address + 1] = value.High();
            data[address] = value.Low();

            if (currentValue != value)
            {
                ValueChanged?.Invoke(this, new MemoryValueChangedEventArgs() { address = address, newValue = value.Low() });
                ValueChanged?.Invoke(this, new MemoryValueChangedEventArgs() { address = (ushort)(address + 1), newValue = value.High() });
            }

        }

        /// <summary>
        /// Combines two bytes to make a word
        /// </summary>
        /// <param name="highOrderByte"></param>
        /// <param name="lowOrderByte"></param>
        /// <returns></returns>
        public static ushort MakeWord(byte highOrderByte, byte lowOrderByte)
            => (ushort)(((ushort)(highOrderByte << 8)) | lowOrderByte);
    }
}
