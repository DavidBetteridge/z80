﻿using System;
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

        public bool RunNextCommand()
        {
            // Read command pointed to by PC
            var newProgramCounter = _machine.Registers.Read(Reg16.PC);
            var currentProgramCounter = newProgramCounter;

            // What command are we going to run?
            var commandInHex = _machine.Memory.ReadByte((newProgramCounter));
            newProgramCounter++;
            var instruction = new InstructionInfo(commandInHex, _instructionLookups);

            // Do we need to supply a parameter?
            var command = instruction.CommandText;
            var operandLength = OperandLength.None;
            if (command.Contains(("nn")))
            {
                var operand = _machine.Memory.ReadWord(newProgramCounter);
                command = command.Replace("nn", operand.ToString());
                newProgramCounter += 2;
                operandLength = OperandLength.Short;
            }

            if (command.Contains(("n")))
            {
                var operand = _machine.Memory.ReadByte(newProgramCounter);
                command = command.Replace("n", operand.ToString());
                newProgramCounter++;
                operandLength = OperandLength.Byte;
            }

            if (command.Contains(("d")))
            {
                var operand = _machine.Memory.ReadByte(newProgramCounter);
                command = command.Replace("d", operand.ToString());
                newProgramCounter++;
                operandLength = OperandLength.Offset;
            }

            var wasHaltCommand = RunCommand(command, operandLength, instruction);

            // Increase PC if not changed by command
            if (currentProgramCounter == _machine.Registers.Read(Reg16.PC))
                _machine.Registers.Set(Reg16.PC, newProgramCounter);


            return wasHaltCommand;
        }

        public bool RunCommand(string command, OperandLength operandLength, InstructionInfo instructionInfo)
        {
            var parameterTypes = new List<Type>();
            var parameters = new List<object>();
            string instruction;

            // The aim is to populate 
            //  instruction - with the name of the instruction,  for example LD
            //  parameterTypes - with the types of the parameters to the instruction
            //  parameters - with the values of the parameters to the instruction
            var suspectOperand = -1;
            if (command.Contains(" "))
            {
                instruction = command.Substring(0, command.IndexOf(' '));
                var parms = command.Substring(command.IndexOf(' ')).Split(',');
                for (int i = 0; i < parms.Length; i++)
                {
                    var parm = parms[i].Trim();

                    if (TryGet8BitRegister(parm, out var reg81))
                    {
                        // There are two options at this point,  either this method is expecting
                        // to be passed a 8bit register or a 8bit register wrapped in an op8.   For now we assume 
                        // that a byte is required,  but it fail to get a match then we try an op8
                        if (i == parms.Length - 1) suspectOperand = i;
                        parameterTypes.Add(typeof(Reg8));
                        parameters.Add(reg81);
                    }
                    else if (TryGet16BitRegister(parm, out var reg16))
                    {
                        parameterTypes.Add(typeof(Reg16));
                        parameters.Add(reg16);
                    }
                    else
                    {
                        switch (operandLength)
                        {
                            case OperandLength.None:
                                break;
                            case OperandLength.Offset:
                                parameterTypes.Add(typeof(sbyte));
                                parameters.Add((sbyte)byte.Parse(parm));
                                break;
                            case OperandLength.Byte:
                                // There are two options at this point,  either this method is expecting
                                // to be passed a byte or a byte wrapped in an op8.   For now we assume 
                                // that a byte is required,  but it fail to get a match then we try an op8
                                if (i == parms.Length - 1) suspectOperand = i;
                                parameterTypes.Add(typeof(byte));
                                parameters.Add(byte.Parse(parm));
                                break;
                            case OperandLength.Short:
                                parameterTypes.Add(typeof(ushort));
                                parameters.Add(ushort.Parse(parm));
                                break;
                            default:
                                break;
                        }
                    }

                }

            }
            else
            {
                // The command has no arguments (operands) for example NOP.
                instruction = command;
            }

            var machineType = _machine.GetType();
            var parmsArray = parameters.ToArray();
            var parmTypesArray = parameterTypes.ToArray();

            var method = machineType.GetRuntimeMethod(instruction, parmTypesArray);

            if (method == null && suspectOperand != -1)
            {
                var previousType = parmTypesArray[suspectOperand];
                parmTypesArray[suspectOperand] = typeof(op8);

                if (previousType == typeof(Byte))
                    parmsArray[suspectOperand] = Read8BitValue((byte)parmsArray[suspectOperand]);

                if (previousType == typeof(Reg8))
                    parmsArray[suspectOperand] = Read8BitValue((Reg8)parmsArray[suspectOperand]);

                method = machineType.GetRuntimeMethod(instruction, parmTypesArray);
            }

            method.Invoke(_machine, parmsArray);

            return (instruction == "HALT");
        }

        private bool TryGet16BitRegister(string operand, out Reg16 reg16)
        {
            var result = true;

            switch (operand)
            {
                case "AF":
                    reg16 = Reg16.AF;
                    break;

                case "BC":
                    reg16 = Reg16.BC;
                    break;

                case "DE":
                    reg16 = Reg16.DE;
                    break;

                case "HL":
                    reg16 = Reg16.HL;
                    break;

                case "IX":
                    reg16 = Reg16.IX;
                    break;

                case "IY":
                    reg16 = Reg16.IY;
                    break;

                case "PC":
                    reg16 = Reg16.PC;
                    break;

                case "SP":
                    reg16 = Reg16.SP;
                    break;
                default:
                    reg16 = Reg16.AF;
                    result = false;
                    break;
            }

            return result;

        }

        private bool TryGet8BitRegister(string operand, out Reg8 reg8)
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
