namespace Destroy
{
    using System;
    using Destroy.Graphics;

    public class Renderer : Component
    {
        public char[,] Chars;

        public ConsoleColor[,] ForeColors;

        public ConsoleColor[,] BackColors;

        public int CharWidth;

        public int Width => Chars.GetLength(1);

        public int Height => Chars.GetLength(0);

        public Renderer()
        {
            Chars = new char[,] { { ' ' } };
            ForeColors = new ConsoleColor[,] { { ConsoleColor.Gray } };
            BackColors = new ConsoleColor[,] { { ConsoleColor.Black } };
            CharWidth = 1;
        }

        public Renderer(char[,] chars, int charWidth)
        {
            Chars = chars;
            CharWidth = charWidth;
            ColorBlock fore = new ColorBlock(Width, Height, ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Width, Height, ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public Renderer(char[,] chars, int charWidth, ConsoleColor[,] foreColors)
        {
            Chars = chars;
            CharWidth = charWidth;
            ColorBlock back = new ColorBlock(Width, Height, ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public Renderer(char[,] chars, int charWidth, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            Chars = chars;
            CharWidth = charWidth;
            ForeColors = foreColors;
            BackColors = backColors;
        }
    }
}