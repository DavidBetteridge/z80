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
        
    }
}
