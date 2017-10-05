using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace z80vm
{
    public class CommandValidator : ICommandValidator
    {
        public void EnsureCommandIsValid(object operand1, object operand2, [System.Runtime.CompilerServices.CallerMemberName] string command = "")
        {
            var assembly = typeof(Machine).GetTypeInfo().Assembly;
            var resourceName = "z80vm.AllowedInstructions.txt";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                var lines = result.Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                command = command.ToLower() + " " + $"{operand1.ToString()},{operand2.ToString()}".ToLower();
                if (!lines.Contains(command)) throw new InvalidOperationException("Invalid operand combination - " + command);
            }
        }
    }
}
