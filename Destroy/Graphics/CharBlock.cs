namespace Destroy.Graphics
{
    public class CharBlock
    {
        public char[,] Chars { get; private set; }

        public int Width => Chars.GetLength(1);

        public int Length => Chars.GetLength(0);

        public CharBlock(int width, int height, char c)
        {
            Chars = new char[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Chars[i, j] = c;
        }
    }
}