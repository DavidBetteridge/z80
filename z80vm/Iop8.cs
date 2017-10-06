namespace z80vm
{
    public interface Iop8
    {
        Reg8? Register { get; }
        Value Value { get; }

        byte Read(Memory memory, Registers registers);
        void Set(Memory memory, Registers registers, byte newValue);
        string ToString();
    }
}