namespace z80vm
{
    public class Value
    {
        public Reg16 Register { get; private set; }

        public static Value valueAt(Reg16 register)
        {
            return new Value() { Register = register }; 
        }
    }
}
