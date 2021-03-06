﻿using System;

namespace z80vm
{
    public class Machine
    {
        private IConditionValidator conditionValidator;
        private IFlagsEvaluator flagsEvaluator;
        private ICommandValidator commandValidator;



        public Registers Registers { get; private set; }

        public Memory Memory { get; private set; }



        public Flags Flags { get; private set; }

        #region RLA

        /// <summary>
        /// Usage: Performs an RL A, but is much faster and S, Z, and P/V flags are preserved.
        /// Flags: C is changed to the leaving 7th bit, H and N are reset, P/V , S and Z are preserved.
        /// </summary>
        public void RLA()
        {
            var currentValue = Registers.Read(Reg8.A);
            var newValue = RL_RotateLeft(currentValue);

            Registers.Set(Reg8.A, newValue);
        }
        #endregion

        #region RL
        /// <summary>
        /// Usage: 9-bit rotation to the left. the register's bits are shifted left. The carry value is put into 0th bit of the register, and the leaving 7th bit is put into the carry.
        /// Flags: C is changed to the leaving 7th bit, H and N are reset, P/V is parity, S and Z are modified by definition
        /// </summary>
        /// <param name="register"></param>
        public void RL(Reg8 register)
        {
            var currentValue = Registers.Read(register);
            var newValue = RL_RotateLeft(currentValue);

            Registers.Set(register, newValue);
        }

        /// <summary>
        /// Usage: 9-bit rotation to the left. the register's bits are shifted left. The carry value is put into 0th bit of the register, and the leaving 7th bit is put into the carry.
        /// Flags: C is changed to the leaving 7th bit, H and N are reset, P/V is parity, S and Z are modified by definition
        /// </summary>
        public void RL(Value value)
        {
            var memoryLocation = (ushort)(Registers.Read(value.Register) + value.Offset);
            var currentValue = Memory.ReadByte(memoryLocation);

            var newValue = RL_RotateLeft(currentValue);

            Memory.Set(memoryLocation, newValue);
        }

        private byte RL_RotateLeft(byte currentValue)
        {
            var carrySet = Flags.Read(Flag.C);
            var newValue = (byte)(currentValue << 1);
            if (carrySet) newValue |= 1;

            if ((currentValue >> 7) == 1)
            {
                Flags.Set(Flag.C);
            }
            else
            {
                Flags.Clear(Flag.C);
            }

            return newValue;
        }
        #endregion

        #region RRCA
        /// <summary>
        /// Usage: Performs a RRC A faster and modifies the flags differently.
        /// Flags: The carry becomes the value leaving on the right, H and N are reset, P/V, S, and Z are preserved
        /// </summary>
        public void RRCA()
        {
            var currentValue = Registers.Read(Reg8.A);
            var newValue = (byte)((currentValue >> 1) | (currentValue << 7));
            Registers.Set(Reg8.A, newValue);

            if ((currentValue & 0b0000_0001) == 1)
            {
                Flags.Set(Flag.C);
            }
            else
            {
                Flags.Clear(Flag.C);

            }

            Flags.Clear(Flag.H);
            Flags.Clear(Flag.N);
        }
        #endregion

        #region RRC

        /// <summary>
        /// Usage: 8-bit rotation to the right. the bit leaving on the right is copied into the carry, and into bit 7.
        /// Flags: The carry becomes the value leaving on the right, H and N are reset, P/V is parity, and S and Z are modified by definition.
        /// </summary>
        /// <param name="register"></param>
        public void RRC(Reg8 register)
        {
            var currentValue = Registers.Read(register);
            var newValue = (byte)((currentValue >> 1) | (currentValue << 7));
            Registers.Set(register, newValue);
        }

        /// <summary>
        /// Usage: 8-bit rotation to the right. the bit leaving on the right is copied into the carry, and into bit 7.
        /// Flags: The carry becomes the value leaving on the right, H and N are reset, P/V is parity, and S and Z are modified by definition.
        /// </summary>
        /// <param name="register"></param>
        public void RRC(Value value)
        {
            var memoryLocation = (ushort)(Registers.Read(value.Register) + value.Offset);
            var currentValue = Memory.ReadByte(memoryLocation);
            var newValue = (byte)((currentValue >> 1) | (currentValue << 7));
            Memory.Set(memoryLocation, newValue);
        }
        #endregion

