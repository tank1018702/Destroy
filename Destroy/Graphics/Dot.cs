namespace Destroy.Graphics
{
    using System;

    public struct Dot
    {
        public Point2D Pos { get; set; }

        public char Char { get; set; }

        public ConsoleColor ForeColor { get; set; }

        public ConsoleColor BackColor { get; set; }

        public Dot(char c)
        {
            Char = c;
            Pos = Point2D.Zero;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        public Dot(char c, Point2D point2D)
        {
            Char = c;
            Pos = point2D;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        public Dot(char c, Point2D point2D, ConsoleColor foreColor)
        {
            Char = c;
            Pos = point2D;
            ForeColor = foreColor;
            BackColor = ConsoleColor.Black;
        }

        public Dot(char c, Point2D point2D, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Char = c;
            Pos = point2D;
            ForeColor = foreColor;
            BackColor = backColor;
        }
    }
}