namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        public char[,] Chars { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int CharWidth { get; set; }

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