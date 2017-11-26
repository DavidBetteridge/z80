using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using z80vm;

namespace z80.ide
{
    public partial class Screen : Form
    {
        private Memory memory;
        public Screen()
        {
            InitializeComponent();
        }

        internal void Display(Memory memory)
        {
            //0   1   0   T   T   L   L   L          Cr Cr Cr Cc Cc Cc Cc Cc
            //T – these two bits refer to which third of the screen is being addressed: 00 – Top, 01 – Middle, 10 – Bottom
            //L – these three bits indicate which line is being addressed: from 0 – 7, or 000 – 111 in binary
            //Cr – these three bits indicate which character row is being addressed: from 0 – 7
            //Cc – these five bits refer to which character column is being addressed: from 0 – 31
            //The top three bits( 010 ) of the high byte don’t change.

            var g = this.panel1.CreateGraphics();

            for (ushort address = 16384; address < 16384 + (2048 * 3); address++)
            {
                DrawAddress(memory, g, address);
            }

            for (ushort address = 22528; address < 22528 + 768; address++)
            {
                DrawColour(memory, g, address);
            }

            this.memory = memory;
        }

        private void DrawColour(Memory memory, Graphics g, ushort address)
        {
            Color LookupColour(int code, bool isBright)
            {
                switch (code)
                {
                    case 0:
                        return Color.Black;
                    case 1:
                        if (isBright)
                            return Color.LightBlue;
                        else
                            return Color.Blue;
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
                            return Color.LightYellow;
                        else
                            return Color.Yellow;
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
            var x = col * 8;

            g.FillRectangle(new SolidBrush(paperColour), x * 4, y * 4, 4, 4);
        }



        public void DrawAddress(Memory memory, ushort address)
        {
            var g = this.panel1.CreateGraphics();
            DrawAddress(memory, g, address);
        }

        private void DrawAddress(Memory memory, Graphics g, ushort address)
        {
            var third = (address & 0b0001_1000_0000_0000) >> 11;
            var line = (address & 0b0000_0111_0000_0000) >> 8;
            var row = (address & 0b0000_0000_1110_0000) >> 5;
            var col = (address & 0b0000_0000_0001_1111);

            var bitmap = memory.ReadByte(address);

            var y = (third * 64) + (row * 8) + line;

            //X
            // col is a byte from 0 to 31
            // each byte is 8 emulator pixels
            // so x = col * 8 + 
            var offset = 7;
            for (int i = 0; i < 8; i++)
            {
                var x = (col * 8) + offset;

                //Read the bitmap to see if the this pixel is set
                var setPixel = (bitmap & (1 << i)) != 0;

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
            for (ushort s = 16384; s <= 22527; s++)
            {
                this.memory.Set(s, 255);
                DrawAddress(this.memory, g, s);
            }
        }

        private void cmdClearDemo_Click(object sender, EventArgs e)
        {
            var g = this.panel1.CreateGraphics();
            for (ushort s = 16384; s <= 22527; s++)
            {
                this.memory.Set(s, 0);
                DrawAddress(this.memory, g, s);
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
            for (ushort s = 16384; s <= 22527 + 768; s++)
            {
                this.memory.Set(s, allBytes[offset]);
                offset++;
            }
            Display(this.memory);
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
