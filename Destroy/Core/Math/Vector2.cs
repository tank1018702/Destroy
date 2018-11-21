namespace Destroy
{
    using System;

    public struct Vector2
    {
        public float X;

        public float Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override int GetHashCode() => base.GetHashCode();

        public override bool Equals(object obj) => this == (Vector2)obj;

        public override string ToString() => $"[X:{X},Y:{Y}]";

        public float Magnitude
        {
            get
            {
                float magSquare = X * X + Y * Y;
                return (float)Math.Sqrt(magSquare);
            }
        }

        public Vector2 Normalized => this / Magnitude;

        public Vector2 Negative => this * -1f;

        public static Vector2 Zero => new Vector2();

        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 vector = a - b;
            return vector.Magnitude;
        }

        public static bool operator ==(Vector2 left, Vector2 right) => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Vector2 left, Vector2 right) => left.X != right.X || left.Y != right.Y;

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static Vector2 operator *(Vector2 left, float right)
        {
            left.X *= right;
            left.Y *= right;
            return left;
        }

        public static Vector2 operator /(Vector2 left, float right)
        {
            if (right == 0)
                throw new Exception("NaN");
            left.X /= right;
            left.Y /= right;
            return left;
        }

        public static explicit operator Vector2(Vector2Int vector)
        {
            Vector2 vector2 = new Vector2();
            vector2.X = vector.X;
            vector2.Y = vector.Y;
            return vector2;
        }
    }
}