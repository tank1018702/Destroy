namespace Destroy
{
    using System;

    public struct Point2D
    {
        public int X;

        public int Y;

        public Point2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point2D Negative => this * -1;

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj) => this == (Point2D)obj;

        public override string ToString() => $"[X:{X},Y:{Y}]";

        public static Point2D Zero => new Point2D();

        public static int Distance(Point2D a, Point2D b)
        {
            int x = Math.Abs(a.X - b.X);
            int y = Math.Abs(a.Y - b.Y);
            return x + y;
        }

        public static bool operator ==(Point2D left, Point2D right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Point2D left, Point2D right) => left.X != right.X || left.Y != right.Y;

        public static Point2D operator +(Point2D left, Point2D right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Point2D operator -(Point2D left, Point2D right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static Point2D operator *(Point2D left, int right)
        {
            left.X *= right;
            left.Y *= right;
            return left;
        }

        public static Point2D operator /(Point2D left, int right)
        {
            if (right == 0)
                throw new Exception("NaN");
            left.X /= right;
            left.Y /= right;
            return left;
        }
    }
}