using System;
using System.Drawing;
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
        private readonly Parser _parser;
        private void DisplayRegisters()
        {
            void DisplayRegister(string type, string name, ushort value)
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
            DisplayRegister("8 Bit Register", "E", _machine.Registers.Read(Reg8.E));
            DisplayRegister("8 Bit Register", "F", _machine.Registers.Read(Reg8.F));
            DisplayRegister("8 Bit Register", "H", _machine.Registers.Read(Reg8.H));
            DisplayRegister("8 Bit Register", "I", _machine.Registers.Read(Reg8.I));
            DisplayRegister("8 Bit Register", "IXH", _machine.Registers.Read(Reg8.IXH));
            DisplayRegister("8 Bit Register", "IXL", _machine.Registers.Read(Reg8.IXL));
            DisplayRegister("8 Bit Register", "IYH", _machine.Registers.Read(Reg8.IYH));
            DisplayRegister("8 Bit Register", "IYL", _machine.Registers.Read(Reg8.IYL));
            DisplayRegister("8 Bit Register", "L", _machine.Registers.Read(Reg8.L));
            DisplayRegister("8 Bit Register", "R", _machine.Registers.Read(Reg8.R));

            DisplayRegister("8 Bit Shadow Register", "A", _machine.Registers.Read(Reg8Shadow.A));
            DisplayRegister("8 Bit Shadow Register", "B", _machine.Registers.Read(Reg8Shadow.B));
            DisplayRegister("8 Bit Shadow Register", "C", _machine.Registers.Read(Reg8Shadow.C));
            DisplayRegister("8 Bit Shadow Register", "D", _machine.Registers.Read(Reg8Shadow.D));
            DisplayRegister("8 Bit Shadow Register", "E", _machine.Registers.Read(Reg8Shadow.E));
            DisplayRegister("8 Bit Shadow Register", "F", _machine.Registers.Read(Reg8Shadow.F));
            DisplayRegister("8 Bit Shadow Register", "H", _machine.Registers.Read(Reg8Shadow.H));
            DisplayRegister("8 Bit Shadow Register", "L", _machine.Registers.Read(Reg8Shadow.L));

            DisplayRegister("16 Bit Register", "AF", _machine.Registers.Read(Reg16.AF));
            DisplayRegister("16 Bit Register", "BC", _machine.Registers.Read(Reg16.BC));
            DisplayRegister("16 Bit Register", "DE", _machine.Registers.Read(Reg16.DE));
            DisplayRegister("16 Bit Register", "HL", _machine.Registers.Read(Reg16.HL));
            DisplayRegister("16 Bit Register", "IX", _machine.Registers.Read(Reg16.IX));
            DisplayRegister("16 Bit Register", "IY", _machine.Registers.Read(Reg16.IY));
            DisplayRegister("16 Bit Register", "PC", _machine.Registers.Read(Reg16.PC));
            DisplayRegister("16 Bit Register", "SP", _machine.Registers.Read(Reg16.SP));

            DisplayRegister("16 Bit Shadow Register", "AF", _machine.Registers.Read(Reg16Shadow.AF));
            DisplayRegister("16 Bit Shadow Register", "BC", _machine.Registers.Read(Reg16Shadow.BC));
            DisplayRegister("16 Bit Shadow Register", "DE", _machine.Registers.Read(Reg16Shadow.DE));
            DisplayRegister("16 Bit Shadow Register", "HL", _machine.Registers.Read(Reg16Shadow.HL));

            this.lvRegisters.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void DisplayMemory()
        {
            this.lvMemory.Items.Clear();
            for (byte address = 0; address < 100; address++)
            {
                var li = this.lvMemory.Items.Add("0x" + address.ToString("X2"));
                var v = _machine.Memory.ReadByte(address);

                li.SubItems.Add(Convert.ToString(v, 2).PadLeft(8, '0'));
                li.SubItems.Add(v.ToString("X2"));
                li.SubItems.Add(v.ToString());
            }

        }

        private void DisplayFlags()
        {
            lvFlags.Items.Clear();

            void Display(string name, bool value)
            {
                var li = this.lvFlags.Items.Add(name);
                li.SubItems.Add(value.ToString());
            }

            Display("Carry", _machine.Flags.Read(Flag.C));
            Display("HalfCarry", _machine.Flags.Read(Flag.H));
            Display("Subtraction", _machine.Flags.Read(Flag.N));
            Display("Parity/Overflow", _machine.Flags.Read(Flag.PV));
            Display("Sign", _machine.Flags.Read(Flag.S));
            Display("Zero", _machine.Flags.Read(Flag.Z));

            this.lvFlags.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
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
            scintilla.SetKeywords(6, "{");
            scintilla.SetKeywords(7, "}");

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
            _parser = new Parser();

            InitializeComponent();

            scintilla.TextChanged += Scintilla_TextChanged;
            scintilla.Margins[0].Type = MarginType.RightText;
            scintilla.Margins[0].Width = 35;
            scintilla.Lexer = Lexer.Asm;
            scintilla.LexerLanguage = "asm";
            Configure();
            scintilla.Text = @"LD A, 10
CALL JumpTo
LD B, 20
JumpTo: LD B, 10
ADD A, B
HALT";

            /*
             * LD A, 0
LD B, 10
Loop: ADD A, B
DJNZ Loop
             * 
             * 
             */

            _machine = new Machine();
            DisplayRegisters();
            DisplayMemory();
            DisplayFlags();

            var screen = new Screen();
            screen.Show();
            screen.Display(_machine.Memory);
            screen.BringToFront();
        }

        private void Scintilla_TextChanged(object sender, EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void UpdateLineNumbers()
        {
            if (string.IsNullOrWhiteSpace(scintilla.Text)) return;

            var commands = _parser.Parse(0, scintilla.Text);

            var l = 0;
            foreach (var line in scintilla.Lines)
            {
                if (!string.IsNullOrWhiteSpace(line.Text))
                {
                    var command = commands[l];
                    l++;

                    line.MarginStyle = Style.LineNumber;
                    line.MarginText = "0x" + command.MemoryLocation.ToString("X2");
                    if (command.IsInValid)
                    {
                        // ignored
                        scintilla.Indicators[8].Style = IndicatorStyle.Squiggle;
                        scintilla.Indicators[8].ForeColor = Color.Red;

                        // Get ready for fill
                        scintilla.IndicatorCurrent = 8;

                        // Fill ranges
                        scintilla.IndicatorFillRange(line.Position, line.Length);
                    }
                    else
                    {
                        scintilla.IndicatorClearRange(line.Position, line.Length);
                    }
                }
            }
        }

        private Line _currentLine;

        private void cmdRun_Click(object sender, EventArgs e)
        {
            _machine = new Machine();
            var commands = _parser.Parse(0, scintilla.Text);

            var loader = new Loader(_machine);
            loader.LoadCommands(commands);
            cmdStep.Enabled = true;

            _commandRunner = new CommandRunner(_machine);
            Display();
        }

        private void cmdStep_Click(object sender, EventArgs e)
        {
            _commandRunner.RunNextCommand();
            Display();

        }

        private void Display()
        {
            DisplayRegisters();
            DisplayMemory();
            DisplayFlags();

            if (_currentLine != null)
            {
                // Remove the marker from the previous line
                this._currentLine.MarkerDelete(CURRENTLINE_MARKER);
            }

            // Find the line with the same memory address as the program counter
            var pc = _machine.Registers.Read(Reg16.PC);
            foreach (var line in scintilla.Lines)
            {
                if (line.MarginText == "0x" + pc.ToString("X2"))
                {
                    line.MarkerAdd(CURRENTLINE_MARKER);
                    _currentLine = line;
                    break;
                }
            }
        }
    }
}
