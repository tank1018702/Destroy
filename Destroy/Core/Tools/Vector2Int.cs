namespace Destroy
{
    using System;

    public struct Vector2Int
    {
        public int X;

        public int Y;

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2Int Negative => this * -1;

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj) => this == (Vector2Int)obj;

        public override string ToString() => $"[X:{X},Y:{Y}]";

        public static Vector2Int Zero => new Vector2Int();

        public static int Distance(Vector2Int a, Vector2Int b)
        {
            int x = Math.Abs(a.X - b.X);
            int y = Math.Abs(a.Y - b.Y);
            return x + y;
        }

        public static bool operator ==(Vector2Int left, Vector2Int right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Vector2Int left, Vector2Int right) => left.X != right.X || left.Y != right.Y;

        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Vector2Int operator -(Vector2Int left, Vector2Int right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static Vector2Int operator *(Vector2Int left, int right)
        {
            left.X *= right;
            left.Y *= right;
            return left;
        }

        public static Vector2Int operator /(Vector2Int left, int right)
        {
            if (right == 0)
                throw new Exception("NaN");
            left.X /= right;
            left.Y /= right;
            return left;
        }
    }
}