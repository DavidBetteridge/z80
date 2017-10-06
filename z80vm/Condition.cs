namespace z80vm
{
    public enum Condition
    {
        /// <summary>
        /// carry (C) is set
        /// </summary>
        c = 0,

        /// <summary>
        /// carry is not set
        /// </summary>
        nc = 1,

        /// <summary>
        /// zero (Z) is set
        /// </summary>
        z,

        /// <summary>
        /// zero is not set
        /// </summary>
        nz,

        /// <summary>
        /// sign (S) is set
        /// </summary>
        m,

        /// <summary>
        /// sign is not set
        /// </summary>
        p,

        /// <summary>
        /// parity/overflow (P/V) is set
        /// </summary>
        pe,

        /// <summary>
        /// parity/overflow is not set
        /// </summary>
        po
    }
}