        #region RLCA
        /// <summary>
        /// Usage: Performs RLC A much quicker, and modifies the flags differently
        /// Flags: S,Z, and P/V are preserved, H and N flags are reset
        /// </summary>
        public void RLCA()
        {
            var currentValue = Registers.Read(Reg8.A);
            var newValue = (byte)((currentValue << 1) | (currentValue >> 7));
            Registers.Set(Reg8.A, newValue);
            Flags.Clear(Flag.H);
            Flags.Clear(Flag.N);
        }
        #endregion

        #region RLC
        /// <summary>
        /// Usage: 8-bit rotation to the left. The bit leaving on the left is copied into the carry, and to bit 0.
        /// Flags: H and N flags are reset, P/V is parity, S and Z are modified by definition.
        /// </summary>
        /// <param name="register"></param>
        public void RLC(Reg8 register)
        {
            var currentValue = Registers.Read(register);
            var newValue = (byte)((currentValue << 1) | (currentValue >> 7));
            Registers.Set(register, newValue);
        }

        /// <summary>
        /// Usage: 8-bit rotation to the left. The bit leaving on the left is copied into the carry, and to bit 0.
        /// Flags: H and N flags are reset, P/V is parity, S and Z are modified by definition.
        /// </summary>
        /// <param name="register"></param>
        public void RLC(Value value)
        {
            var memoryLocation = (ushort)(Registers.Read(value.Register) + value.Offset);
            var currentValue = Memory.ReadByte(memoryLocation);
            var newValue = (byte)((currentValue << 1) | (currentValue >> 7));
            Memory.Set(memoryLocation, newValue);
        }
        #endregion

        #region NOP
        /// <summary>
        /// Usage: Wastes time
        /// Flags: None
        /// </summary>
        public void NOP()
        {
            // Should waste 4 clock cycles here.
        }

        #endregion

        #region HALT

        /// <summary>
        /// Usage: Waits for the next interupt - crashes if interupts are disabled
        /// Flags: None
        /// </summary>
        public void HALT()
        {
            // Does nothing,  waits for the next interupt
        }
        #endregion

        #region DAA
        /// <summary>
        /// Usage: When this instruction is executed, the A register is BCD corrected using the contents of the flags. The exact process is the following: if the least significant four bits of A contain a non-BCD digit (i. e. it is greater than 9) or the H flag is set, then $06 is added to the register. Then the four most significant bits are checked. If this more significant digit also happens to be greater than 9 or the C flag is set, then $60 is added. If this second addition was needed, the C flag is set after execution, otherwise it is reset.
        /// Flags: The N flag is preserved, P/V is parity and the others are altered by definition.
        /// </summary>
        public void DAA()
        {
            var currentValue = this.Registers.Read(Reg8.A);
            if (currentValue > 9)
                currentValue = (byte)(currentValue + 6);
            this.Registers.Set(Reg8.A, currentValue);

        }

        #endregion

        #region MyRegion
        /// <summary>
        /// Usage: The value in A is multiplied by -1 (two’s complement). 
        /// Flags: The N flag is set, P/V is interpreted as overflow. The rest of the flags is modified by definition.
        /// </summary>
        public void NEG()
        {
            var currentValue = (sbyte)this.Registers.Read(Reg8.A);
            var newValue = (sbyte)(currentValue * -1);
            this.Registers.Set(Reg8.A, (byte)newValue);
        }
        #endregion

        #region DEC
        /// <summary>
        /// Usage: Decrements the value of the operand by one.
        /// Flags: 8-bit decrements preserve the C flag, set N, treat P/V as overflow and modify the others by definition. 
        /// </summary>
        /// <param name="operand"></param>
        public void DEC(IOp8 operand)
        {
            this.commandValidator.EnsureCommandIsValid(operand);

            var currentValue = operand.Read(this.Memory, this.Registers);
            var newValue = (byte)(currentValue - 1);
            operand.Set(this.Memory, this.Registers, newValue);
        }

        /// <summary>
        /// Usage: Decrements the value of the operand by one.
        /// Flags: 16-bit decrements do not alter any of the flags.
        /// </summary>
        /// <param name="operand"></param>
        public void DEC(Reg16 operand)
        {
            this.commandValidator.EnsureCommandIsValid(operand);

            var currentValue = this.Registers.Read(operand);
            var newValue = (byte)(currentValue - 1);
            this.Registers.Set(operand, newValue);
        }

