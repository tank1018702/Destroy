using System;

namespace Destroy
{
    public class Renderer : Component
    {
        public int CharWidth { get; set; }

        public char[,] Chars { get; set; }

        public ConsoleColor[,] ForeColors { get; set; }

        public ConsoleColor[,] BackColors { get; set; }

        public int Width => Chars.GetLength(1);

        public int Height => Chars.GetLength(0);
    }
}