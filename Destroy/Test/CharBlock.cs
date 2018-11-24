namespace Destroy.Test
{
    public struct CharBlock
    {
        public char[,] Chars { get; private set; }

        public int Width => Chars.GetLength(1);

        public int Height => Chars.GetLength(0);

        public CharBlock(int width, int height, char c)
        {
            Chars = new char[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Chars[i, j] = c;
        }
    }
}