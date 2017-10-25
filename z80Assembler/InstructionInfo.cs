using System;

namespace z80Assembler
{
    public class InstructionInfo
    {
        /// <summary>
        /// Some hex codes could contain up to 3 bytes
        /// The 4 MSBs will always be zero 
        /// </summary>
        public int HexCode { get; private set; }

        /// <summary>
        /// How many bytes of memory does this command take up?
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Amount of memory the first operand requires,  could be 0, 1 or 2 bytes
        /// </summary>
        public int Operand1Size { get; set; }

        /// <summary>
        /// Amount of memory the second operand requires,  could be 0, 1 or 2 bytes
        /// </summary>
        public int Operand2Size { get; set; }

        public InstructionInfo(int hexCode, InstructionLookups InstructionLookup)
        {
            this.HexCode = hexCode;

            var cmd = InstructionLookup.LookupCommandFromHexCode(hexCode);

            // Split up the command into the command and it's two operands
            cmd = cmd.Trim();
            var withOutCommand = "";
            if (cmd.Contains(" "))
                withOutCommand = cmd.Substring(cmd.IndexOf(' ')).Trim();

            var operands = withOutCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (operands.Length > 0)
            {
                switch (operands[0])
                {
                    case "n":
                        this.Operand1Size = 1;
                        break;
                    case "nn":
                        this.Operand1Size = 2;
                        break;
                    default:
                        this.Operand1Size = 0;
                        break;
                }
            }

            if (operands.Length > 1)
            {
                switch (operands[1])
                {
                    case "n":
                        this.Operand2Size = 1;
                        break;
                    case "nn":
                        this.Operand2Size = 2;
                        break;
                    default:
                        this.Operand2Size = 0;
                        break;
                }
            }

            this.Length = 1 + this.Operand1Size + this.Operand2Size;
            //if (cmd.Contains("nn"))
            //{
            //    Length += 2;
            //    cmd = cmd.Replace("nn", "");
            //}

            //if (cmd.Contains("n"))
            //{
            //    Length++;
            //    cmd = cmd.Replace("n", "");
            //}
        }
    }
}
