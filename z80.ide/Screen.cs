using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using z80vm;

namespace z80.ide
{

    //    //0   1   0   T   T   L   L   L          Cr Cr Cr Cc Cc Cc Cc Cc
    //    //T – these two bits refer to which third of the screen is being addressed: 00 – Top, 01 – Middle, 10 – Bottom
    //    //L – these three bits indicate which line is being addressed: from 0 – 7, or 000 – 111 in binary
    //    //Cr – these three bits indicate which character row is being addressed: from 0 – 7
    //    //Cc – these five bits refer to which character column is being addressed: from 0 – 31
    //    //The top three bits( 010 ) of the high byte don’t change.

    public partial class Screen : Form
    {
        private const ushort SCREEN_MEMORY_START = 0x4000;
        private const ushort SCREEN_MEMORY_BITMAP_END = 0x5800;
        private const ushort SCREEN_MEMORY_END = 0x5B00;

        private readonly Memory memory;
        public Screen(Memory memory)
        {
            this.memory = memory;
            InitializeComponent();
            memory.ValueChanged += Memory_ValueChanged;
        }
        private void Memory_ValueChanged(object sender, MemoryValueChangedEventArgs e)
        {
            if (e.address >= SCREEN_MEMORY_START && e.address < SCREEN_MEMORY_BITMAP_END)
            {
                DrawAddress(memory, e.address);
            }

            if (e.address >= SCREEN_MEMORY_BITMAP_END && e.address < SCREEN_MEMORY_END)
            {
                DrawColour(memory, e.address);
            }
        }
        private static (int col, int y) XYFromAddress(ushort address)
        {
            var third = (address & 0b0001_1000_0000_0000) >> 11;
            var line = (address & 0b0000_0111_0000_0000) >> 8;
            var row = (address & 0b0000_0000_1110_0000) >> 5;
            var col = (address & 0b0000_0000_0001_1111);
            var y = (third * 64) + (row * 8) + line;

            return (col, y);
        }

        private static ushort AddressFromXY(int x, int y)
        {
            var third = (y / 64);
            y = y % 64;

            var cr = y / 8;
            var L = y % 8;

            return (ushort)(0b0100_0000_0000_0000 | (third << 11) | (L << 8) | (cr << 5) | x);
        }

        private bool isPixelSet(byte bitMap, int offset)
        {
            return (bitMap & (1 << offset)) != 0;
        }

        private void DrawColour(Memory memory, ushort address)
        {
            Color LookupColour(int code, bool isBright)
            {
                switch (code)
                {
                    case 0:
                        return Color.Black;
                    case 1:
                        if (isBright)
                            return Color.Blue;
                        else
                            return Color.DarkBlue;
                    case 2:
                        if (isBright)
                            return Color.Red;
                        else
                            return Color.DarkRed;
                    case 3:
                        if (isBright)
                            return Color.Magenta;
                        else
                            return Color.DarkMagenta;
                    case 4:
                        if (isBright)
                            return Color.LightGreen;
                        else
                            return Color.Green;
                    case 5:
                        if (isBright)
                            return Color.LightCyan;
                        else
                            return Color.Cyan;
                    case 6:
                        if (isBright)
                            return Color.Yellow;
                        else
                            return Color.GreenYellow;
                    case 7:
                        return Color.White;
                    default:
                        return Color.Black;
                }
            }

            var bitmap = memory.ReadByte(address);

            var ink = bitmap & 0b111;
            var paper = (bitmap & 0b111000) >> 3;
            var bright = ((bitmap & 0b1000000) >> 6) == 1;
            var flash = ((bitmap & 0b10000000) >> 7) == 1;

            var inkColour = LookupColour(ink, bright);
            var paperColour = LookupColour(paper, bright);

            var blockOffset = address - 22528;
            var row = blockOffset / 32;
            var col = blockOffset % 32;

            var y = row * 8;
            var x = col;
            var g = this.panel1.CreateGraphics();
            for (int y0 = 0; y0 < 8; y0++)
            {
                var bitMap = memory.ReadByte(AddressFromXY(x, y + y0));

                var offset = 7;
                for (int x0 = 0; x0 < 8; x0++)
                {
                    if (!isPixelSet(bitMap, x0))
                    {
                        g.FillRectangle(new SolidBrush(paperColour), ((x*8)+ offset) * 4, (y + y0) * 4, 4, 4);
                    }
                    else
                    {
                        g.FillRectangle(new SolidBrush(inkColour), ((x * 8) + offset) * 4, (y + y0) * 4, 4, 4);
                    }
                    offset--;
                }
            }
        }

        private void DrawAddress(Memory memory, ushort address)
        {
            var (col, y) = XYFromAddress(address);

            var g = this.panel1.CreateGraphics();
            var bitmap = memory.ReadByte(address);
            var offset = 7;
            for (int i = 0; i < 8; i++)
            {
                var x = (col * 8) + offset;

                //Read the bitmap to see if the this pixel is set
                var setPixel = isPixelSet(bitmap, i);

                var realX = x * 4;
                var realY = y * 4;

                if (setPixel)
                    g.FillRectangle(Brushes.White, realX, realY, 4, 4);
                else
                    g.FillRectangle(Brushes.Black, realX, realY, 4, 4);

                offset--;
            }
        }

        private void cmdFillDemo_Click(object sender, EventArgs e)
        {
            var g = this.panel1.CreateGraphics();
            for (ushort s = SCREEN_MEMORY_START; s < SCREEN_MEMORY_END; s++)
            {
                this.memory.Set(s, 255);
            }
        }

        private void cmdClearDemo_Click(object sender, EventArgs e)
        {
            var g = this.panel1.CreateGraphics();
            for (ushort s = SCREEN_MEMORY_START; s < SCREEN_MEMORY_END; s++)
            {
                this.memory.Set(s, 0);
            }
        }

        private void cmdLoadGloria_Click(object sender, EventArgs e)
        {
            LoadFile("gloria.scr");
        }

        private void LoadFile(string filename)
        {
            var allBytes = File.ReadAllBytes(filename);
            var offset = 0;
            for (ushort s = SCREEN_MEMORY_START; s < SCREEN_MEMORY_END; s++)
            {
                this.memory.Set(s, allBytes[offset]);
                offset++;
            }
        }

        private void cmdLoadColour_Click(object sender, EventArgs e)
        {
            LoadFile("colour.scr");
        }

        private void cmdLoadManicMinor_Click(object sender, EventArgs e)
        {
            LoadFile("ManicMiner(SoftwareProjectsLtd).scr");
        }
    }
}
