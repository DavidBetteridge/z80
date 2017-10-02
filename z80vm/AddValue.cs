namespace z80vm
{
    public class AddValue
    {
        public Reg16 Register { get; set; }
        public sbyte n { get; set; }

        public AddValue(Reg16 Register, sbyte n)
        {
            this.Register = Register;
            this.n = n;
        }
    }
}