        #endregion

        /// <summary>
        /// Override the command validator
        /// </summary>
        /// <param name="commandValidator"></param>
        public void SetCommandValidator(ICommandValidator commandValidator)
        {
            this.commandValidator = commandValidator;
        }

        /// <summary>
        /// Override the condition validator
        /// </summary>
        /// <param name="conditionValidator"></param>
        public void SetConditionValidator(IConditionValidator conditionValidator)
        {
            this.conditionValidator = conditionValidator;
        }

        /// <summary>
        /// Override the flags evaluator
        /// </summary>
        /// <param name="conditionValidator"></param>
        public void SetFlagsEvaluator(IFlagsEvaluator flagsEvaluator)
        {
            this.flagsEvaluator = flagsEvaluator;
        }


        public Machine()
        {
            this.Registers = new Registers();
            this.Memory = new Memory();
            this.Flags = new Flags(this.Registers);
            this.conditionValidator = new ConditionValidator();
            this.flagsEvaluator = new FlagsEvaluator();
            this.commandValidator = new CommandValidator();
        }

        #region INC
        /// <summary>
        /// Usage: Increments the value of the operand by one. 
        /// Flags: 8-bit increments preserve the C flag, reset N, treat P/V as overflow and modify the others by definition
        /// </summary>
        /// <param name="op8"></param>
        public void INC(IOp8 op8)
        {
            this.commandValidator.EnsureCommandIsValid(op8);

            var currentValue = op8.Read(this.Memory, this.Registers);
            var newValue = (byte)(currentValue + 1);
            op8.Set(this.Memory, this.Registers, newValue);
        }

        /// <summary>
        /// Usage: Increments the value of the operand by one.
        /// Flags: No flag are altered.
        /// </summary>
        /// <param name="register"></param>
        public void INC(Reg16 register)
        {
            var currentValue = this.Registers.Read(register);
            var newValue = (ushort)(currentValue + 1);
            this.Registers.Set(register, newValue);
        }
        #endregion

        #region SUB
        /// <summary>
        /// Usage: The value of the operand is subtracted from A, and the result is also written back to 
        /// Flags: The N flag is set, P/V is interpreted as overflow. The rest of the flags is modified by definition.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="op8"></param>
        public void SUB(Reg8 a, op8 op8)
        {
            var currentValue = this.Registers.Read(Reg8.A);
            var valueToSubtract = op8.Read(this.Memory, this.Registers);
            var newValue = (byte)(currentValue - valueToSubtract);
            this.Registers.Set(Reg8.A, newValue);

            this.flagsEvaluator.Evalulate(this.Flags, (sbyte)currentValue, (sbyte)newValue);

            this.Flags.Set(Flag.N);
        }
        #endregion

        #region LDDR
        /// <summary>
        /// This is an ldi repeated until BC reaches zero. ie This single instruction copies BC bytes from below (HL) to (DE), decreases both HL and DE by BC, and sets BC to zero.
        /// Flags: the P/V flag holds zero after leaving the instruction
        /// </summary>
        public void LDDR()
        {
            var hl = this.Registers.Read(Reg16.HL);
            var bc = this.Registers.Read(Reg16.BC);
            var de = this.Registers.Read(Reg16.DE);

            for (int i = 0; i < bc; i++)
            {
                var contentsOfHL = this.Memory.ReadByte((ushort)(hl - i));
                this.Memory.Set((ushort)(de - i), contentsOfHL);
            }

            this.Registers.Set(Reg16.BC, 0);
            this.Registers.Set(Reg16.HL, (ushort)(hl - bc));
            this.Registers.Set(Reg16.DE, (ushort)(de - bc));

            this.Flags.Clear(Flag.H);
            this.Flags.Clear(Flag.N);
            this.Flags.Clear(Flag.PV);
        }



        #endregion

