using System;

namespace ZombieInfection
{
    /// <summary>
    /// 标识2D向量或点
    /// </summary>
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => this == (Vector2)obj;

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString() => $"[X:{X},Y:{Y}]";

        public void ToInt(out int x, out int y)
        {
            x = (int)X;
            y = (int)Y;
        }

        public float Magnitude
        {
            get
            {
                float magSqr = X * X + Y * Y;
                return (float)Math.Sqrt(magSqr);
            }
        }

        public Vector2 Normalized
        {
            get => this / Magnitude;
        }

        public Vector2 Negative
        {
            get => this * -1;
        }

        public static Vector2 Zero
        {
            get => new Vector2(0, 0);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 vector = a - b;
            return vector.Magnitude;
        }

        public static float Dot(Vector2 a, Vector2 b)
        {
            a.X *= b.X;
            a.Y *= b.Y;
            return a.X + a.Y;
        }

        public static bool operator ==(Vector2 self, Vector2 other)
        {
            return self.X == other.X && self.Y == other.Y;
        }

        public static bool operator !=(Vector2 self, Vector2 other)
        {
            return self.X != other.X || self.Y != other.Y;
        }

        public static Vector2 operator +(Vector2 self, Vector2 other)
        {
            self.X += other.X;
            self.Y += other.Y;
            return self;
        }

        public static Vector2 operator -(Vector2 self, Vector2 other)
        {
            self.X -= other.X;
            self.Y -= other.Y;
            return self;
        }

        public static Vector2 operator *(Vector2 self, float scalar)
        {
            self.X *= scalar;
            self.Y *= scalar;
            return self;
        }

        public static Vector2 operator /(Vector2 self, float scalar)
        {
            if (scalar == 0) //防止除0得到NaN
                return self;

            self.X /= scalar;
            self.Y /= scalar;
            return self;
        }
    }

    /// <summary>
    /// 标识2D整数向量或点
    /// </summary>
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Vector2Int(Vector2 vector)
        {
            Vector2Int vector2Int = new Vector2Int();
            vector2Int.X = (int)vector.X;
            vector2Int.Y = (int)vector.Y;
            return vector2Int;
        }
    }
}