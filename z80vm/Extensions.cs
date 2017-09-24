using System;

namespace z80vm
{
    static class Extensions
    {
        /// <summary>
        /// Splits a 16bit number into two bytes
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static (byte high, byte low) Split(this ushort word)
        {
            var highOrderByte = (byte)(word >> 8);
            var lowOrderByte = (byte)(word & 0x00FF);

            return (highOrderByte, lowOrderByte);
        }

        /// <summary>
        /// Reads a single bit from the byte
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitNumber">7 6 5 4 3 2 1 0</param>
        /// <returns></returns>
        public static bool ReadBit(this byte value, int bitNumber)
        {
            if (bitNumber < 0 || bitNumber > 7) throw new ArgumentOutOfRangeException(nameof(bitNumber), "Must be between 0 and 7");
            return (value & (1 << bitNumber)) != 0;
        }

        /// <summary>
        /// Returns a new byte based on the supplied value but with the specified bit reset to 0
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitNumber">7 6 5 4 3 2 1 0</param>
        /// <returns></returns>
        public static byte ClearBit(this byte value, int bitNumber)
        {
            if (bitNumber < 0 || bitNumber > 7) throw new ArgumentOutOfRangeException(nameof(bitNumber), "Must be between 0 and 7");
            return (byte)(value & ~(1 << bitNumber));
        }

        /// <summary>
        /// Returns a new byte based on the supplied value but with the specified bit set to 1
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bitNumber">7 6 5 4 3 2 1 0</param>
        /// <returns></returns>
        public static byte SetBit(this byte value, int bitNumber)
        {
            if (bitNumber < 0 || bitNumber > 7) throw new ArgumentOutOfRangeException(nameof(bitNumber), "Must be between 0 and 7");
            return (byte)(value | (1 << bitNumber));
        }
    }
}
