using System;

namespace z80vm
{
    public class Machine
    {
        private IConditionValidator conditionValidator;

        public Registers Registers { get; private set; }
        public Memory Memory { get; private set; }
        public Flags Flags { get; private set; }
        public Labels Labels { get; private set; }

        public Machine(IConditionValidator conditionValidator)
        {
            this.Registers = new Registers();
            this.Memory = new Memory();
            this.Flags = new Flags(this.Registers);
            this.Labels = new Labels();
            this.conditionValidator = conditionValidator;
        }

        #region LDD
        /// <summary>
        /// Usage: The instruction copies a byte from (HL) to (DE) (i. e. it does an ld (de),(hl)), then decreases both HL and DE to advance to the previous byte. It also decreases BC,
        /// Flags: Set P/V flag if BC overflows. Z and C are not altered, but H and N are set to zero.
        /// </summary>
        public void LDD()
        {
            var hl = this.Registers.Read(Reg16.HL);
            var de = this.Registers.Read(Reg16.DE);
            var bc = this.Registers.Read(Reg16.BC);

            var contentsOfHL = this.Memory.Read(hl);
            this.Memory.Set(de, contentsOfHL);

            this.Registers.Set(Reg16.HL, (ushort)(hl - 1));
            this.Registers.Set(Reg16.DE, (ushort)(de - 1));

            if (bc == 0)
            {
                this.Flags.Set(Flag.PV);
                this.Registers.Set(Reg16.BC, ushort.MaxValue);
            }
            else
            {
                this.Flags.Clear(Flag.PV);
                this.Registers.Set(Reg16.BC, (ushort)(bc - 1));
            }

            this.Flags.Clear(Flag.H);
            this.Flags.Clear(Flag.N);
        }
        #endregion

        #region LDI
        /// <summary>
        /// Usage: The instruction copies a byte from (HL) to (DE) (i. e. it does an ld (de),(hl)), then increases both HL and DE to advance to the next byte. It also decreases BC,
        /// Flags: Set P/V flag if BC overflows. Z and C are not altered, but H and N are set to zero.
        /// </summary>
        public void LDI()
        {
            var hl = this.Registers.Read(Reg16.HL);
            var de = this.Registers.Read(Reg16.DE);
            var bc = this.Registers.Read(Reg16.BC);

            var contentsOfHL = this.Memory.Read(hl);
            this.Memory.Set(de, contentsOfHL);

            this.Registers.Set(Reg16.HL, (ushort)(hl + 1));
            this.Registers.Set(Reg16.DE, (ushort)(de + 1));

            if (bc == 0)
            {
                // Decreasing BC would overflow
                this.Registers.Set(Reg16.BC, ushort.MaxValue);
                this.Flags.Set(Flag.PV);
            }
            else
            {
                this.Registers.Set(Reg16.BC, (ushort)(bc - 1));
                this.Flags.Clear(Flag.PV);
            }

            this.Flags.Clear(Flag.H);
            this.Flags.Clear(Flag.N);
        }
        #endregion

        #region DJNZ
        /// <summary>
        /// Usage: B is decreased, and a jr label happens if the result was not zero. 
        /// Flags: Not changed
        /// </summary>
        /// <param name="offset"></param>
        public void DJNZ(sbyte offset)
        {
            // Take 1 from 'b' but handling overflows
            var b = this.Registers.Read(Reg8.B);
            if (b == 0)
            {
                b = byte.MaxValue;
            }
            else
            {
                b = (byte)(b - 1);
            }
            this.Registers.Set(Reg8.B, b);

            if (b != 0)
            {
                this.JR(offset);
            }
        }
        #endregion

        #region JR

        /// <summary>
        /// Usage: Relative jumps to the address. This means that it can only jump between 128 bytes ahead or behind
        /// Flags: Not changed
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="offset"></param>
        public void JR(sbyte offset)
        {
            var currentAddress = this.Registers.Read(Reg16.PC);
            var newAddress = (ushort)(currentAddress + offset);
            this.Registers.Set(Reg16.PC, newAddress);
        }

