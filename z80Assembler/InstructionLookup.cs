using System.Collections.Generic;

namespace z80Assembler
{
    public class InstructionLookup
    {
        public string Normal { get; internal set; }
        public byte Hex { get; internal set; }
        public string DDPrefix { get; set; }
        public string CBPrefix { get; set; }
        public string EDPrefix { get; set; }
        public string DDCBPrefix { get; set; }

        public InstructionLookup(string line)
        {
            //|[09 | 009] | ADD  HL,BC | ADD  IX,BC | RRC   C | *NOP | *LD    C,RRC(IX + d) |
            var parts = line.Split('|');
            this.Hex = byte.Parse(parts[1].Substring(1).Trim(), System.Globalization.NumberStyles.HexNumber);

            this.Normal = CleanValue(parts[3]);
            this.DDPrefix = CleanValue(parts[4]);
            this.CBPrefix = CleanValue(parts[5]);
            this.EDPrefix = CleanValue(parts[6]);
            this.DDCBPrefix = CleanValue(parts[7]);
        }

        private string CleanValue(string value)
        {
            var cleaned = value
                .Trim()
                .Replace(", ", ",")
                .Replace("*", "");
                //.Replace("nn", "n");

            while (cleaned.Contains("  "))
                cleaned = cleaned.Replace("  ", " ");

            return cleaned;
        }
    }
}
