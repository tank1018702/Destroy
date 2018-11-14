namespace Destroy
{
    using System;

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
    }
}