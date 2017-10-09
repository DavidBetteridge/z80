using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace z80Assembler
{
    public class Assembler
    {
        public IEnumerable<byte> Parse(string command)
        {
            var results = new List<byte>();

            var lookup = new InstructionLookups();
            lookup.Load();

            var hex = lookup.LookupHexCodeFromNormal(this.Normalise(command));
            results.Add(hex);

            // Remove the command from the start
            command = command.Trim();
            var withOutCommand = "";
            if (command.Contains(" "))
                withOutCommand = command.Substring(command.IndexOf(' ')).Trim();

            var operands = withOutCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var operand in operands)
            {
                ParseOperand(results, operand);
            }

            return results;
        }

        private static void ParseOperand(List<byte> results, string operand)
        {
            operand = operand.Trim();

            // Immediate Value n == byte
            if (Regex.IsMatch(operand, "^[0-9]*$"))
                results.Add(byte.Parse(operand));

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
    }
}
