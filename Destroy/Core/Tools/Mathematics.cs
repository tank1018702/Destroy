namespace Destroy
{
    public class Mathematics
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
    }
}