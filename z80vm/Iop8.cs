namespace z80vm
{
    public interface IOp8
    {
        Reg8? Register { get; }

        byte Read(Memory memory, Registers registers);
        void Set(Memory memory, Registers registers, byte newValue);
        string ToString();
    }
}