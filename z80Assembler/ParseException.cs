using System;

namespace z80Assembler
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
