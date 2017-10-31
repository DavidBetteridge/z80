using System.Collections.Generic;
using z80vm;

namespace z80Assembler
{
    public class Loader
    {
        private readonly Machine _machine;
        public Loader(Machine machine)
        {
            _machine = machine;
        }

        public void LoadCommands(IEnumerable<ParsedCommand> parsedCommands)
        {
            ushort memoryLocation = 0x00;
            foreach (var command in parsedCommands)
            {
                // An OPCODE can be 1,2 or 3 bytes long
                _machine.Memory.Set(memoryLocation, command.OpCode.Final());
                memoryLocation++;

                if (command.OpCode.Third() != 0)
                {
                    _machine.Memory.Set(memoryLocation, command.OpCode.Third());
                    memoryLocation++;
                }

                if (command.OpCode.Second() != 0)
                {
                    _machine.Memory.Set(memoryLocation, command.OpCode.Second());
                    memoryLocation++;
                }

                // An OPCODE can have between 0 and 3 operands
                switch (command.Operand1Length)
                {
                    case 1:
                        // Operand is a single byte long
                        _machine.Memory.Set(memoryLocation, command.Operand1.Low());
                        memoryLocation++;
                        break;

                    case 2:
                        // Operand is a 2 bytes long
                        _machine.Memory.Set(memoryLocation, command.Operand1);
                        memoryLocation += 2;
                        break;

                    default:
                        break;
                }

                switch (command.Operand2Length)
                {
                    case 1:
                        // Operand is a single byte long
                        _machine.Memory.Set(memoryLocation, command.Operand2.Low());
                        memoryLocation++;
                        break;

                    case 2:
                        // Operand is a 2 bytes long
                        _machine.Memory.Set(memoryLocation, command.Operand2);
                        memoryLocation += 2;
                        break;

                    default:
                        break;
                }

                switch (command.Operand3Length)
                {
                    case 1:
                        // Operand is a single byte long
                        _machine.Memory.Set(memoryLocation, command.Operand3.Low());
                        memoryLocation++;
                        break;

                    case 2:
                        // Operand is a 2 bytes long
                        _machine.Memory.Set(memoryLocation, command.Operand3);
                        memoryLocation += 2;
                        break;

                    default:
                        break;
                }
            }
        }

    }
}
