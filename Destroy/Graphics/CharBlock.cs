namespace Destroy.Graphics
{
    public struct CharBlock
    {
        public string[,] Items { get; private set; }

        public int Width => Items.GetLength(1);

        public int Length => Items.GetLength(0);

        public CharBlock(int width, int height, string str)
        {
            Items = new string[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Items[i, j] = str;
        }
    }
}