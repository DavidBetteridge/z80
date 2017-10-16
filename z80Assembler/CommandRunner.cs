using System;
using System.Collections.Generic;
using z80vm;
using System.Reflection;
using static z80vm.op8;

namespace z80Assembler
{
    public class CommandRunner
    {
        private readonly Machine _machine;
        private readonly InstructionLookups _instructionLookups;

        public CommandRunner(Machine machine)
        {
            _machine = machine;
            _instructionLookups = new InstructionLookups();
            _instructionLookups.Load();
        }

        public void RunNextCommand()
        {
            // Read command pointed to by PC
            var newProgramCounter = _machine.Registers.Read(Reg16.PC);
            var currentProgramCounter = newProgramCounter;

            // What command are we going to run?
            var commandInHex = _machine.Memory.ReadByte((newProgramCounter));
            var command = _instructionLookups.LookupCommandFromHexCode(commandInHex);
            newProgramCounter++;

            // Do we need to supply a parameter?
            if (command.Contains(("nn")))
            {
                var operand = _machine.Memory.ReadWord(newProgramCounter);
                command = command.Replace("nn", operand.ToString());
                newProgramCounter += 2;
            }

            if (command.Contains(("n")))
            {
                var operand = _machine.Memory.ReadByte(newProgramCounter);
                command = command.Replace("n", operand.ToString());
                newProgramCounter++;
            }

            RunCommand(command);

            // Increase PC if not changed by command
            if (currentProgramCounter == _machine.Registers.Read(Reg16.PC))
                _machine.Registers.Set(Reg16.PC, newProgramCounter);

        }

        public void RunCommand(string command)
        {
            var parameterTypes = default(Type[]);
            var parameters = new List<object>();
            string instruction;

            if (!command.Contains(" "))
            {
                // The command has no arguments (operands) for example NOP.
                instruction = command;
                parameterTypes = new Type[0];
            }
            else
            {
                // This command has one or two arguments.
                instruction = command.Substring(0, command.IndexOf(' '));
                var operands = command.Substring(command.IndexOf(' ') + 1).Split(',');

                if (operands.Length == 1)
                {
                    if (TryGetRegister(operands[0], out var reg81))
                    {
                        var op81 = Read8BitValue(reg81);

                        parameterTypes = new[] { typeof(op8) };
                        parameters.Add(op81);
                    }
                    else
                    {
                        parameterTypes = new[] { typeof(ushort) };
                        parameters.Add(ushort.Parse(operands[0]));
                    }
                }

                if (operands.Length == 2)
                {
                    TryGetRegister(operands[0], out var reg81);

                    parameterTypes = new[] { typeof(Reg8), typeof(op8) };

                    if (TryGetRegister(operands[1], out var reg82))
                    {
                        var op82 = Read8BitValue(reg82);
                        parameters.Add(reg81);
                        parameters.Add(op82);
                    }
                    else
                    {
                        var asImmediate = byte.Parse(operands[1]);
                        var op82 = Read8BitValue(asImmediate);
                        parameters.Add(reg81);
                        parameters.Add(op82);
                    }
                }
            }

            var machineType = _machine.GetType();
            var method = machineType.GetRuntimeMethod(instruction, parameterTypes);
            method.Invoke(_machine, parameters.ToArray());
        }

        private bool TryGetRegister(string operand, out Reg8 reg8)
        {
            var result = true;

            switch (operand)
            {
                case "A":
                    reg8 = Reg8.A;
                    break;
                case "B":
                    reg8 = Reg8.B;
                    break;
                case "C":
                    reg8 = Reg8.C;
                    break;
                case "D":
                    reg8 = Reg8.D;
                    break;
                case "E":
                    reg8 = Reg8.E;
                    break;
                case "F":
                    reg8 = Reg8.F;
                    break;
                case "H":
                    reg8 = Reg8.H;
                    break;
                case "I":
                    reg8 = Reg8.I;
                    break;
                case "IXH":
                    reg8 = Reg8.IXH;
                    break;
                case "IXL":
                    reg8 = Reg8.IXL;
                    break;
                case "IYH":
                    reg8 = Reg8.IYH;
                    break;
                case "IYL":
                    reg8 = Reg8.IYL;
                    break;
                case "L":
                    reg8 = Reg8.L;
                    break;
                case "R":
                    reg8 = Reg8.R;
                    break;
                default:
                    reg8 = Reg8.A;
                    result = false;
                    break;
            }

            return result;
        }
    }
}