        /// <summary>
        /// Usage: Relative jumps to the address. This means that it can only jump between 128 bytes ahead or behind
        /// Flags: Not changed
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="offset"></param>
        public void JR(Condition condition, sbyte offset)
        {
            switch (condition)
            {
                case Condition.m:
                case Condition.p:
                case Condition.pe:
                case Condition.po:
                    throw new InvalidOperationException("Only conditions c,nc,z and nz are supported");
                default:
                    break;
            }

            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.JR(offset);
            }
        }
        #endregion

        #region JP
        /// <summary>
        /// Usage: When arriving at any of these intructions, execution is immediately continued from the location given 
        /// Flags: Preserved
        /// </summary>
        /// <param name="targetMemoryAddress"></param>
        public void JP(ushort targetMemoryAddress)
        {
            this.Registers.Set(Reg16.PC, targetMemoryAddress);
        }

        /// <summary>
        /// Usage: When arriving at any of these intructions, execution is immediately continued from the location pointed to by the label
        /// Flags: Preserved 
        /// </summary>
        /// <param name="label"></param>
        public void JP(string label)
        {
            var memoryAddress = this.Labels.Read(label);
            this.JP(memoryAddress);
        }



        /// <summary>
        /// Usage: If the operand is a register reference(e.g.jp (hl)), it means that the value of the register will be loaded into PC directly
        /// Flags: Preserved 
        /// </summary>
        /// <param name="label"></param>
        public void JP(Reg16 register)
        {
            switch (register)
            {
                case Reg16.HL:
                case Reg16.IX:
                case Reg16.IY:
                    break;
                default:
                    throw new InvalidOperationException("Only the registers HL, IX, IH are valid");
            }

            var contentsOfRegister = this.Registers.Read(register);
            this.JP(contentsOfRegister);
        }

