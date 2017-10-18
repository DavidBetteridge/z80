﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using z80Assembler;
using z80vm;

namespace z80.ide
{
    public partial class Form1 : Form
    {
        private const int CURRENTLINE_MARKER = 1;
        private Machine _machine;
        private CommandRunner _commandRunner;
        private void DisplayRegisters()
        {
            void DisplayRegister(string type, string name, byte value)
            {
                var li = this.lvRegisters.Items.Add(type);
                li.SubItems.Add(name);
                li.SubItems.Add(value.ToString("X2"));
                li.SubItems.Add(value.ToString());
            }

            this.lvRegisters.Items.Clear();
            DisplayRegister("8 Bit Register", "A", _machine.Registers.Read(Reg8.A));
            DisplayRegister("8 Bit Register", "B", _machine.Registers.Read(Reg8.B));
            DisplayRegister("8 Bit Register", "C", _machine.Registers.Read(Reg8.C));
            DisplayRegister("8 Bit Register", "D", _machine.Registers.Read(Reg8.D));

        }

        private void DisplayMemory()
        {
            this.lvMemory.Items.Clear();
            for (byte address = 0; address < 100; address++)
            {
                var li = this.lvMemory.Items.Add(address.ToString("X2"));
                var v = _machine.Memory.ReadByte(address);

                li.SubItems.Add(Convert.ToString(v, 2).PadLeft(8, '0'));
                li.SubItems.Add(v.ToString("X2"));
                li.SubItems.Add(v.ToString());
            }

        }

        public void Configure()
        {
            var marker = scintilla.Markers[CURRENTLINE_MARKER];
            marker.Symbol = MarkerSymbol.Arrow;
            marker.SetBackColor(Color.Red);
            marker.SetForeColor(Color.Yellow);

            var alphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numericChars = "0123456789";
            var accentedChars = "ŠšŒœŸÿÀàÁáÂâÃãÄäÅåÆæÇçÈèÉéÊêËëÌìÍíÎîÏïÐðÑñÒòÓóÔôÕõÖØøÙùÚúÛûÜüÝýÞþßö";

            // Configuring the default style with properties
            // we have common to every lexer style saves time.
            scintilla.StyleResetDefault();
            //scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.StyleClearAll();

            // Configure the Lua lexer styles
            scintilla.Styles[Style.Asm.Default].ForeColor = Color.Silver;
            scintilla.Styles[Style.Asm.Comment].ForeColor = Color.Green;
            scintilla.Styles[Style.Asm.Number].ForeColor = Color.Olive;
            scintilla.Styles[Style.Asm.Register].ForeColor = Color.Blue;
            scintilla.Styles[Style.Asm.CpuInstruction].ForeColor = Color.BlueViolet;
            scintilla.Styles[Style.Asm.String].ForeColor = Color.Red;
            scintilla.Styles[Style.Asm.Character].ForeColor = Color.Red;
            scintilla.Styles[Style.Asm.StringEol].BackColor = Color.Pink;
            scintilla.Styles[Style.Asm.Operator].ForeColor = Color.Purple;
            scintilla.Lexer = Lexer.Asm;
            scintilla.WordChars = alphaChars + numericChars + accentedChars;

            var a = (scintilla.DescribeKeywordSets());

            // Console.WriteLine(scintilla.DescribeKeywordSets());

            //0 CPU instructions
            //1 FPU instructions
            //2 Registers
            //3 Directives
            //4 Directive operands
            //5 Extended instructions
            //6 Directives4Foldstart
            //7 Directives4Foldend

            scintilla.SetKeywords(0, "adc add and bit call ccf cp cpd cpdr cpi cpir cpl daa dec di djnz ei ex exx halt im in inc ind indr ini inir jp jr ld ldd lddr ldi ldir neg nop or otdr otir out outd outi pop push res ret reti retn rl rla rlc rlca rld rr rra rrc rrca rrd rst sbc scf set sla sll sra srl sub xor");
            scintilla.SetKeywords(2, "a b c d e f");

            // Instruct the lexer to calculate folding
            scintilla.SetProperty("fold", "1");
            scintilla.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            scintilla.Margins[2].Type = MarginType.Symbol;
            scintilla.Margins[2].Mask = Marker.MaskFolders;
            scintilla.Margins[2].Sensitive = true;
            scintilla.Margins[2].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                scintilla.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                scintilla.Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Configure folding markers with respective symbols
            scintilla.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            scintilla.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);
        }
        public Form1()
        {
            InitializeComponent();

            scintilla.TextChanged += Scintilla_TextChanged;
            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 35;
            scintilla.Lexer = Lexer.Asm;
            scintilla.LexerLanguage = "asm";
            Configure();
            scintilla.Text = @"LD A, 10
LD B, 10
ADD A, B
HALT";
            _machine = new Machine();
            DisplayRegisters();
            DisplayMemory();
        }

        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void UpdateLineNumbers()
        {
            var assembler = new Assembler();
            var nextMemoryAddress = 0;
            foreach (var line in scintilla.Lines)
            {
                line.MarginStyle = Style.LineNumber;
                line.MarginText = "0x" + nextMemoryAddress.ToString("X2");
                if (!string.IsNullOrWhiteSpace(line.Text))
                {
                    try
                    {
                        nextMemoryAddress = nextMemoryAddress + assembler.Parse(line.Text).Count;
                        scintilla.IndicatorClearRange(line.Position, line.Length);
                    }
                    catch
                    {
                        // ignored
                        scintilla.Indicators[8].Style = IndicatorStyle.Squiggle;
                        scintilla.Indicators[8].ForeColor = Color.Red;

                        // Get ready for fill
                        scintilla.IndicatorCurrent = 8;

                        // Fill ranges
                        scintilla.IndicatorFillRange(line.Position, line.Length);
                    }
                }
            }
        }

        private int _currentLineNumber;
        private void cmdRun_Click(object sender, EventArgs e)
        {
            var loader = new Loader(_machine);
            loader.LoadCommands(scintilla.Text);
            cmdStep.Enabled = true;
            _commandRunner = new CommandRunner(_machine);

            _currentLineNumber = 0;
            scintilla.Lines[_currentLineNumber].MarkerAdd(CURRENTLINE_MARKER);

        }

        private void cmdStep_Click(object sender, EventArgs e)
        {
            _commandRunner.RunNextCommand();
            DisplayRegisters();
            DisplayMemory();

            scintilla.Lines[_currentLineNumber].MarkerDelete(CURRENTLINE_MARKER);
            _currentLineNumber++;
            scintilla.Lines[_currentLineNumber].MarkerAdd(CURRENTLINE_MARKER);
        }
    }
}