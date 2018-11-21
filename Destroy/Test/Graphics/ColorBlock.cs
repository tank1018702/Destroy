namespace Destroy.Graphics
{
    using System;

    public struct ColorBlock
    {
        public ConsoleColor[,] Colors { get; private set; }

        public int Width => Colors.GetLength(1);

        public int Height => Colors.GetLength(0);

        public ColorBlock(int width, int height, ConsoleColor color)
        {
            Colors = new ConsoleColor[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Colors[i, j] = color;
        }
    }
}