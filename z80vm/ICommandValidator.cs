using System.Runtime.CompilerServices;

namespace z80vm
{
    public interface ICommandValidator
    {
        void EnsureCommandIsValid(object operand1, object operand2, [CallerMemberName] string command = "");
    }
}