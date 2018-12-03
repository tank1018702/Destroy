namespace Destroy
{
    using System;
    using System.Collections.Generic;

    public enum CoordinateType
    {
        /// <summary>
        /// right x, down Y
        /// </summary>
        Window = 0,
        /// <summary>
        /// right X, up Y
        /// </summary>
        Normal = 1,
    }

    public enum RotationAngle
    {
        RotRight90,
        Rot180,
        RotLeft90,
    }

    public static class Mathematics
    {
        /// <summary>
        /// 算两个浮点数是否相近
        /// </summary>
        public static bool Approximately(float x, float y, float epsilon = 0.001f)
        {
            return x + epsilon >= y && x - epsilon <= y;
        }

        /// <summary>
        /// 限制Int取值范围
        /// </summary>
        public static int ClampInt(int x, int min, int max)
        {
            x = (x > max) ? max : x;
            x = (x < min) ? min : x;

            return x;
        }

        /// <summary>
        /// 插入排序 (从小到大)
        /// </summary>
        public static void InsertionSort(List<KeyValuePair<int, object>> pairs)
        {
            //正序:从大到小
            for (int i = 1; i < pairs.Count; i++)
            {
                int j = i;

                while (j > 0 && pairs[j].Key < pairs[j - 1].Key)
                {
                    var temp = pairs[j];
                    pairs[j] = pairs[j - 1];
                    pairs[j - 1] = temp;
                    j--;
                }
            }
        }

        /// <summary>
        /// 插入排序 (从小到大)
        /// </summary>
        public static void InsertionSort(List<KeyValuePair<uint, object>> pairs)
        {
            //正序:从大到小
            for (int i = 1; i < pairs.Count; i++)
            {
                int j = i;

                while (j > 0 && pairs[j].Key < pairs[j - 1].Key)
                {
                    var temp = pairs[j];
                    pairs[j] = pairs[j - 1];
                    pairs[j - 1] = temp;
                    j--;
                }
            }
        }

        /// <summary>
        /// 在数组中取值
        /// </summary>
        [Obsolete("转而使用矩阵")]
        public static T GetInArray<T>(T[,] array, int x, int y, CoordinateType coordinate)
        {
            if (coordinate == CoordinateType.Normal)
            {
                int _x = array.GetLength(0) - 1 - y;
                int _y = x;
                return array[_x, _y];
            }
            else if (coordinate == CoordinateType.Window)
            {
                return array[y, x];
            }
            return default(T);
        }

        /// <summary>
        /// 设置数组的值
        /// </summary>
        [Obsolete("转而使用矩阵")]
        public static void SetInArray<T>(T[,] array, T item, int x, int y, CoordinateType coordinate)
        {
            if (coordinate == CoordinateType.Normal)
            {
                int _x = array.GetLength(0) - 1 - y;
                int _y = x;
                array[_x, _y] = item;
            }
            else if (coordinate == CoordinateType.Window)
            {
                array[y, x] = item;
            }
        }

        /// <summary>
        /// 旋转数组
        /// </summary>
        public static T[,] RotateArray<T>(T[,] array, RotationAngle angle)
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