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
        private readonly Bitmap bitmap;
        private readonly Bitmap flashBitmap;
        public Screen(Memory memory)
        {
            this.memory = memory;
            InitializeComponent();

            this.bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            this.flashBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

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

        /// <summary>
        /// Calculates the x and y corrds of a pixel from it's memory address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static (int col, int y) XYFromAddress(ushort address)
        {
            var third = (address & 0b0001_1000_0000_0000) >> 11;
            var line = (address & 0b0000_0111_0000_0000) >> 8;
            var row = (address & 0b0000_0000_1110_0000) >> 5;
            var col = (address & 0b0000_0000_0001_1111);
            var y = (third * 64) + (row * 8) + line;

            return (col, y);
        }

        /// <summary>
        /// Calculates the bitmap address from a pixel's x and y corrds.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static ushort AddressFromXY(int x, int y)
        {
            var third = (y / 64);
            y = y % 64;

            var cr = y / 8;
            var L = y % 8;

            return (ushort)(0b0100_0000_0000_0000 | (third << 11) | (L << 8) | (cr << 5) | x);
        }

        /// <summary>
        /// Is this particular bit set on the bitmap
        /// </summary>
        /// <param name="bitMap"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool isPixelSet(byte bitMap, int offset)
        {
            return (bitMap & (1 << offset)) != 0;
        }

        /// <summary>
        /// Given a value 0-7 and the isBright bit return it's colour
        /// </summary>
        /// <param name="code"></param>
        /// <param name="isBright"></param>
        /// <returns></returns>
        private Color LookupColour(int code, bool isBright)
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

        /// <summary>
        /// Draw pixel on the current display.  Each emulator pixel is a 4x4 block in real life
        /// We also draw directly onto the graphics device to make it look more reliastic.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        private void SetPixelBlock(int x, int y, Color color)
        {
            for (int x0 = 0; x0 < 4; x0++)
            {
                for (int y0 = 0; y0 < 4; y0++)
                {
                    this.bitmap.SetPixel(x + x0, y + y0, color);
                }
            }

            if (!flash)
            {
                var g = this.pictureBox1.CreateGraphics();
                g.FillRectangle(new SolidBrush(color), x, y, 4, 4);
            }
        }

        /// <summary>
        /// Draw pixel on the flash display.  Each emulator pixel is a 4x4 block in real life
        /// We also draw directly onto the graphics device to make it look more reliastic.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        private void SetFlashPixelBlock(int x, int y, Color color)
        {
            for (int x0 = 0; x0 < 4; x0++)
            {
                for (int y0 = 0; y0 < 4; y0++)
                {
                    this.flashBitmap.SetPixel(x + x0, y + y0, color);
                }
            }

            if (flash)
            {
                var g = this.pictureBox1.CreateGraphics();
                g.FillRectangle(new SolidBrush(color), x, y, 4, 4);
            }
        }

        /// <summary>
        /// A colour attribute value has changed.  A single colour block gives the
        /// colours for a 8x8 block.  Update this block.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="address"></param>
        private void DrawColour(Memory memory, ushort address)
        {
            var (paperColour, inkColour, flash) = LookupColours(address);

            var blockOffset = address - SCREEN_MEMORY_BITMAP_END;
            var row = blockOffset / 32;
            var col = blockOffset % 32;

            var y = row * 8;
            var x = col;
            for (int y0 = 0; y0 < 8; y0++)
            {
                var bitMap = memory.ReadByte(AddressFromXY(x, y + y0));

                var offset = 7;
                for (int x0 = 0; x0 < 8; x0++)
                {
                    if (!isPixelSet(bitMap, x0))
                    {
                        SetPixelBlock(((x * 8) + offset) * 4, (y + y0) * 4, paperColour);
                        SetFlashPixelBlock(((x * 8) + offset) * 4, (y + y0) * 4, flash ? inkColour : paperColour);
                    }
                    else
                    {
                        SetPixelBlock(((x * 8) + offset) * 4, (y + y0) * 4, inkColour);
                        SetFlashPixelBlock(((x * 8) + offset) * 4, (y + y0) * 4, flash ? paperColour : inkColour);
                    }

                    offset--;
                }
            }
        }

        /// <summary>
        /// Loads the colour attribute from the given memory address and work out
        /// it's colours and if it should be flashing
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private (Color paper, Color ink, bool flash) LookupColours(ushort address)
        {
            var bitmap = memory.ReadByte(address);

            var ink = bitmap & 0b111;
            var paper = (bitmap & 0b111000) >> 3;
            var bright = ((bitmap & 0b1000000) >> 6) == 1;
            var flash = ((bitmap & 0b10000000) >> 7) == 1;

            return (LookupColour(paper, bright), LookupColour(ink, bright), flash);
        }

        /// <summary>
        /// Part of the bitmap has changed.   Lookup the colour for these 8 pixels
        /// and redraw them.
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="address"></param>
        private void DrawAddress(Memory memory, ushort address)
        {
            var (col, y) = XYFromAddress(address);

            var bitmap = memory.ReadByte(address);
            var offset = 7;
            for (int i = 0; i < 8; i++)
            {
                var x = (col * 8) + offset;

                //Read the bitmap to see if the this pixel is set
                var setPixel = isPixelSet(bitmap, i);

                var realX = x * 4;
                var realY = y * 4;

                var colourAddress = (ushort)(SCREEN_MEMORY_BITMAP_END + ((y / 8) * 32) + (x / 8));
                var (paperColour, inkColour, flash) = LookupColours(colourAddress);
                
                if (setPixel)
                {
                    SetPixelBlock(realX, realY, inkColour);
                    SetFlashPixelBlock(realX, realY, flash ? paperColour : inkColour);
                }
                else
                {
                    SetPixelBlock(realX, realY, paperColour);
                    SetFlashPixelBlock(realX, realY, flash ? inkColour : paperColour);
                }

                offset--;
            }
        }


        /// <summary>
        /// Loads a file in the scr format
        /// </summary>
        /// <param name="filename"></param>
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

        /// <summary>
        /// Sets the screen to black
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdFillDemo_Click(object sender, EventArgs e)
        {
            for (ushort s = SCREEN_MEMORY_START; s < SCREEN_MEMORY_END; s++)
            {
                this.memory.Set(s, 255);
            }
        }

        /// <summary>
        /// Sets the screen to white
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdClearDemo_Click(object sender, EventArgs e)
        {
            for (ushort s = SCREEN_MEMORY_START; s < SCREEN_MEMORY_END; s++)
            {
                this.memory.Set(s, 0);
            }
        }

        private void cmdLoadGloria_Click(object sender, EventArgs e)
        {
            LoadFile("gloria.scr");
        }

        private void cmdLoadColour_Click(object sender, EventArgs e)
        {
            LoadFile("colour.scr");
        }

        private void cmdLoadManicMinor_Click(object sender, EventArgs e)
        {
            LoadFile("ManicMiner(SoftwareProjectsLtd).scr");
        }

        private void cmdLoadManicMiner2_Click(object sender, EventArgs e)
        {
            LoadFile("ManicMiner.scr");
        }

        private void cmdLoadJSW_Click(object sender, EventArgs e)
        {
            LoadFile("JetSetWilly.scr");
        }

        /// <summary>
        /// Alternates between normal and flash
        /// </summary>
        private bool flash;
        
        /// <summary>
        /// Switch between the normal and flash version every 0.5 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (flash)
            {
                this.pictureBox1.Image = bitmap;
            }
            else
            {
                this.pictureBox1.Image = flashBitmap;
            }

            flash = !flash;
        }

        private void cmdLoadDanDare_Click(object sender, EventArgs e)
        {
            LoadFile("DanDare.scr");
        }
    }
}
