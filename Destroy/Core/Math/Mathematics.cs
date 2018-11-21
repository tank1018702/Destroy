using System.Collections.Generic;

namespace Destroy
{
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
            if (min > max)
            {
                return x;
            }

            if (x < min)
            {
                x = min;
            }
            else if (x > max)
            {
                x = max;
            }

            return x;
        }

        /// <summary>
        /// 插入排序
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
        /// 插入排序
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
    }
}