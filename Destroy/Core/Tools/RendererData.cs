namespace Destroy
{
    using System;
    using Destroy.Graphics;

    public struct RendererData
    {
        public char[,] Chars { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int CharWidth { get; set; }

        public int Width => Chars.GetLength(1);

        public int Height => Chars.GetLength(0);

        public RendererData(int width, int height)
        {
            CharWidth = 1;
            CharBlock charBlock = new CharBlock(width, height, ' ');
            ColorBlock fore = new ColorBlock(width, height, ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(width, height, ConsoleColor.Black);
            Chars = charBlock.Chars;
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public RendererData(char[,] chars)
        {
            CharWidth = 1;
            Chars = chars;
            ColorBlock fore = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public RendererData(char[,] chars, int charWidth)
        {
            CharWidth = charWidth;
            Chars = chars;
            ColorBlock fore = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Gray);
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = fore.Colors;
            BackColors = back.Colors;
        }

        public RendererData(char[,] chars, int charWidth, ConsoleColor[,] foreColors)
        {
            CharWidth = charWidth;
            Chars = chars;
            ColorBlock back = new ColorBlock(Chars.GetLength(1), Chars.GetLength(0), ConsoleColor.Black);
            ForeColors = foreColors;
            BackColors = back.Colors;
        }

        public RendererData(char[,] chars, int charWidth, ConsoleColor[,] foreColors, ConsoleColor[,] backColors)
        {
            CharWidth = charWidth;
            Chars = chars;
            ForeColors = foreColors;
            BackColors = backColors;
        }
    }
}