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

            var hex = _instructionLookups.LookupHexCodeFromNormalisedCommand(Normalise(command));
            if (hex.Second() != 0) results.Add(hex.Second());
            if (hex.Third() != 0) results.Add(hex.Third());
            results.Add(hex.Final());

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
