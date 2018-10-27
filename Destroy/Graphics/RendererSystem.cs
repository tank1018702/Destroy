using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.Graphics
{
    public static class RendererSystem
    {
        public static void RenderDot(Dot dot, Coordinate coordinate)
        {
            if (coordinate.Type == Coordinate.Mode.RightX__UpY)
            {
                int x = dot.Pos.X * 2;
                int y = coordinate.Height - dot.Pos.Y;
                Print.SetCursorPos(x, y);
            }
            else if (coordinate.Type == Coordinate.Mode.RightX__DownY)
            {
                int x = dot.Pos.X * 2;
                int y = dot.Pos.Y;
                Print.SetCursorPos(x, y);
            }

            Print.Draw(dot.Char, dot.ForeColor, dot.BackColor);
        }

        public static void RenderBlock(Block block, Coordinate coordinate)
        {
            for (int i = 0; i < block.Height; i++)
            {
                for (int j = 0; j < block.Width; j++)
                {
                    if (coordinate.Type == Coordinate.Mode.RightX__UpY)
                    {
                        int x = (block.Pos.X + j) * 2;
                        int y = coordinate.Height - block.Pos.Y + i;
                        Print.SetCursorPos(x, y);
                    }
                    else if (coordinate.Type == Coordinate.Mode.RightX__DownY)
                    {
                        int x = (block.Pos.X + j) * 2;
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