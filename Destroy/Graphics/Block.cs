namespace Destroy.Graphics
{
    using System;

    public struct Block
    {
        public string[,] Items { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int StrWidth { get; private set; }

        public Point2D Pos { get; set; }

        public int Width => Items.GetLength(1);

        public int Height => Items.GetLength(0);

        public Block(string str, int charWidth)
        {
            Items = new string[,] { { str } };
            StrWidth = charWidth;
            Pos = Point2D.Zero;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string str, int charWidth, Point2D point2D)
        {
            Items = new string[,] { { str } };
            StrWidth = charWidth;
            Pos = point2D;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth)
        {
            Items = items;
            StrWidth = charWidth;
            Pos = Point2D.Zero;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, Point2D point2D)
        {
            Items = items;
            StrWidth = charWidth;
            Pos = point2D;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, Point2D point2D, ConsoleColor[,] foreColors)
        {
            Items = items;
            StrWidth = charWidth;
            Pos = point2D;
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, Point2D point2D, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            Items = items;
            StrWidth = charWidth;
            Pos = point2D;
            ForeColors = foreColors;
            BackColors = backColors;
        }

        public Block Copy()
        {
            Block block = new Block(Items, StrWidth, Pos, ForeColors, BackColors);
            return block;
        }
    }
}