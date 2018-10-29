namespace Destroy.Graphics
{
    using System;

    public struct Block
    {
        public char[,] Items { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int CharWidth { get; private set; }

        public Point2D Pos { get; set; }

        public int Width => Items.GetLength(1);

        public int Height => Items.GetLength(0);

        public Block(char c, int charWidth)
        {
            Items = new char[,] { { c } };
            CharWidth = charWidth;
            Pos = Point2D.Zero;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char c, int charWidth, Point2D point2D)
        {
            Items = new char[,] { { c } };
            CharWidth = charWidth;
            Pos = point2D;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, int charWidth)
        {
            this.Items = items;
            CharWidth = charWidth;
            Pos = Point2D.Zero;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, int charWidth, Point2D point2D)
        {
            this.Items = items;
            CharWidth = charWidth;
            Pos = point2D;
            ColorBlock fore = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, int charWidth, Point2D point2D, ConsoleColor[,] foreColors)
        {
            this.Items = items;
            CharWidth = charWidth;
            Pos = point2D;
            ColorBlock back = new ColorBlock(this.Items.GetLength(1), this.Items.GetLength(0), ConsoleColor.Black);
            this.ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, int charWidth, Point2D point2D, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            this.Items = items;
            CharWidth = charWidth;
            Pos = point2D;
            this.ForeColors = foreColors;
            this.BackColors = backColors;
        }

        public Block Copy()
        {
            Block block = new Block(Items, CharWidth, Pos, ForeColors, BackColors);
            return block;
        }
    }
}