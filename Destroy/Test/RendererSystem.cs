namespace Destroy.Test
{
    using System;

    /// <summary>
    /// 渲染系统
    /// </summary>
    public static class RendererSystem
    {
        /// <summary>
        /// 在控制台缓冲区上渲染一个Block
        /// </summary>
        public static void RenderBlock(Block block)
        {
            int height = Console.BufferHeight;

            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    int y = -1;

                    if (block.Coordinate == CoordinateType.Normal)
                        y = height - 1 - block.Position.Y + i;
                    else if (block.Coordinate == CoordinateType.Window)
                        y = block.Position.Y + i;

                    int x = (block.Position.X + j) * block.CharWidth;
                    Console.SetCursorPosition(x, y);

                    char c = block.Chars[i, j];
                    ConsoleColor foreColor = block.ForeColors[i, j];
                    ConsoleColor backColor = block.BackColors[i, j];
                    Print.Draw(c, foreColor, backColor);
                }
            }
        }

        /// <summary>
        /// 在控制台缓冲区上利用双缓冲技术渲染一个Block
        /// </summary>
        public static void RenderBlockBuffer(Block block, ref Block buffer)
        {
            int height = Console.BufferHeight;

            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    //Diff
                    if (block.Chars[i, j] != buffer.Chars[i, j])
                    {
                        int y = -1;

                        if (block.Coordinate == CoordinateType.Normal)
                            y = height - 1 - block.Position.Y + i;
                        else if (block.Coordinate == CoordinateType.Window)
                            y = block.Position.Y + i;

                        int x = (block.Position.X + j) * block.CharWidth;
                        Console.SetCursorPosition(x, y);

                        char c = block.Chars[i, j];
                        ConsoleColor foreColor = block.ForeColors[i, j];
                        ConsoleColor backColor = block.BackColors[i, j];
                        Print.Draw(c, foreColor, backColor);
                    }
                }
            }
            //Cache
            buffer = block;
        }

        /// <summary>
        /// 在Block上叠加一层相同大小的Mask，根据maskStr指定Block中哪些item需要被替换为maskStr。
        /// </summary>
        public static Block MaskCulling(Block block, char[,] maskArray, char maskChar)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (maskArray[i, j] == maskChar)
                        block.Chars[i, j] = maskChar;
                }
            }
            return block;
        }

        /// <summary>
        /// 在一个Block上进行一个新的Block的渲染。
        /// </summary>
        public static Block OcclusionCulling(Block block, Block occlusion)
        {
            for (int i = 0; i < occlusion.Height; i++)
            {
                for (int j = 0; j < occlusion.Width; j++)
                {
                    int x = occlusion.Position.X + j;
                    int y = -1;

                    if (occlusion.Coordinate == CoordinateType.Normal)
                        y = occlusion.Position.Y - i;
                    else if (occlusion.Coordinate == CoordinateType.Window)
                        y = occlusion.Position.Y + i;

                    char c = occlusion.Chars[i, j];
                    ConsoleColor foreColor = occlusion.ForeColors[i, j];
                    ConsoleColor backColor = occlusion.BackColors[i, j];
                    Coordinate.SetInArray(block.Chars, c, x, y, occlusion.Coordinate);
                    Coordinate.SetInArray(block.ForeColors, foreColor, x, y, occlusion.Coordinate);
                    Coordinate.SetInArray(block.BackColors, backColor, x, y, occlusion.Coordinate);
                }
            }
            return block;
        }

        /// <summary>
        /// 在一个Block上截取出指定cutBlock。
        /// </summary>
        public static void BlockCutting(Block block, ref Block cutBlock)
        {
            for (int i = 0; i < cutBlock.Height; i++)
            {
                for (int j = 0; j < cutBlock.Width; j++)
                {
                    int x = cutBlock.Position.X + j;
                    int y = -1;

                    if (cutBlock.Coordinate == CoordinateType.Normal)
                        y = cutBlock.Position.Y - i;
                    else if (cutBlock.Coordinate == CoordinateType.Window)
                        y = cutBlock.Position.Y + i;

                    char c = Coordinate.GetInArray(block.Chars, x, y, cutBlock.Coordinate);
                    ConsoleColor foreColor = Coordinate.GetInArray(block.ForeColors, x, y, cutBlock.Coordinate);
                    ConsoleColor backColor = Coordinate.GetInArray(block.BackColors, x, y, cutBlock.Coordinate);
                    cutBlock.Chars[i, j] = c;
                    cutBlock.ForeColors[i, j] = foreColor;
                    cutBlock.BackColors[i, j] = backColor;
                }
            }
        }
    }
}