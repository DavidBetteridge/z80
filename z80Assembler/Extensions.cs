namespace z80Assembler
{
    public static class Extensions
    {
        /// <summary>
        /// Extracts the high order byte from the word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static byte High(this ushort word) => (byte)(word >> 8);

        /// <summary>
        /// Extracts the low order byte from the word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static byte Low(this ushort word) => (byte)(word & 0x00FF);

        /// <summary>
        /// Extracts the second byte from the integer
        /// </summary>
        public static byte Second(this int i) => (byte)((i & 0xFF0000) >> 16);

        /// <summary>
        /// Extracts the 3rd byte from the integer
        /// </summary>
        public static byte Third(this int i) => (byte)((i & 0xFF00) >> 8);

        /// <summary>
        /// Extracts the 4th byte from the integer
        /// </summary>
        public static byte Final(this int i) => (byte)((i & 0xFF));
    }
}
