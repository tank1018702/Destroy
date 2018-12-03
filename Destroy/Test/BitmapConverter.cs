namespace Destroy.Test
{
    using System;
    using System.Drawing;

    public struct Pixel
    {
        public Vector2Int Pos;
        public ConsoleColor Color;

        public Pixel(Vector2Int pos, ConsoleColor color)
        {
            Pos = pos;
            Color = color;
        }
    }

    public static class BitmapConverter
    {
        private static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor result = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor each in Enum.GetValues(typeof(ConsoleColor)))
            {
                string name = Enum.GetName(typeof(ConsoleColor), each);

                Color color = Color.FromName(name == "DarkYellow" ? "Orange" : name); // bug fix
                var t = Math.Pow(color.R - rr, 2.0) + Math.Pow(color.G - gg, 2.0) + Math.Pow(color.B - bb, 2.0);
                if (t == 0.0)
                    return each;
                if (t < delta)
                {
                    delta = t;
                    result = each;
                }
            }
            return result;
        }

        private static void Get(Bitmap bitmap)
        {
            Pixel[] pixels = new Pixel[bitmap.Size.Width * bitmap.Size.Height];

            int i = 0;
            for (int x = 0; x < bitmap.Size.Width; x++)
            {
                for (int y = 0; y < bitmap.Size.Height; y++)
                {

                    Color col = bitmap.GetPixel(x, y);

                    pixels[i] = new Pixel(new Vector2Int(x, y), ClosestConsoleColor(col.R, col.G, col.B));
                    i++;
                }
            }
        }
    }
}