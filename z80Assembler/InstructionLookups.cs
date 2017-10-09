using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace z80Assembler
{
    public class InstructionLookups
    {
        private int numberOfLines = 0;
        private Dictionary<string, InstructionLookup> normals;

        public void Load()
        {
            var assembly = typeof(InstructionLookups).GetTypeInfo().Assembly;
            var resourceName = "z80Assembler.z80Instructions.txt";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                normals = result
                            .Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .Skip(3)
                            .Take(256)
                            .Select(l => new InstructionLookup(l))
                            .ToDictionary(a => a.Normal);
                numberOfLines = normals.Count();
            }
        }

        public byte LookupHexCodeFromNormal(string normal)
        {
            return normals[normal].Hex;
        }

        public int Count()
        {
            return numberOfLines;
        }


    }
}
