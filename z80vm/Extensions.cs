namespace z80vm
{
    static class Extensions
    {
        /// <summary>
        /// Splits a 16bit number into two bytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static (byte high, byte low) Split(this ushort value)
        {
            var highOrderByte = (byte)(value >> 8);
            var lowOrderByte = (byte)(value & 0x00FF);

            return (highOrderByte, lowOrderByte);
        }
    }
}
