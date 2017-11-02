# z80
This project contains my implementation of a z80 emulator written in C#.  I've been mainly using http://z80-heaven.wikidot.com/ as source for the definition of each of the instructions.

The solution has been split up into a number of projects:

* z80vm - this is a .net standard class library containing the processor's memory, registers and implementations of each of the commands (ADD A, B etc)

* z80Assembler - this is a .net standard class library containing
    + PARSER: this parses commands from their textual representation converts them into their hexadecimal codes
    + LOADER: stores hexadecimal commands in memory
    + COMMAND RUNNER: executes the commands stored in memory

* z80.ide - this is a .net framework windows forms application which implements a very simple debugger.

* z80.example -  this is a .net framework console application showing how to use the APIs.

* z80vm.Tests - xunit tests for the z80vm project.

* z80Assembler.Tests - xunit tests for the z80Assembler project.

## Debugger
The debugger in action

![Debugger](https://github.com/DavidBetteridge/z80/blob/master/debugger.PNG "Debugger")

## Example Program Loader

```C#
// Load the program
var loader = new Loader(machine);
loader.LoadCommands(@"
LD A, 10
LD B, 10
ADD A, B
HALT");
```

## Example Program Runner
Once a program has been loaded into memory it can executed step-by-step as follows:

```C#
var commandRunner = new CommandRunner(machine);
var halted = false;
while (!halted)
{
    halted = commandRunner.RunNextCommand();
    Console.WriteLine("Press any key to run the next command");
    Console.ReadKey(true);
}
```
**Note**

We are misusing the halt statement in the line above to mean QUIT the program.  It's actual meaning is halt the program until it we receive an interrupt.



## Example Virtual Machine

```C#
using z80vm;
using static z80vm.op8;
using static z80vm.Value;

// Prepare the machine
var conditionValidator = new ConditionValidator();
var machine = new Machine(conditionValidator);

// Load an immediate value into a register
machine.LD(Reg8.A, Read8BitValue(100));

// Load the contents of another 8bit register into a register
machine.LD(Reg8.B, Read8BitValue(Reg8.A));

// Load the contents of a memory address specified by a 16bit register (BC) into a register
machine.LD(Reg8.A, Read8BitValue(valueAt(Reg16.BC)));

// Load the contents of a memory address specified by a 16bit register plus an offset (IX+n) into a register
machine.LD(Reg8.A, Read8BitValue(valueAt(Reg16.IX).Add(10)));

// Load the contents of a memory address (ofs) into a register
machine.LD(Reg8.A, Read8BitValue(valueAt(0xFFFF)));

// Load an immediate value into a memory address specified by a 16bit register (BC)
machine.LD(valueAt(Reg16.BC), Read8BitValue(100));

// Load the contents of a register into a memory address specified by a 16bit register (BC)
machine.LD(valueAt(Reg16.BC), Read8BitValue(Reg8.A));

// Load an immediate value into a memory address specified by a 16bit register plus an offset (IX+n)
machine.LD(valueAt(Reg16.IX).Add(10), Read8BitValue(100));

// Load the contents of a register into a memory address specified by a 16bit register plus an offset (IX+n)
machine.LD(valueAt(Reg16.IX).Add(10), Read8BitValue(Reg8.A));

// Loads a memory address with the value of a register
machine.LD(0xFFFE, Read8BitValue(Reg8.A));

```