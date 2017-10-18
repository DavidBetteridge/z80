using System;
using z80vm;

namespace z80Assembler
{
    public class Loader
    {
        private readonly Machine _machine;
        private readonly Assembler _assembler;
        private ushort _nextAddress;

        public Loader(Machine machine)
        {
            _machine = machine;
            _assembler = new Assembler();
            _nextAddress = 0x00;
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
            foreach (var command in commands.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                LoadCommand(command);
            }
        }
    }
}
