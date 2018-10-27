namespace Destroy.Graphics
{
    using System;

    public struct Block
    {
        private char[,] items;

        public Point2D Pos { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int Width => items.GetLength(1);

        public int Height => items.GetLength(0);

        public Block(char[,] items)
        {
            Pos = Point2D.Zero;
            this.items = items;
            ColorBlock fore = new ColorBlock(this.items.GetLength(1), this.items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.items.GetLength(1), this.items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, Point2D point2D)
        {
            Pos = point2D;
            this.items = items;
            ColorBlock fore = new ColorBlock(this.items.GetLength(1), this.items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(this.items.GetLength(1), this.items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, Point2D point2D, ConsoleColor[,] foreColors)
        {
            Pos = point2D;
            this.items = items;
            ColorBlock back = new ColorBlock(this.items.GetLength(1), this.items.GetLength(0), ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Block(char[,] items, Point2D point2D, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            Pos = point2D;
            this.items = items;
            ForeColors = foreColors;
            BackColors = backColors;
        }

        public enum RotationAngle
        {
            RotRight90,
            Rot180,
            RotLeft90,
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
                                items[i, j] = this.items[height - 1 - j, i];
                    }
                    break;
                case RotationAngle.Rot180:
                    {
                        items = new char[height, width];
                        for (int i = 0; i < height; i++)
                            for (int j = 0; j < width; j++)
                                items[i, j] = this.items[height - 1 - i, width - 1 - j];
                    }
                    break;
                case RotationAngle.RotLeft90:
                    {
                        items = new char[width, height];
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < height; j++)
                                items[i, j] = this.items[j, width - 1 - i];
                    }
                    break;
            }

            return items;
        }

        public char GetItem(int x, int y) => items[x, y];

        public char GetItem2(int x, int y)
        {
            int _x = items.GetLength(0) - 1 - y;
            int _y = x;
            return items[_x, _y];
        }

        public char SetItem(char c, int x, int y) => items[x, y] = c;

        public void SetItem2(char c, int x, int y)
        {
            int _x = items.GetLength(0) - 1 - y;
            int _y = x;
            items[_x, _y] = c;
        }
    }
}