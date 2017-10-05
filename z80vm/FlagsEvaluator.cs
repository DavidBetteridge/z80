namespace z80vm
{
    public class FlagsEvaluator : IFlagsEvaluator
    {
        public void Evalulate(Flags flags, sbyte previousValue, sbyte newValue)
        {
            // Sign Flag - Set if the 2-complement value is negative (copy of MSB) 
            if (newValue < 0)
                flags.Set(Flag.S);
            else
                flags.Clear(Flag.S);

            // Zero Flag - Set if the value is zero 
            if (newValue == 0)
                flags.Set(Flag.Z);
            else
                flags.Clear(Flag.Z);

            // V - Overflow set if the 2-complement result does not fit in the register
            if (previousValue < 0 && newValue >= 0)
                flags.Set(Flag.PV);
            else if (previousValue >= 0 && newValue < 0)
                flags.Set(Flag.PV);
            else
                flags.Clear(Flag.PV);

            // Carry - Set if the result did not fit in the register 
            if (previousValue < 0 && newValue > 0)
                flags.Set(Flag.C);
            else if (previousValue > 0 && newValue < 0)
                flags.Set(Flag.C);
            else
                flags.Clear(Flag.C);

        }
    }
}
