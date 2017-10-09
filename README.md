# z80
A z80 virtual machine

## Example Parser

```C#
// Prepare the assembler
var assembler = new Assembler();

// Parse a single instruction,  this will return 3 bytes
//  0x32 - this represents the LD (nn), A instruction
//  0b0000_0001 - this is the higher order byte of 500
//  0b1111_0100 - this is the lower order byte of 500
var bytes = assembler.Parse("LD (500),A");
```

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