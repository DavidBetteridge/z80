namespace z80Assembler
{
    public class InstructionLookup
    {
        public string Normal { get; internal set; }
        public byte Hex { get; internal set; }

        public InstructionLookup(string line)
        {
            //|[09 | 009] | ADD  HL,BC | ADD  IX,BC | RRC   C | *NOP | *LD    C,RRC(IX + d) |
            var parts = line.Split('|');
            this.Hex = byte.Parse(parts[1].Substring(1).Trim(), System.Globalization.NumberStyles.HexNumber);
            this.Normal = parts[3]
                            .Trim()
                            .Replace(", ", ",")
                            .Replace("nn", "n");
            while (this.Normal.Contains("  ")) this.Normal = this.Normal.Replace("  ", " ");
        }
    }
}
