using System;
using System.Collections.Generic;

namespace z80vm
{
    public class Labels
    {
        private Dictionary<string, ushort> labels = new Dictionary<string, ushort>();
        public void Set(string label, ushort word)
        {
            if (this.labels.ContainsKey(label)) throw new InvalidOperationException($"The label {label} has already been defined.");
            this.labels.Add(label, word);
        }

        public ushort Read(string label)
        {
            if (this.labels.TryGetValue(label,out var word))
                return word;

            throw new InvalidOperationException($"The label {label} has not been defined.");
        }
    }
}
