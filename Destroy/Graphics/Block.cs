namespace Destroy.Graphics
{
    using System;

    public enum RotationAngle
    {
        RotRight90,
        Rot180,
        RotLeft90,
    }

    public struct Block
    {
        public char[,] Items;

        public ConsoleColor[,] ForeColors;

        public ConsoleColor[,] BackColors;

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

        public char[,] Rotate(RotationAngle angle)
        {
            char[,] items = null;

            int width = Width;
            int height = Height;

            switch (angle)
            {
                case RotationAngle.RotRight90:
                    {
                        items = new char[width, height];
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < height; j++)
                                items[i, j] = this.Items[height - 1 - j, i];
                    }
                    break;
                case RotationAngle.Rot180:
                    {
                        items = new char[height, width];
                        for (int i = 0; i < height; i++)
                            for (int j = 0; j < width; j++)
                                items[i, j] = this.Items[height - 1 - i, width - 1 - j];
                    }
                    break;
                case RotationAngle.RotLeft90:
                    {
                        items = new char[width, height];
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < height; j++)
                                items[i, j] = this.Items[j, width - 1 - i];
                    }
                    break;
            }

            return items;
        }

        public Block Copy()
        {
            Block block = new Block(Items, CharWidth, Pos, ForeColors, BackColors);
            return block;
        }
    }
}