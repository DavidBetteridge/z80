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

        /// <summary>
        /// Amount of memory the third operand requires,  could be 0, 1 or 2 bytes
        /// </summary>
        public int Operand3Size { get; set; }


        public InstructionInfo(int hexCode, InstructionLookups InstructionLookup)
        {
            int GetOperandSize(string operand)
            {
                switch (operand)
                {
                    case "(n)":
                    case "d":
                    case "n":
                    case "(IX+d)":
                        return 1;

                    case "(nn)":
                    case "nn":
                        return 2;

                    default:
                        return 0;
                }
            }


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
                this.Operand1Size = GetOperandSize(operands[0]);
            }

            if (operands.Length > 1)
            {
                this.Operand2Size = GetOperandSize(operands[1]);
            }

            if (operands.Length > 2)
            {
                this.Operand3Size = GetOperandSize(operands[2]);
            }

            if (hexCode.Second() != 0)
                this.Length = 3;
            else if (hexCode.Third() != 0)
                this.Length = 2;
            else
                this.Length = 1;

            this.Length += this.Operand1Size + this.Operand2Size + this.Operand3Size;
        }
    }
}
