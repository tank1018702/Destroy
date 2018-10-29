namespace Destroy.Graphics
{
    using System;

    public struct Block
    {
        public int StrWidth { get; private set; }

        public CoordinateType Coordinate { get; set; }

        public Point2D Pos { get; set; }

        public string[,] Items { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int Width => Items.GetLength(1);

        public int Height => Items.GetLength(0);

        public Block(string[,] items, int charWidth, CoordinateType coordinate)
        {
            StrWidth = charWidth;
            Coordinate = coordinate;
            Pos = Point2D.Zero;
            Items = items;
            ColorBlock fore = new ColorBlock(Items.GetLength(1), Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Items.GetLength(1), Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, CoordinateType coordinate, Point2D point2D)
        {
            StrWidth = charWidth;
            Coordinate = coordinate;
            Pos = point2D;
            Items = items;
            ColorBlock fore = new ColorBlock(Items.GetLength(1), Items.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Items.GetLength(1), Items.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, CoordinateType coordinate, Point2D point2D, ConsoleColor[,] foreColors)
        {
            StrWidth = charWidth;
            Coordinate = coordinate;
            Pos = point2D;
            Items = items;
            ColorBlock back = new ColorBlock(Items.GetLength(1), Items.GetLength(0), ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Block(string[,] items, int charWidth, CoordinateType coordinate, Point2D point2D, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            StrWidth = charWidth;
            Coordinate = coordinate;
            Pos = point2D;
            Items = items;
            ForeColors = foreColors;
            BackColors = backColors;
        }
    }
}