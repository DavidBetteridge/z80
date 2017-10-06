using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace z80vm
{
    /// <summary>
    /// A CommandValidator checks that the operands provided for a command are valid for that instruction.
    /// For example,  when adding an 8bit number,  the only valid value for the first operand is Reg8.A
    /// Instead of filling the source code with a lot of IF/SWITCH statements we stored the list of valid
    /// commands in an embedded text file called AllowedInstructions.txt
    /// </summary>
    public class CommandValidator : ICommandValidator
    {
        private HashSet<string> lines;

        /// <summary>
        /// Checks that the command is valid with these two operands.  Throws an exception if not
        /// </summary>
        /// <param name="operand1">The value of the first parameter</param>
        /// <param name="operand2">The value of the second parameter</param>
        /// <param name="command">The name of the command,  or the calling method is not supplied.  For example SUB</param>
        public void EnsureCommandIsValid(object operand1, object operand2, [System.Runtime.CompilerServices.CallerMemberName] string command = "")
        {
            if (lines == null)
            {
                var assembly = typeof(Machine).GetTypeInfo().Assembly;
                var resourceName = "z80vm.AllowedInstructions.txt";
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    var linesInFile = result
                                        .Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(l => l.Trim())
                                        .Where(l => !string.IsNullOrWhiteSpace(l));
                    this.lines = new HashSet<string>(linesInFile);
                }
            }

            command = command.ToLower() + " " + $"{operand1.ToString()},{operand2.ToString()}".ToLower();
            if (!lines.Contains(command)) throw new InvalidOperationException("Invalid operand combination - " + command);
        }
    }
}
