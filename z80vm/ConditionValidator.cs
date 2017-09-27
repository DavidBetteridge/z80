using System;

namespace z80vm
{
    public class ConditionValidator : IConditionValidator
    {
        public bool IsTrue(Flags flags, Condition condition)
        {
            switch (condition)
            {
                case Condition.c:
                    return flags.Read(Flag.C);
                case Condition.nc:
                    return !flags.Read(Flag.C);
                case Condition.z:
                    return flags.Read(Flag.Z);
                case Condition.nz:
                    return !flags.Read(Flag.Z);
                case Condition.m:
                    return flags.Read(Flag.S);
                case Condition.p:
                    return !flags.Read(Flag.S);
                case Condition.pe:
                    return flags.Read(Flag.PV);
                case Condition.po:
                    return !flags.Read(Flag.PV);
                default:
                    throw new Exception("Unknown condition - " + condition);
            }
        }
    }
}
