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
        private Dictionary<string, InstructionLookup> dds;
        private Dictionary<string, InstructionLookup> cbs;
        private Dictionary<string, InstructionLookup> eds;
        private Dictionary<string, InstructionLookup> ddcbs;

        public void Load()
        {
            var assembly = typeof(InstructionLookups).GetTypeInfo().Assembly;
            var resourceName = "z80Assembler.z80Instructions.txt";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();

                var lines = result
                            .Split(new[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                            .Skip(3)
                            .Take(256)
                            .Select(l => new InstructionLookup(l));

                this.normals = lines.ToDictionary(a => a.Normal);
                this.dds = lines.ToDictionary(a => a.DDPrefix);
                this.cbs = lines.ToDictionary(a => a.CBPrefix);

                // This column contains a number of repeated commands,  hence the need for a manual loop
                this.eds = new Dictionary<string, InstructionLookup>();
                foreach (var op in lines)
                {
                    if (!this.eds.ContainsKey(op.EDPrefix))
                        this.eds.Add(op.EDPrefix, op);
                }

                this.ddcbs = new Dictionary<string, InstructionLookup>();
                foreach (var op in lines)
                {
                    if (!this.ddcbs.ContainsKey(op.DDCBPrefix))
                        this.ddcbs.Add(op.DDCBPrefix, op);
                }

                numberOfLines = normals.Count();
            }
        }

        public int LookupHexCodeFromNormalisedCommand(string normal)
        {
            if (normals.ContainsKey(normal))
                return normals[normal].Hex;

            if (dds.ContainsKey(normal))
                return (int)(0xDD00 | dds[normal].Hex);

            if (cbs.ContainsKey(normal))
                return (int)(0xCB00 | cbs[normal].Hex);

            if (eds.ContainsKey(normal))
                return (int)(0xED00 | eds[normal].Hex);

            return (int)(0xDDCB00 | ddcbs[normal].Hex);
        }

        public int Count()
        {
            return numberOfLines;
        }


    }
}
