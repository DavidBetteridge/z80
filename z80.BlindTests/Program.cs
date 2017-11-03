using System;
using z80Assembler;
using z80vm;

namespace z80.BlindTests
{
    /* This program allows us to test out all the available commands by calling each
     * opcode in turn with some meaningless operand values
     * 
     * It's blind in the sense that it doesn't understand what is being tested.  It's just ensuring
     * that when passed a value parameter then the instruction doesn't throw an exceptions.
     * 
     * The instruction could still be carring out the wrong action.
     */
    class Program
    {
        static void Main(string[] args)
        {
            //TODO:  Extend this to test out the instructions with prefixes
            for (byte opCode = 0; opCode < 255; opCode++)
            {
                try
                {
                    Console.WriteLine("Trying command 0x" + opCode.ToString("X2"));
                    TryCommand(opCode);
                    Console.WriteLine("     SUCCESS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("     FAILED - " + ex.Message);
                }
            }
        }

        private static void TryCommand(byte opCode)
        {
            var machine = new Machine();
            machine.Registers.Set(Reg16.PC, 10);
            machine.Memory.Set(10, opCode);
            machine.Memory.Set(11, 0x12);
            machine.Memory.Set(12, 0x34);

            var commandRunner = new CommandRunner(machine);
            var actualResult = commandRunner.RunNextCommand();
        }
    }
}