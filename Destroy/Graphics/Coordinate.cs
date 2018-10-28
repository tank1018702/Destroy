namespace Destroy.Graphics
{
    public enum CoordinateType
    {
        RightX_UpY,
        RightX_DownY,
    }

    public class Coordinate
    {
        public int Height { get; private set; }

        public CoordinateType Type { get; private set; }

        public Coordinate(CoordinateType type, int height)
        {
            Type = type;
            Height = height;
        }

        public static T Get_RightX_UpY<T>(T[,] array, int x, int y)
        {
            int _x = array.GetLength(0) - 1 - y;
            int _y = x;
            return array[_x, _y];
        }

        public static void Set_RightX_UpY<T>(T[,] array, T t, int x, int y)
        {
            int _x = array.GetLength(0) - 1 - y;
            int _y = x;
            array[_x, _y] = t;
        }

        public static T Get_RightX_DownY<T>(T[,] array, int x, int y) => array[y, x];

        public static void Set_RightX_DownY<T>(T[,] array, T t, int x, int y) => array[y, x] = t;
    }
}