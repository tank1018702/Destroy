namespace Destroy.Graphics
{
    public enum CoordinateType
    {
        RightX_UpY,
        RightX_DownY,
    }

    public enum RotationAngle
    {
        RotRight90,
        Rot180,
        RotLeft90,
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

        public static T[,] Rotate<T>(T[,] array, RotationAngle angle)
        {
            T[,] rotArray = null;

            int width = array.GetLength(1);
            int height = array.GetLength(0);

            switch (angle)
            {
                case RotationAngle.RotRight90:
                    {
                        rotArray = new T[width, height];
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < height; j++)
                                rotArray[i, j] = array[height - 1 - j, i];
                    }
                    break;
                case RotationAngle.Rot180:
                    {
                        rotArray = new T[height, width];
                        for (int i = 0; i < height; i++)
                            for (int j = 0; j < width; j++)
                                rotArray[i, j] = array[height - 1 - i, width - 1 - j];
                    }
                    break;
                case RotationAngle.RotLeft90:
                    {
                        rotArray = new T[width, height];
                        for (int i = 0; i < width; i++)
                            for (int j = 0; j < height; j++)
                                rotArray[i, j] = array[j, width - 1 - i];
                    }
                    break;
            }

            return rotArray;
        }
    }
}