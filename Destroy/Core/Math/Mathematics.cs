namespace Destroy
{
    using System;
    using System.Collections.Generic;

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
        public static int ClampInt(int i, int min, int max)
        {
            i = (i > max) ? max : i;
            i = (i < min) ? min : i;

            return i;
        }

        #region 插入排序

        /// <summary>
        /// 插入排序 (从小到大)
        /// </summary>
        public static void InsertionSort(List<KeyValuePair<int, object>> pairs)
        {
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
        public static void InsertionSort(List<int> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                int j = i;
                while (j > 0 && list[j] < list[j - 1])
                {
                    var temp = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = temp;
                    j--;
                }
            }
        }

        /// <summary>
        /// 插入排序 (从小到大)
        /// </summary>
        public static void InsertionSort(List<uint> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                int j = i;
                while (j > 0 && list[j] < list[j - 1])
                {
                    var temp = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = temp;
                    j--;
                }
            }
        }

        #endregion

        #region 快速排序

        /// <summary>
        /// 快速排序 (从小到大)
        /// </summary>
        public static void QuickSort(List<KeyValuePair<int, object>> pairs)
        {
            _QuickSort(pairs, 0, pairs.Count - 1);

            void _QuickSort(List<KeyValuePair<int, object>> list, int low, int hign)
            {
                if (low >= hign)
                    return;
                int temp = list[low].Key;
                int i = low + 1;
                int j = hign;
                while (true)
                {
                    while (list[j].Key > temp) j--;
                    while (list[i].Key < temp && i < j) i++;
                    if (i >= j)
                        break;
                    //Swap
                    var t = list[i];
                    list[i] = list[j];
                    list[j] = t;
                    i++;
                    j--;
                }
                if (j != low)
                {
                    var t = list[low];
                    list[low] = list[j];
                    list[j] = t;
                }
                _QuickSort(list, j + 1, hign);
                _QuickSort(list, low, j - 1);
            }
        }

        /// <summary>
        /// 快速排序 (从小到大)
        /// </summary>
        public static void QuickSort(List<KeyValuePair<uint, object>> pairs)
        {
            _QuickSort(pairs, 0, pairs.Count - 1);

            void _QuickSort(List<KeyValuePair<uint, object>> list, int low, int hign)
            {
                if (low >= hign)
                    return;
                uint temp = list[low].Key;
                int i = low + 1;
                int j = hign;
                while (true)
                {
                    while (list[j].Key > temp) j--;
                    while (list[i].Key < temp && i < j) i++;
                    if (i >= j)
                        break;
                    //Swap
                    var t = list[i];
                    list[i] = list[j];
                    list[j] = t;
                    i++;
                    j--;
                }
                if (j != low)
                {
                    var t = list[low];
                    list[low] = list[j];
                    list[j] = t;
                }
                _QuickSort(list, j + 1, hign);
                _QuickSort(list, low, j - 1);
            }
        }

        /// <summary>
        /// 快速排序 (从小到大)
        /// </summary>
        public static void QuickSort(List<int> list)
        {
            _QuickSort(list, 0, list.Count - 1);

            void _QuickSort(List<int> param, int low, int hign)
            {
                if (low >= hign)
                    return;
                int temp = param[low];
                int i = low + 1;
                int j = hign;
                while (true)
                {
                    while (param[j] > temp) j--;
                    while (param[i] < temp && i < j) i++;
                    if (i >= j)
                        break;
                    //Swap
                    var t = param[i];
                    param[i] = param[j];
                    param[j] = t;
                    i++;
                    j--;
                }
                if (j != low)
                {
                    var t = param[low];
                    param[low] = param[j];
                    param[j] = t;
                }
                _QuickSort(param, j + 1, hign);
                _QuickSort(param, low, j - 1);
            }
        }

        /// <summary>
        /// 快速排序 (从小到大)
        /// </summary>
        public static void QuickSort(List<uint> list)
        {
            _QuickSort(list, 0, list.Count - 1);

            void _QuickSort(List<uint> param, int low, int hign)
            {
                if (low >= hign)
                    return;
                uint temp = param[low];
                int i = low + 1;
                int j = hign;
                while (true)
                {
                    while (param[j] > temp) j--;
                    while (param[i] < temp && i < j) i++;
                    if (i >= j)
                        break;
                    //Swap
                    var t = param[i];
                    param[i] = param[j];
                    param[j] = t;
                    i++;
                    j--;
                }
                if (j != low)
                {
                    var t = param[low];
                    param[low] = param[j];
                    param[j] = t;
                }
                _QuickSort(param, j + 1, hign);
                _QuickSort(param, low, j - 1);
            }
        }

        #endregion

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