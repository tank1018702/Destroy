namespace Destroy.Graphics
{
    public class RendererSystem
    {
        private Block buffer = default(Block);

        public void RenderBlockBuffer(Block block, Coordinate coordinate)
        {
            if (buffer.Equals(default(Block)) && !block.Equals(default(Block)))
            {
                CharBlock charBlock = new CharBlock(block.Width, block.Height, ' ');

                buffer = new Block(charBlock.Chars, block.CharWidth);
            }

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
    }
}