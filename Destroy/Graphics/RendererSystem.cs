using System;

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
                    if (coordinate.Type == CoordinateType.RightX_UpY)
                    {
                        int x = (block.Pos.X + j) * block.StrWidth;
                        int y = coordinate.Height - 1 - block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }
                    else if (coordinate.Type == CoordinateType.RightX_DownY)
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

        public static void RenderBlockBuffer(Block block, ref Block buffer, Coordinate coordinate)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    //Diff
                    if (block.Items[i, j] != buffer.Items[i, j])
                    {
                        if (coordinate.Type == CoordinateType.RightX_UpY)
                        {
                            int x = (block.Pos.X + j) * block.StrWidth;
                            int y = coordinate.Height - 1 - block.Pos.Y + i;
                            Print.SetCursorPos(x, y);
                        }
                        else if (coordinate.Type == CoordinateType.RightX_DownY)
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

        public static Block MaskCulling(Block block, Block mask, string maskStr)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (mask.Items[i, j] == maskStr)
                        block.Items[i, j] = maskStr;
                }
            }
            return block;
        }

        public static Block MaskCulling(Block block, string[,] maskArray, string maskStr)
        {
            Block mask = new Block(maskArray, block.StrWidth);
            Block b = MaskCulling(block, mask, maskStr);
            return b;
        }

        public static Block OcclusionCulling(Block block, Block occlusion, Point2D point, CoordinateType type)
        {
            if (type == CoordinateType.RightX_UpY)
            {
                for (int i = 0; i < occlusion.Height; i++)
                {
                    for (int j = 0; j < occlusion.Width; j++)
                    {
                        int x = point.X + j;
                        int y = point.Y - i;
                        string str = occlusion.Items[i, j];
                        ConsoleColor foreColor = occlusion.ForeColors[i, j];
                        ConsoleColor backColor = occlusion.BackColors[i, j];
                        Coordinate.Set_RightX_UpY(block.Items, str, x, y);
                        Coordinate.Set_RightX_UpY(block.ForeColors, foreColor, x, y);
                        Coordinate.Set_RightX_UpY(block.BackColors, backColor, x, y);
                    }
                }
            }
            else if (type == CoordinateType.RightX_DownY)
            {
                for (int i = 0; i < occlusion.Height; i++)
                {
                    for (int j = 0; j < occlusion.Width; j++)
                    {
                        int x = point.X + j;
                        int y = point.Y + i;
                        string str = occlusion.Items[i, j];
                        ConsoleColor foreColor = occlusion.ForeColors[i, j];
                        ConsoleColor backColor = occlusion.BackColors[i, j];
                        Coordinate.Set_RightX_DownY(block.Items, str, x, y);
                        Coordinate.Set_RightX_DownY(block.ForeColors, foreColor, x, y);
                        Coordinate.Set_RightX_DownY(block.BackColors, backColor, x, y);
                    }
                }
            }
            return block;
        }

        public static void BlockCutting(Block block, ref Block cutBlock, Point2D point, CoordinateType type)
        {
            if (type == CoordinateType.RightX_UpY)
            {
                for (int i = 0; i < cutBlock.Height; i++)
                {
                    for (int j = 0; j < cutBlock.Width; j++)
                    {
                        int x = point.X + j;
                        int y = point.Y - i;
                        string str = Coordinate.Get_RightX_UpY(block.Items, x, y);
                        ConsoleColor foreColor = Coordinate.Get_RightX_UpY(block.ForeColors, x, y);
                        ConsoleColor backColor = Coordinate.Get_RightX_UpY(block.BackColors, x, y);
                        cutBlock.Items[i, j] = str;
                        cutBlock.ForeColors[i, j] = foreColor;
                        cutBlock.BackColors[i, j] = backColor;
                    }
                }
            }
            else if (type == CoordinateType.RightX_DownY)
            {
                for (int i = 0; i < cutBlock.Height; i++)
                {
                    for (int j = 0; j < cutBlock.Width; j++)
                    {
                        int x = point.X + j;
                        int y = point.Y + i;
                        string str = Coordinate.Get_RightX_DownY(block.Items, x, y);
                        ConsoleColor foreColor = Coordinate.Get_RightX_DownY(block.ForeColors, x, y);
                        ConsoleColor backColor = Coordinate.Get_RightX_DownY(block.BackColors, x, y);
                        cutBlock.Items[i, j] = str;
                        cutBlock.ForeColors[i, j] = foreColor;
                        cutBlock.BackColors[i, j] = backColor;
                    }
                }
            }
        }
    }
}