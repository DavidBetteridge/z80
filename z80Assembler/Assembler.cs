using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace z80Assembler
{
    public class Assembler
    {
        private readonly InstructionLookups _instructionLookups;
        public Assembler()
        {
            _instructionLookups = new InstructionLookups();
            _instructionLookups.Load();
        }

        public List<byte> Parse(string command)
        {
            var results = new List<byte>();

            var cmd = _instructionLookups.TryLookupHexCodeFromNormalisedCommand(Normalise(command));
            if (cmd == null) return results;//Invalid command
            var hex = cmd.HexCode;

            if (hex.Second() != 0) results.Add(hex.Second());
            if (hex.Third() != 0) results.Add(hex.Third());
            results.Add(hex.Final());

            // Remove the command from the start
            command = command.Trim();
            var withOutCommand = "";
            if (command.Contains(" "))
                withOutCommand = command.Substring(command.IndexOf(' ')).Trim();

            var operands = withOutCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (operands.Length > 0)
            {
                ParseOperand(results, operands[0], cmd.Operand1Size);
            }

            if (operands.Length > 1)
            {
                ParseOperand(results, operands[1], cmd.Operand2Size);
            }

            return results;
        }

        private static void ParseOperand(List<byte> results, string operand, int operandSize)
        {
            operand = operand.Trim();

            // Immediate Value n == byte
            if (Regex.IsMatch(operand, "^[0-9]*$"))
            {
                var v = ushort.Parse(operand);
                results.Add(v.Low());

                if (operandSize == 2)
                {
                    results.Add(v.High());
                }
            }

            // Memory Address (nn) == 2 bytes
            if (Regex.IsMatch(operand, @"^\([0-9]*\)$"))
            {
                var address = operand.Replace("(", "").Replace(")", "");
                var memoryAddress = ushort.Parse(address);
                results.Add(memoryAddress.Low());
                results.Add(memoryAddress.High());
            }
        }

        public string Normalise(string command)
        {
            command = command.Replace(", ", ",");
            while (command.Contains("  ")) command = command.Replace("  ", " ");
            command = Regex.Replace(command, "[0-9]+", "n");
            command = command.Trim();
            return command;
        }

        public int CalculateCommandLength(string command)
        {
            var cmd = Normalise(command);
            var info = _instructionLookups.TryLookupHexCodeFromNormalisedCommand(Normalise(command));
            if (info == null)
            {
                return 0;
            }
            return info.Length;
        }
    }
}
