using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace z80Assembler
{
    public class Parser
    {
        private readonly InstructionLookups _instructionLookups;
        public Parser()
        {
            _instructionLookups = new InstructionLookups();
            _instructionLookups.Load();
        }

        public List<ParsedCommand> Parse(ushort baseMemoryAddress, string program)
        {
            // Each command is on it's own line
            var commands = program.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // First pass, get a list of labels
            var labels = new Dictionary<string, ushort>();
            foreach (var command in commands)
            {
                var label = ExtractLabel(command);
                if (!string.IsNullOrWhiteSpace(label))
                {
                    labels.Add(label, 0);
                }
            }

            // Second pass,  
            ushort nextMemoryLocation = baseMemoryAddress;
            foreach (var command in commands)
            {
                var definedLabel = ExtractLabel(command);
                if (!string.IsNullOrWhiteSpace(definedLabel))
                {
                    labels[definedLabel] = nextMemoryLocation;
                }

                //replace labels with dummy memory locations so that they parse as then 
                //we know the length of the instruction
                var cleanCommand = command;
                foreach (var label in labels.Keys)
                {
                    cleanCommand = cleanCommand.Replace(label, "00");
                }

                var normalisedCommand = Normalise(cleanCommand);
                var i = _instructionLookups.TryLookupHexCodeFromNormalisedCommand(normalisedCommand);
                if (i != null)
                    nextMemoryLocation += (ushort)i.Length;
            }

            var result = new List<ParsedCommand>();
            nextMemoryLocation = baseMemoryAddress;  //Reset after 2nd pass
            foreach (var command in commands)
            {
                var cleanCommand = command;

                // If present then remove the label defintion from the start of the command
                cleanCommand = RemoveLabel(cleanCommand);

                // Replace the labels by their actual memory addresses
                foreach (var label in labels)
                {
                    cleanCommand = cleanCommand.Replace(label.Key, label.Value.ToString());
                }

                // Create a normalised version of the command
                var normalisedCommand = Normalise(cleanCommand);
                var i = _instructionLookups.TryLookupHexCodeFromNormalisedCommand(normalisedCommand);

                var parsedCommand = new ParsedCommand();
                if (i == null)
                {
                    // This command could not be parsed
                    parsedCommand.IsInValid = true;
                }
                else
                {
                    // This command is fine
                    parsedCommand.IsInValid = false;
                    parsedCommand.OpCode = i.HexCode;
                    parsedCommand.TotalLength = i.Length;
                    parsedCommand.MemoryLocation = nextMemoryLocation;

                    var (operand1, operand2, operand3) = ParseOperands(i, cleanCommand);
                    parsedCommand.Operand1 = operand1;
                    parsedCommand.Operand2 = operand2;
                    parsedCommand.Operand3 = operand3;

                    nextMemoryLocation += (ushort)parsedCommand.TotalLength;
                }

                result.Add(parsedCommand);
            }

            return result;
        }

        /// <summary>
        /// Given a command prefixed with a label,  returns just it's label.
        /// For example JumpTo:  ADD A,B would return JumpTo
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string ExtractLabel(string command)
        {
            var i = command.IndexOf(':');
            if (i < 0) return string.Empty;
            return command.Substring(0, i).Trim();
        }

        /// <summary>
        /// Given a command prefixed with a label,  returns just the command.
        /// For example JumpTo:  ADD A,B would return ADD A,B
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string RemoveLabel(string command)
        {
            var i = command.IndexOf(':');
            if (i < 0) return command;
            return command.Substring(i + 1).Trim();
        }

        /// <summary>
        /// Normalise the command means converting the command into a format which matches the entries in
        /// z80Instructions.txt  For example replacing numbers with n so that LD BC,123 becomes LD BC, n
        /// The only exceptions are the RST, RES and SET command,  as RST 8 etc each has their own opcode
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private string Normalise(string command)
        {
            command = command.Replace(", ", ",");
            while (command.Contains("  ")) command = command.Replace("  ", " ");

            var parts = command.Split(',');
            command = "";
            foreach (var part in parts)
            {
                if (command != "") command += ",";

                if (part.StartsWith("RST") || part.StartsWith("RES") || part.StartsWith("SET"))
                {
                    //An exception
                    command += part;
                }
                else
                {
                    // Replace an numerics with a n
                    command += Regex.Replace(part, "[0-9]+", "n");
                }
            }
            command = command.Trim();
            return command;
        }

        private (ushort operand1, ushort operand2, ushort operand3) ParseOperands(InstructionInfo cmd, string command)
        {
            if (command.StartsWith("RST")) return (0, 0, 0);  //Exception

            // Remove the command from the start
            command = command.Trim();
            var withOutCommand = "";
            if (command.Contains(" "))
                withOutCommand = command.Substring(command.IndexOf(' ')).Trim();

            var operands = withOutCommand.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            ushort operand1 = 0;
            ushort operand2 = 0;
            ushort operand3 = 0;

            if (operands.Length > 0)
            {
                operand1 = ParseOperand(operands[0]);
            }

            if (operands.Length > 1)
            {
                operand2 = ParseOperand(operands[1]);
            }

            if (operands.Length > 2)
            {
                operand3 = ParseOperand(operands[2]);
            }

            return (operand1, operand2, operand3);
        }

        private static ushort ParseOperand(string operand)
        {
            operand = operand.Trim();

            // Immediate Value n == byte
            if (Regex.IsMatch(operand, "^[0-9]*$"))
            {
                return ushort.Parse(operand);
            }

            // Memory Address (nn) == 2 bytes
            if (Regex.IsMatch(operand, @"^\([0-9]*\)$"))
            {
                var address = operand.Replace("(", "").Replace(")", "");
                return ushort.Parse(address);
            }

            // (IX+d)  returns d (byte)
            var matches = Regex.Match(operand, @"^\(IX\+(?<d>[0-9]*)\)$");
            if (matches.Success)
            {
                var d = matches.Groups["d"].Value;
                return ushort.Parse(d);
            }

            return 0;
        }
    }
}
