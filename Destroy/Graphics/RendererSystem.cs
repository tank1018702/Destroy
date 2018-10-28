namespace Destroy.Graphics
{
    public static class RendererSystem
    {
        public static void RenderBlock(Block block, Coordinate coordinate)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (coordinate.Type == Coordinate.Mode.RightX_UpY)
                    {
                        int x = (block.Pos.X + j) * block.CharWidth;
                        int y = coordinate.Height - 1 - block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }
                    else if (coordinate.Type == Coordinate.Mode.RightX_DownY)
                    {
                        int x = (block.Pos.X + j) * block.CharWidth;
                        int y = block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }

                    char c = block.GetItem(i, j);
                    Print.Draw(c, block.ForeColors[i, j], block.BackColors[i, j]);
                }
            }
        }

        public static void RenderBlockBuffer(Block block, ref Block buffer, Coordinate coordinate)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    //Diff
                    if (block.GetItem(i, j) != buffer.GetItem(i, j))
                    {
                        if (coordinate.Type == Coordinate.Mode.RightX_UpY)
                        {
                            int x = (block.Pos.X + j) * block.CharWidth;
                            int y = coordinate.Height - 1 - block.Pos.Y + i;
                            Print.SetCursorPos(x, y);
                        }
                        else if (coordinate.Type == Coordinate.Mode.RightX_DownY)
                        {
                            int x = (block.Pos.X + j) * block.CharWidth;
                            int y = block.Pos.Y + i;
                            Print.SetCursorPos(x, y);
                        }

                        char c = block.GetItem(i, j);
                        Print.Draw(c, block.ForeColors[i, j], block.BackColors[i, j]);
                    }
                }
            }
            //Cache
            buffer = block;
        }

        public static Block RenderMask(Block block, Block mask, char maskChar)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (mask.GetItem(i, j) == maskChar)
                    {
                        block.SetItem(maskChar, i, j);
                    }
                }
            }
            return block;
        }

        public static Block RenderMask(Block block, char[,] maskArray, char maskChar)
        {
            Block mask = new Block(maskArray, block.CharWidth);
            Block b = RenderMask(block, mask, maskChar);
            return b;
        }
    }
}