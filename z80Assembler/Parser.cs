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

            var result = new List<ParsedCommand>();
            ushort nextMemoryLocation = 0;
            foreach (var command in commands)
            {
                // Create a normalised version of the command
                var normalisedCommand = Normalise(command);
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

                    nextMemoryLocation += (ushort)parsedCommand.TotalLength;
                }

                result.Add(parsedCommand);
            }

            return result;
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
    }
}
