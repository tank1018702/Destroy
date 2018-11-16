namespace Destroy
{
    using System;

    public struct RendererData
    {
        public RendererGrid[,] Grids { get; set; }

        public int CharWidth { get; set; }

        public int Width => Grids.GetLength(1);

        public int Height => Grids.GetLength(0);

        public RendererData(int width, int height)
        {
            Grids = new RendererGrid[height, width];
            CharWidth = 1;
            for (int i = 0; i < Grids.GetLength(0); i++)
                for (int j = 0; j < Grids.GetLength(1); j++)
                    Grids[i, j] = new RendererGrid(' ', ConsoleColor.Gray, ConsoleColor.Black);
        }

        public RendererData(int width, int height, int charWidth)
        {
            Grids = new RendererGrid[height, width];
            CharWidth = charWidth;
            for (int i = 0; i < Grids.GetLength(0); i++)
                for (int j = 0; j < Grids.GetLength(1); j++)
                    Grids[i, j] = new RendererGrid(' ', ConsoleColor.Gray, ConsoleColor.Black);
        }

        public RendererData(int width, int height, int charWidth, char c)
        {
            Grids = new RendererGrid[height, width];
            CharWidth = charWidth;
            for (int i = 0; i < Grids.GetLength(0); i++)
                for (int j = 0; j < Grids.GetLength(1); j++)
                    Grids[i, j] = new RendererGrid(c, ConsoleColor.Gray, ConsoleColor.Black);
        }

        public RendererData(int width, int height, int charWidth, char c, ConsoleColor fore)
        {
            Grids = new RendererGrid[height, width];
            CharWidth = charWidth;
            for (int i = 0; i < Grids.GetLength(0); i++)
                for (int j = 0; j < Grids.GetLength(1); j++)
                    Grids[i, j] = new RendererGrid(c, fore, ConsoleColor.Black);
        }

        public RendererData(int width, int height, int charWidth, char c, ConsoleColor fore, ConsoleColor back)
        {
            Grids = new RendererGrid[height, width];
            CharWidth = charWidth;
            for (int i = 0; i < Grids.GetLength(0); i++)
                for (int j = 0; j < Grids.GetLength(1); j++)
                    Grids[i, j] = new RendererGrid(c, fore, back);
        }
    }
}