        /// <summary>
        /// Usage: When arriving at any of these intructions, execution is immediately continued from the location given 
        /// Flags: Preserved
        /// </summary>
        /// <param name="targetMemoryAddress"></param>
        public void JP(Condition condition, ushort targetMemoryAddress)
        {
            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.JP(targetMemoryAddress);
            }
        }

        /// <summary>
        /// Usage: When arriving at any of these intructions, execution is immediately continued from the location pointed to by the label
        /// Flags: Preserved 
        /// </summary>
        /// <param name="label"></param>
        public void JP(Condition condition, string label)
        {
            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.JP(label);
            }
        }
        #endregion

        #region RET
        /// <summary>
        /// Usage: ret(unconditional) or ret condition(conditional)
        /// The word on the top of the stack is retrieved and it is used as the address of the next instruction from which the 
        /// execution is to be continued.
        /// This is basically a pop pc.The conditions work the same way as above, all of them can be used. 
        /// Flags: The flags are preserved.
        /// </summary>
        public void RET()
        {
            this.POP(Reg16.PC);
        }

        public void RET(Condition condition)
        {
            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.POP(Reg16.PC);
            }
        }
        #endregion

        #region CALL

        /// <summary>
        /// Usage: Change the address of execution whilst saving the return address on the stack
        /// Flags: Preserved
        /// </summary>
        /// <param name="memoryAddress"></param>
        public void CALL(ushort memoryAddress)
        {
            var currentPC = this.Registers.Read(Reg16.PC);
            this.PUSH((ushort)(currentPC + 3));
            this.Registers.Set(Reg16.PC, memoryAddress);
        }

        /// <summary>
        /// Usage: Change the address of execution whilst saving the return address on the stack
        /// Flags: Preserved
        /// </summary>
        /// <param name="memoryAddress"></param>
        /// <param name="condition"></param>
        public void CALL(ushort memoryAddress, Condition condition)
        {
            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.CALL(memoryAddress);
            }
        }

        /// <summary>
        /// Usage: Change the address of execution whilst saving the return address on the stack
        /// Flags: Preserved
        /// </summary>
        /// <param name="label"></param>
        /// <param name="condition"></param>
        public void CALL(string label, Condition condition)
        {
            if (this.conditionValidator.IsTrue(this.Flags, condition))
            {
                this.CALL(label);
            }
        }

        /// <summary>
        /// Usage: Change the address of execution whilst saving the return address on the stack
        /// Flags: Preserved
        /// </summary>
        /// <param name="label"></param>
        public void CALL(string label)
        {
            var memoryAddress = this.Labels.Read(label);
            this.CALL(memoryAddress);
        }
        #endregion

        /// <summary>
        /// There are no operands. This instruction exchanges BC with BC’, DE with DE’ and HL with HL’ at the same time. 
        /// </summary>
        public void EXX()
        {
            void swap(Reg16 register, Reg16Shadow shadow)
            {
                var temp = this.Registers.Read(register);
                this.Registers.Set(register, this.Registers.Read(shadow));
                this.Registers.Set(shadow, temp);
            }

            swap(Reg16.BC, Reg16Shadow.BC);
            swap(Reg16.DE, Reg16Shadow.DE);
            swap(Reg16.HL, Reg16Shadow.HL);
        }

        /// <summary>
        /// Usage: Adds 2 numbers together and stores the result in the first operand
        /// Flags: Preserves the S, Z and P/V flags, and H is undefined. Rest of flags modified by definition.
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        public void ADD(Reg16 reg1, Reg16 reg2)
        {
            switch (reg1)
            {
                case Reg16.HL:
                case Reg16.IX:
                case Reg16.IY:
                    break;

                default:
                    throw new InvalidOperationException();
            }

            switch (reg2)
            {
                case Reg16.AF:
                case Reg16.PC:
                    throw new InvalidOperationException();
            }

            if ((reg1 == Reg16.HL && reg2 == Reg16.IX) ||
                (reg1 == Reg16.HL && reg2 == Reg16.IY) ||
                (reg1 == Reg16.IX && reg2 == Reg16.HL) ||
                (reg1 == Reg16.IX && reg2 == Reg16.IY) ||
                (reg1 == Reg16.IY && reg2 == Reg16.HL) ||
                (reg1 == Reg16.IY && reg2 == Reg16.IX))
            {
                throw new InvalidOperationException();
            }

            var value1 = this.Registers.Read(reg1);
            var value2 = this.Registers.Read(reg2);
            var total = (value1 + value2);

            if (total > ushort.MaxValue)
            {
                this.Flags.Set(Flag.C);
                total = total - 65536;
            }
            else
                this.Flags.Clear(Flag.C);

            this.Registers.Set(reg1, (ushort)total);
        }

        #region EX
        /// <summary>
        /// Usage: Exchanges the register AF with its shadow register
        /// Flags: Yes as this is the F register
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        public void EX(Reg16 operand1, Reg16Shadow operand2)
        {
            if (operand1 != Reg16.AF) throw new InvalidOperationException("The only register supported is AF");
            if (operand2 != Reg16Shadow.AF) throw new InvalidOperationException("The only shadow register supported is AF");

            var valueForOperand1 = this.Registers.Read(operand1);
            var valueForOperand2 = this.Registers.Read(operand2);

            this.Registers.Set(operand2, valueForOperand1);
            this.Registers.Set(operand1, valueForOperand2);
        }

        /// <summary>
        /// Usage: Exchanges the register DE with the register HL
        /// Flags: N/A
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        public void EX(Reg16 operand1, Reg16 operand2)
        {
            if (operand1 != Reg16.DE) throw new InvalidOperationException("The first operand must be DE");
            if (operand2 != Reg16.HL) throw new InvalidOperationException("The second operand must be HL");

            var valueForOperand1 = this.Registers.Read(operand1);
            var valueForOperand2 = this.Registers.Read(operand2);

            this.Registers.Set(operand2, valueForOperand1);
            this.Registers.Set(operand1, valueForOperand2);
        }


        /// Usage: Exchanges the value pointed at by SP with one of the following registers:  HL,  IX or IY
        /// Flags: N/A
        public void EX(Value operand1, Reg16 operand2)
        {
            if (operand1.Register != Reg16.SP) throw new InvalidOperationException("The first operand must be value at SP");
            if (operand2 != Reg16.HL && operand2 != Reg16.IX && operand2 != Reg16.IY) throw new InvalidOperationException("The second operand must be value at HL, IX or IY");

            // What is the current memory address?
            var memoryAddress = this.Registers.Read(operand1.Register);

            // Read the current value at that address
            var higherOrderByte = this.Memory.Read((ushort)(memoryAddress + 1));
            var lowerOrderByte = this.Memory.Read(memoryAddress);
            var valueForOperand1 = MakeWord(higherOrderByte, lowerOrderByte);

            // Read the current value from the register
            var valueForOperand2 = this.Registers.Read(operand2);
            var (h, l) = valueForOperand2.Split();

            // Swap them
            this.Registers.Set(operand2, valueForOperand1);
            this.Memory.Set((ushort)(memoryAddress + 1), h);
            this.Memory.Set(memoryAddress, l);
        }
        #endregion

        #region PUSH
        /// <summary>
        /// SP is decreased by two and the value of reg16 is copied to the memory location pointed by the new value of SP. 
        /// It does not affect the flags.
        /// </summary>
        /// <param name="register"></param>
        public void PUSH(Reg16 register)
        {
            var value = this.Registers.Read(register);
            var (highOrderByte, lowOrderByte) = value.Split();

            var stackPointer = this.Registers.Read(Reg16.SP);
            stackPointer = (ushort)(stackPointer - 2);
            this.Registers.Set(Reg16.SP, stackPointer);

            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            this.Memory.Set((ushort)(stackPointer + 1), highOrderByte);
            this.Memory.Set((ushort)(stackPointer), lowOrderByte);
        }

        /// <summary>
        /// Used internally to push a value straight onto the stack without using a registry
        /// </summary>
        /// <param name="value"></param>
        private void PUSH(ushort value)
        {
            var (highOrderByte, lowOrderByte) = value.Split();

            var stackPointer = this.Registers.Read(Reg16.SP);
            stackPointer = (ushort)(stackPointer - 2);
            this.Registers.Set(Reg16.SP, stackPointer);

            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            this.Memory.Set((ushort)(stackPointer + 1), highOrderByte);
            this.Memory.Set((ushort)(stackPointer), lowOrderByte);
        }
        #endregion

        public void LD(Reg8 register, byte value)
        {
            this.Registers.Set(register, value);
        }

        /// <summary>
        /// The value of the word found at the memory location pointed by SP is copied into reg16, then SP is increased by 2. 
        /// No flags are affected (except for the case of popping into AF).
        /// </summary>
        /// <param name="register"></param>
        public void POP(Reg16 register)
        {
            var stackPointer = this.Registers.Read(Reg16.SP);

            var highOrderByte = this.Memory.Read((ushort)(stackPointer + 1));
            var lowOrderByte = this.Memory.Read((ushort)(stackPointer));
            var fullByte = (ushort)((ushort)(highOrderByte << 8) | lowOrderByte);

            this.Registers.Set(Reg16.SP, (ushort)(stackPointer + 2));
            this.Registers.Set(register, fullByte);

        }

        /// <summary>
        /// Combines two bytes to make a word
        /// </summary>
        /// <param name="highOrderByte"></param>
        /// <param name="lowOrderByte"></param>
        /// <returns></returns>
        private ushort MakeWord(byte highOrderByte, byte lowOrderByte)
            => (ushort)(((ushort)(highOrderByte << 8)) | lowOrderByte);
    }
}
