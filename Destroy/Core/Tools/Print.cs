namespace Destroy
{
    using System;
    using System.Text;

    public static class Print
    {
        public static void Draw(object msg) => Console.Write(msg);

        public static void Draw(object msg, ConsoleColor foreColor)
        {
            Console.ForegroundColor = foreColor;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void Draw(object msg, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.Write(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public static void DrawLine(object msg) => Console.WriteLine(msg);

        public static void DrawLine(object msg, ConsoleColor foreColor)
        {
            Console.ForegroundColor = foreColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void DrawLine(object msg, ConsoleColor foreColor, ConsoleColor backColor)
        {
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// 返回一个字符的宽度 待优化
        /// </summary>
        public static int CharWide(char c)
        {
            //只要不低于127都算chinese算了
            if (c >= 0x4e00 && c <= 0x9fbb)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 返回一个字符串的长度
        /// </summary>
        public static int StrWide(string str)
        {
            int sum = 0;
            foreach(var v in str)
            {
                sum += CharWide(v);
            }
            return sum;
        }

        /// <summary>
        /// 按照标准长度截断字符串,不足用空格补上,超出截断
        /// </summary>
        /// <param name="str"></param>
        /// <param name="wide"></param>
        /// <returns></returns>
        public static string SubStr(string str,int wide)
        {
            StringBuilder builder = new StringBuilder();
            int sum = 0;
            foreach (var v in str)
            {
                sum += Print.CharWide(v);
                //当超出时返回
                if (sum>wide)
                {
                    return builder.ToString();
                }
                else
                {
                    builder.Append(v);
                }
            }
            if(sum == wide)
            {
                return builder.ToString();
            }
            if(sum < wide)
            {
                for(int i = 0;i<wide-sum;i++)
                {
                    builder.Append(' ');
                }
            }
            return builder.ToString();
        }
    }
}