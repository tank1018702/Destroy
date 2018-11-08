namespace Destroy.Graphics
{
    using System;

    public struct Block
    {
        public int CharWidth { get; private set; }

        public CoordinateType Coordinate { get; set; }

        public Vector2Int Position { get; set; }

        public char[,] Chars { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int Width => Chars.GetLength(1);

        public int Height => Chars.GetLength(0);

        public Block(char[,] chars, int charWidth, CoordinateType coordinate)
        {
            CharWidth = charWidth;
            Coordinate = coordinate;
            Position = Vector2Int.Zero;
            Chars = chars;
            ColorBlock fore = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] chars, int charWidth, CoordinateType coordinate, Vector2Int point2D)
        {
            CharWidth = charWidth;
            Coordinate = coordinate;
            Position = point2D;
            Chars = chars;
            ColorBlock fore = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Block(char[,] chars, int charWidth, CoordinateType coordinate, Vector2Int point2D, ConsoleColor[,] foreColors)
        {
            CharWidth = charWidth;
            Coordinate = coordinate;
            Position = point2D;
            Chars = chars;
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Block(char[,] chars, int charWidth, CoordinateType coordinate, Vector2Int point2D, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            CharWidth = charWidth;
            Coordinate = coordinate;
            Position = point2D;
            Chars = chars;
            ForeColors = foreColors;
            BackColors = backColors;
        }
    }
}