        #region LDIR
        /// <summary>
        /// This is an ldi repeated until BC reaches zero. ie This single instruction copies BC bytes from (HL) to (DE), increases both HL and DE by BC, and sets BC to zero.
        /// Flags: the P/V flag holds zero after leaving the instruction
        /// </summary>
        public void LDIR()
        {
            var hl = this.Registers.Read(Reg16.HL);
            var bc = this.Registers.Read(Reg16.BC);
            var de = this.Registers.Read(Reg16.DE);

            for (int i = 0; i < bc; i++)
            {
                var contentsOfHL = this.Memory.ReadByte((ushort)(hl + i));
                this.Memory.Set((ushort)(de + i), contentsOfHL);
            }

            this.Registers.Set(Reg16.BC, 0);
            this.Registers.Set(Reg16.HL, (ushort)(hl + bc));
            this.Registers.Set(Reg16.DE, (ushort)(de + bc));

            this.Flags.Clear(Flag.H);
            this.Flags.Clear(Flag.N);
            this.Flags.Clear(Flag.PV);
        }




        #endregion

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

            var contentsOfHL = this.Memory.ReadByte(hl);
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

            var contentsOfHL = this.Memory.ReadByte(hl);
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

        #region ADC
        /// <summary>
        /// Usage: The sum of the two operands plus the carry flag (0 or 1) is calculated, and the result is written back into the first operand.
        /// Flags: The N flag is reset, P/V is interpreted as overflow. The rest of the flags is modified by definition. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="op8"></param>
        public void ADC(Reg8 a, op8 op8)
        {
            this.commandValidator.EnsureCommandIsValid(a, op8);

            var currentValue = this.Registers.Read(Reg8.A);
            var valueToAdd = op8.Read(this.Memory, this.Registers);
            var newValue = (byte)(currentValue + valueToAdd);

            if (this.Flags.Read(Flag.C))
                newValue++;

            this.Registers.Set(Reg8.A, newValue);

            this.flagsEvaluator.Evalulate(this.Flags, (sbyte)currentValue, (sbyte)newValue);

            this.Flags.Clear(Flag.N);
        }



        /// <summary>
        /// Usage: The sum of the two operands plus the carry flag (0 or 1) is calculated, and the result is written back into the first operand.
        /// Flags: The N flag is reset, P/V is interpreted as overflow. The rest of the flags is modified by definition. The H flag is undefined.
        /// </summary>
        /// <param name="operand1"></param>
        /// <param name="operand2"></param>
        public void ADC(Reg16 operand1, Reg16 operand2)
        {
            this.commandValidator.EnsureCommandIsValid(operand1, operand2);

            var currentValue = this.Registers.Read(operand1);
            var valueToAdd = this.Registers.Read(operand2);
            var newValue = (ushort)(currentValue + valueToAdd);

            if (this.Flags.Read(Flag.C))
                newValue++;

            this.Registers.Set(operand1, newValue);
        }


        #endregion

        #region ADD

        /// <summary>
        /// Usage: Adds 2 numbers together and stores the result in the first operand
        /// Flags: The N flag is reset, P/V is interpreted as overflow. The rest of the flags is modified by definition.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="op8"></param>
        public void ADD(Reg8 a, op8 op8)
        {
            this.commandValidator.EnsureCommandIsValid(a, op8);

            var currentValue = this.Registers.Read(a);
            var valueToAdd = op8.Read(this.Memory, this.Registers);
            var newTotal = (byte)(currentValue + valueToAdd);
            this.Registers.Set(Reg8.A, newTotal);

            this.flagsEvaluator.Evalulate(this.Flags, (sbyte)currentValue, (sbyte)newTotal);

            //It is set when there is a carry transfer from bit 3 to bit 4,
            if ((byte)(currentValue & 0b1111) + (byte)(valueToAdd & 0b1111) > 0b1111)
                this.Flags.Set(Flag.H);
            else
                this.Flags.Clear(Flag.H);

            //indicates subtraction
            this.Flags.Clear(Flag.N);
        }

        /// <summary>
        /// Usage: Adds 2 numbers together and stores the result in the first operand
        /// Flags: Preserves the S, Z and P/V flags, and H is undefined. Rest of flags modified by definition.
        /// </summary>
        /// <param name="reg1"></param>
        /// <param name="reg2"></param>
        public void ADD(Reg16 reg1, Reg16 reg2)
        {
            this.commandValidator.EnsureCommandIsValid(reg1, reg2);

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
        #endregion

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
            var valueForOperand1 = this.Memory.ReadWord(memoryAddress);

            // Read the current value from the register
            var valueForOperand2 = this.Registers.Read(operand2);

            // Swap them
            this.Registers.Set(operand2, valueForOperand1);
            this.Memory.Set(memoryAddress, valueForOperand2);
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
            var stackPointer = this.Registers.Read(Reg16.SP);
            stackPointer = (ushort)(stackPointer - 2);
            this.Registers.Set(Reg16.SP, stackPointer);

            this.Memory.Set((ushort)(stackPointer), value);
        }

