﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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

        private Dictionary<byte, InstructionLookup> byHexCode;

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
                            .Select(l => new InstructionLookup(l))
                            .ToArray();

                this.normals = lines.ToDictionary(a => a.Normal.Replace("nn", "n"));
                this.dds = lines.ToDictionary(a => a.DDPrefix.Replace("nn", "n"));
                this.cbs = lines.ToDictionary(a => a.CBPrefix.Replace("nn", "n"));

                // This column contains a number of repeated commands,  hence the need for a manual loop
                this.eds = new Dictionary<string, InstructionLookup>();
                foreach (var op in lines)
                {
                    if (!this.eds.ContainsKey(op.EDPrefix.Replace("nn", "n")))
                        this.eds.Add(op.EDPrefix.Replace("nn", "n"), op);
                }

                this.ddcbs = new Dictionary<string, InstructionLookup>();
                foreach (var op in lines)
                {
                    if (!this.ddcbs.ContainsKey(op.DDCBPrefix.Replace("nn", "n")))
                        this.ddcbs.Add(op.DDCBPrefix.Replace("nn", "n"), op);
                }

                this.byHexCode = lines.ToDictionary(a => a.Hex);

                numberOfLines = normals.Count();
            }
        }

        public InstructionInfo TryLookupHexCodeFromNormalisedCommand(string normal)
        {
            for (int i = 0; i < 2; i++)
            {
                if (normals.ContainsKey(normal))
                {
                    return new InstructionInfo(normals[normal].Hex, this);
                }

                if (dds.ContainsKey(normal))
                {
                    return new InstructionInfo(0xDD00 | dds[normal].Hex, this);
                }

                if (cbs.ContainsKey(normal))
                {
                    return new InstructionInfo(0xCB00 | cbs[normal].Hex, this);
                }

                if (eds.ContainsKey(normal))
                {
                    return new InstructionInfo(0xED00 | eds[normal].Hex, this);
                }

                if (ddcbs.ContainsKey(normal))
                {
                    return new InstructionInfo(0xDDCB00 | ddcbs[normal].Hex, this);
                }

                // The command could except a ushort but we have only been given a single byte
                // so we need to check again.
                normal = normal.Replace("n", "nn");
            }
            return null;
        }

        public int Count()
        {
            return numberOfLines;
        }

        public string LookupCommandFromHexCode(int hexCode)
        {
            if (hexCode.Second() == 0xDD && hexCode.Third() == 0xCB)
            {
                return byHexCode[hexCode.Final()].DDCBPrefix;
            }

            switch (hexCode.Third())
            {
                case 0xDD:
                    return byHexCode[hexCode.Final()].DDPrefix;

                case 0xCB:
                    return byHexCode[hexCode.Final()].CBPrefix;

                case 0xED:
                    return byHexCode[hexCode.Final()].EDPrefix;

                default:
                    return byHexCode[hexCode.Final()].Normal;
            }
        }


    }
}
