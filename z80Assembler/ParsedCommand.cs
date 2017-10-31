using System.Collections.Generic;

namespace z80Assembler
{
    public class ParsedCommand
    {
        /// <summary>
        /// The opcode which represents this command.  It's an integer rather than
        /// a byte to allow for DDCB etc prefixes.
        /// </summary>
        public int OpCode { get; set; }

        /// <summary>
        /// Set to TRUE when the command cannot be parsed
        /// </summary>
        public bool IsInValid { get; set; }

        /// <summary>
        /// The total length including it's operands of this command
        /// (ie the number of bytes in memory required by this command)
        /// </summary>
        public int TotalLength { get; set; }

        /// <summary>
        /// The location at which this command is going to be loaded into memory
        /// </summary>
        public ushort MemoryLocation { get; set; }

        /// <summary>
        /// This can be either an 8bit byte or an 16bit ushort
        /// </summary>
        public ushort Operand1 { get; set; }

        /// <summary>
        /// This can be either an 8bit byte or an 16bit ushort
        /// </summary>
        public ushort Operand2 { get; set; }

        /// <summary>
        /// This can be either an 8bit byte or an 16bit ushort
        /// </summary>
        public ushort Operand3 { get; set; }
    }
}
