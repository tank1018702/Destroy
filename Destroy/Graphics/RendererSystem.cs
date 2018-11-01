using System;

namespace Destroy.Graphics
{
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
                    if (block.Coordinate == CoordinateType.Normal)
                    {
                        int x = (block.Pos.X + j) * block.StrWidth;
                        int y = height - 1 - block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }
                    else if (block.Coordinate == CoordinateType.Window)
                    {
                        int x = (block.Pos.X + j) * block.StrWidth;
                        int y = block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }
                    string str = block.Items[i, j];
                    ConsoleColor foreColor = block.ForeColors[i, j];
                    ConsoleColor backColor = block.BackColors[i, j];
                    Print.Draw(str, foreColor, backColor);
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
                    if (block.Items[i, j] != buffer.Items[i, j])
                    {
                        if (block.Coordinate == CoordinateType.Normal)
                        {
                            int x = (block.Pos.X + j) * block.StrWidth;
                            int y = height - 1 - block.Pos.Y + i;
                            Print.SetCursorPos(x, y);
                        }
                        else if (block.Coordinate == CoordinateType.Window)
                        {
                            int x = (block.Pos.X + j) * block.StrWidth;
                            int y = block.Pos.Y + i;
                            Print.SetCursorPos(x, y);
                        }
                        string str = block.Items[i, j];
                        ConsoleColor foreColor = block.ForeColors[i, j];
                        ConsoleColor backColor = block.BackColors[i, j];
                        Print.Draw(str, foreColor, backColor);
                    }
                }
            }
            //Cache
            buffer = block;
        }

        /// <summary>
        /// 在Block上叠加一层相同大小的Mask，根据maskStr指定Block中哪些item需要被替换为maskStr。
        /// </summary>
        public static Block MaskCulling(Block block, string[,] maskArray, string maskStr)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (maskArray[i, j] == maskStr)
                        block.Items[i, j] = maskStr;
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
                    int x = -1, y = -1;
                    if (occlusion.Coordinate == CoordinateType.Normal)
                    {
                        x = occlusion.Pos.X + j;
                        y = occlusion.Pos.Y - i;
                    }
                    else if (occlusion.Coordinate == CoordinateType.Window)
                    {
                        x = occlusion.Pos.X + j;
                        y = occlusion.Pos.Y + i;
                    }
                    string str = occlusion.Items[i, j];
                    ConsoleColor foreColor = occlusion.ForeColors[i, j];
                    ConsoleColor backColor = occlusion.BackColors[i, j];
                    Coordinate.SetInArray(block.Items, str, x, y, occlusion.Coordinate);
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
                    int x = -1, y = -1;
                    if (cutBlock.Coordinate == CoordinateType.Normal)
                    {
                        x = cutBlock.Pos.X + j;
                        y = cutBlock.Pos.Y - i;
                    }
                    else if (cutBlock.Coordinate == CoordinateType.Window)
                    {
                        x = cutBlock.Pos.X + j;
                        y = cutBlock.Pos.Y + i;
                    }
                    string str = Coordinate.GetInArray(block.Items, x, y, cutBlock.Coordinate);
                    ConsoleColor foreColor = Coordinate.GetInArray(block.ForeColors, x, y, cutBlock.Coordinate);
                    ConsoleColor backColor = Coordinate.GetInArray(block.BackColors, x, y, cutBlock.Coordinate);
                    cutBlock.Items[i, j] = str;
                    cutBlock.ForeColors[i, j] = foreColor;
                    cutBlock.BackColors[i, j] = backColor;
                }
            }
        }
    }
}