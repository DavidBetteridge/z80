namespace z80vm
{
    public interface IConditionValidator
    {
        bool IsTrue(Flags flags, Condition condition);
    }
}