using System;

namespace z80vm
{
    public class Memory
    {
        public const ushort HIGHEST_ADDRESS = 0xFFFF;

        private byte[] data = new byte[HIGHEST_ADDRESS + 1];

        public byte ReadByte(ushort address)
        {
            return data[address];
        }

        public ushort ReadWord(ushort address)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            return MakeWord(data[address+1], data[address]);
        }

        public void Set(ushort address, byte value)
        {
            data[address] = value;
        }

        public void Set(ushort address, ushort value)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            data[address + 1] = value.High();
            data[address] = value.Low();
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
