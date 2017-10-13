using System;
using System.Collections.Generic;
using System.Text;
using z80vm;
using System.Reflection;
using static z80vm.op8;

namespace z80Assembler
{
    public class CommandRunner
    {
        private Machine machine;

        public CommandRunner(Machine machine)
        {
            this.machine = machine;
        }

        public void RunCommand(string command)
        {
            var instruction = command.Substring(0, command.IndexOf(' '));
            var operands = command.Substring(command.IndexOf(' ') + 1).Split(',');
            var machineType = this.machine.GetType();


            var reg8_1 = (Reg8)Enum.Parse(typeof(Reg8), operands[0]);
            var op8_1 = Read8BitValue(reg8_1);

            var parameterTypes = default(Type[]);
            var parameters = default(Object[]);

            if (operands.Length == 1)
            {
                parameterTypes = new[] { typeof(op8) };
                parameters = new[] { op8_1 };
            }

            if (operands.Length > 1)
            {
                parameterTypes = new[] { typeof(Reg8), typeof(op8) };

                var reg8_2 = (Reg8)Enum.Parse(typeof(Reg8), operands[1]);
                var op8_2 = Read8BitValue(reg8_2);
                parameters = new object[] { reg8_1, op8_2 };
            }

            var method = machineType.GetRuntimeMethod(instruction, parameterTypes);
            method.Invoke(machine, parameters);
        }
    }
}
