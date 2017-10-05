namespace z80vm
{
    public interface IFlagsEvaluator
    {
        void Evalulate(Flags flags, sbyte previousValue, sbyte newValue);
    }
}