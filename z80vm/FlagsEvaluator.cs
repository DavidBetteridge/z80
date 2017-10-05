namespace z80vm
{
    public class FlagsEvaluator : IFlagsEvaluator
    {
        public void Evalulate(Flags flags, sbyte previousValue, sbyte newValue)
        {
            if (previousValue < 0 && newValue >= 0)
                flags.Set(Flag.S);
            else
                flags.Clear(Flag.S);

            if (newValue == 0)
                flags.Set(Flag.Z);
            else
                flags.Clear(Flag.Z);

            if (previousValue < 0 && newValue >= 0)
                flags.Set(Flag.PV);
            else if (previousValue >= 0 && newValue < 0)
                flags.Set(Flag.PV);
            else
                flags.Clear(Flag.PV);

            if (previousValue < 0 && newValue > 0)
                flags.Set(Flag.C);
            else if (previousValue > 0 && newValue < 0)
                flags.Set(Flag.C);
            else
                flags.Clear(Flag.C);

        }
    }
}
