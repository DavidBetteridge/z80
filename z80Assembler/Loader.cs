using System;
using System.Collections.Generic;
using z80vm;

namespace z80Assembler
{
    public class Loader
    {
        private readonly Machine _machine;
        private readonly Assembler _assembler;
        private ushort _nextAddress;

        public Dictionary<string, ushort> Labels { get; private set; }

        public Loader(Machine machine)
        {
            _machine = machine;
            _assembler = new Assembler();
            _nextAddress = 0x00;
            Labels = new Dictionary<string, ushort>();
        }

        /// <summary>
        /// Loads a single command into memory
        /// </summary>
        /// <param name="command"></param>
        public void LoadCommand(string command)
        {
            var opCode = _assembler.Parse(command);

            foreach (var part in opCode)
            {
                _machine.Memory.Set(_nextAddress, part);
                _nextAddress++;
            }
        }

        /// <summary>
        /// Loads multiple commands (seperated by newlines) into memory
        /// </summary>
        /// <param name="commands"></param>
        public void LoadCommands(string commands)
        {
            var listOfCommands = commands.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // First pass, get a list of labels
            foreach (var command in listOfCommands)
            {
                var label = ExtractLabel(command);
                if (!string.IsNullOrWhiteSpace(label))
                {
                    Labels.Add(label, 0);
                }
            }

            // Second pass,  
            ushort memoryAddress = 0;
            foreach (var command in listOfCommands)
            {
                var definedLabel = ExtractLabel(command);
                if (!string.IsNullOrWhiteSpace(definedLabel))
                {
                    Labels[definedLabel] = memoryAddress;
                }

                //replace labels with dummy memory locations so that they parse as then 
                //we know the length of the instruction
                var cleanCommand = command;
                foreach (var label in Labels.Keys)
                {
                    cleanCommand = cleanCommand.Replace(label, "00");
                }

                memoryAddress += (ushort)_assembler.Parse(cleanCommand).Count;
            }

            // Final pass, load the commands, but with the labels replaced
            // by their actual memory addresses
            foreach (var command in listOfCommands)
            {
                var cleanCommand = command;
                foreach (var label in Labels)
                {
                    cleanCommand = cleanCommand.Replace(label.Key, label.Value.ToString());
                }
                LoadCommand(cleanCommand);
            }
        }

        private string ExtractLabel(string command)
        {
            var i = command.IndexOf(':');
            if (i < 0) return string.Empty;
            return command.Substring(0, i);
        }
    }
}
