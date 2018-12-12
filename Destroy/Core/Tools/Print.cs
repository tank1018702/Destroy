namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class Print
    {
        /// <summary>
        /// 返回一个字符的宽度 待优化
        /// </summary>
        public static int CharWide(char c)
        {
            //12/11打了个补丁,制表符宽度算1
            if(c >=  0x2500 &&c<= 0x257F)
            {
                return 1;
            }
            //只要不低于127都算chinese算了
            else if (c >= 0x4e00 && c <= 0x9fbb)
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

        //将一个数字转换为两格一共4位的字符串
        public static string NumToStrW4(int n)
        {
            string numStr;
            if (n > 999)
            {
                numStr = " 999";
            }
            else if (n >= 0)
            {
                numStr = " " + n.ToString("D3");
            }
            else if (n > -999)
            {
                numStr = n.ToString("D3");
            }
            else
            {
                numStr = "-999";
            }
            return numStr;
        }

        /// <summary>
        /// 将String分割成适合于单个格子的数组 单字符会被转换为 char + 空格 双字符会直接输出
        /// </summary>
        public static List<string> DivStr(string str)
        {
            List<string> list = new List<string>();
            int outputLength = 0;
            string output = "";
            foreach(var v in str)
            {
                if(CharWide(v) == 1)
                {
                    if(outputLength == 0)
                    {
                        outputLength = 1;
                        output = v.ToString();
                    }
                    else
                    {
                        outputLength = 0;
                        output += v.ToString();
                        list.Add(output);
                        output = "";
                    }
                }
                else if(CharWide(v) == 2)
                {
                    if(outputLength == 1)
                    {
                        //如果当前有缓存,那么先把缓存输出
                        outputLength = 0;
                        output += " ";
                        list.Add(output);
                        output = "";
                    }
                    //再把自己输出
                    list.Add(v.ToString());
                }
            }
            if(outputLength == 1)
            {
                outputLength = 0;
                output += " ";
                list.Add(output);
                output = "";
            }
            return list;
        }
    }
}