        /// <summary>
        /// Used internally to push a value straight onto the stack without using a registry
        /// </summary>
        /// <param name="value"></param>
        private void PUSH(ushort value)
        {
            var stackPointer = this.Registers.Read(Reg16.SP);
            stackPointer = (ushort)(stackPointer - 2);
            this.Registers.Set(Reg16.SP, stackPointer);
            this.Memory.Set((ushort)(stackPointer), value);
        }
        #endregion

        #region LD

        public void LD(Reg8 target, op8 source)
        {
            var value = source.Read(this.Memory, this.Registers);
            this.Registers.Set(target, value);

            if (source.Register.HasValue && (source.Register == Reg8.I || source.Register == Reg8.R))
            {
                this.Flags.Clear(Flag.H);
                this.Flags.Clear(Flag.N);

                if (value == 0)
                {
                    this.Flags.Set(Flag.Z);
                }
                else
                {
                    this.Flags.Clear(Flag.Z);
                }

                if (value.ReadBit(7))
                {
                    // MSB is set,  so the value is negative.  Clear the sign bit
                    this.Flags.Clear(Flag.S);
                }
                else
                {
                    // MSB isn't set,  so the value is positive.  Set the sign bit
                    this.Flags.Set(Flag.S);
                }
            }
        }

        /// <summary>
        /// Usage: Loads a 16 bit register with the contents of another 16bit register
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public void LD(Reg16 target, Reg16 source)
        {
            var value = this.Registers.Read(source);
            this.Registers.Set(target, value);
        }

        /// <summary>
        /// Usage: Loads the 16bit register with the contents of the memory location
        /// Flags: Not changed
        /// </summary>
        /// <param name="target"></param>
        /// <param name="valueInMemoryAddress"></param>
        public void LD(Reg16 target, MemoryAddress valueInMemoryAddress)
        {
            this.Registers.Set(target, this.Memory.ReadByte(valueInMemoryAddress.MemoryLocation));
        }

        /// <summary>
        /// Loads a 16BIT registry with an immediate value
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        public void LD(Reg16 target, ushort value)
        {
            this.Registers.Set(target, value);
        }

        public void LD(Value operand1, op8 op8)
        {
            var memoryAddress = (ushort)(this.Registers.Read(operand1.Register) + operand1.Offset);
            this.Memory.Set(memoryAddress, op8.Read(this.Memory, this.Registers));
        }

        public void LD(Reg8 register, Value operand2)
        {
            var memoryAddress = (ushort)(this.Registers.Read(operand2.Register) + operand2.Offset);
            this.Registers.Set(register, this.Memory.ReadByte(memoryAddress));
        }

        /// <summary>
        /// Usage: Loads the value of the registry into the memory location
        /// Flags: Not changed
        /// </summary>
        /// <param name="valueInMemoryAddress"></param>
        /// <param name="source"></param>
        public void LD(MemoryAddress targetMemoryAddress, Reg16 source)
        {
            var value = this.Registers.Read(source);
            this.Memory.Set(targetMemoryAddress.MemoryLocation, value);
        }

        public void LD(ushort memoryAddress, op8 op8)
        {
            this.Memory.Set(memoryAddress, op8.Read(this.Memory, this.Registers));
        }

        #endregion

        /// <summary>
        /// The value of the word found at the memory location pointed by SP is copied into reg16, then SP is increased by 2. 
        /// No flags are affected (except for the case of popping into AF).
        /// </summary>
        /// <param name="register"></param>
        public void POP(Reg16 register)
        {
            var stackPointer = this.Registers.Read(Reg16.SP);
            var fullByte = this.Memory.ReadWord(stackPointer);

            this.Registers.Set(Reg16.SP, (ushort)(stackPointer + 2));
            this.Registers.Set(register, fullByte);
        }
    }
}
