using System;

namespace z80Assembler
{
    [System.Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
