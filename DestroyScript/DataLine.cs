namespace Destroy.Script
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 数据行
    /// </summary>
    public class DataLine
    {
        /// <summary>
        /// 数据行类型
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// 错误
            /// </summary>
            Error,
            /// <summary>
            /// 标识
            /// </summary>
            Iden,
            /// <summary>
            /// 类型
            /// </summary>
            Type,
            /// <summary>
            /// 数值
            /// </summary>
            Value,
        }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public DataType Type { get; private set; }

        /// <summary>
        /// 列
        /// </summary>
        public string[] Columns { get; private set; }

        /// <summary>
        /// 该行内容([]中间的字符串)
        /// </summary>
        private string content;

        /// <summary>
        /// 实例化DataLine
        /// </summary>
        public DataLine(string str, int columnNumber)
        {
            this.Id = 0;
            this.Type = DataType.Error;
            this.Columns = null;
            this.content = str.Substring(1, str.Length - 2); //去除掉[]
            this.Columns = Compile(columnNumber);              //编译
        }

        /// <summary>
        /// 编译该行
        /// </summary>
        private string[] Compile(int columnNumber)
        {
            List<string> list;
            string[] array = new string[columnNumber];

            list = CheckIdenLine();
            if (list != null)
            {
                //如果指定列数量超过集合长度, 返回集合
                if (list.Count < columnNumber)
                    return list.ToArray();

                list.CopyTo(0, array, 0, (int)columnNumber);
                return array;
            }

            list = CheckTypeLine();
            if (list != null)
            {
                //如果指定列数量超过集合长度, 返回集合
                if (list.Count < columnNumber)
                    return list.ToArray();

                list.CopyTo(0, array, 0, (int)columnNumber);
                return array;
            }

            list = CheckValueLine();
            if (list != null)
            {
                //如果指定列数量超过集合长度, 返回集合
                if (list.Count < columnNumber)
                    return list.ToArray();

                list.CopyTo(0, array, 0, (int)columnNumber);
                return array;
            }

            return null;
        }

        /// <summary>
        /// 检查是否是类型行
        /// </summary>
        private List<string> CheckTypeLine()
        {
            //如果不是类型行
            if (content.IndexOf('~') == -1)
                return null;
            //如果是标识行
            if (content.IndexOf('@') != -1)
                return null;
            //如果是数值行
            if (content.IndexOf('#') != -1)
                return null;

            List<string> list = new List<string>();

            //类型关键字
            string[] keyword = new string[] { "string", "int", "float", "bool" };

            //根据,拆分字符串
            string[] types = content.Split(',');
            //遍历列
            for (int i = 0; i < types.Length; i++)
            {
                //去除空格
                types[i] = types[i].Trim();
                //检测 ~ 符号
                if (types[i].First() == '~')
                {
                    //去除掉 ~ 符号
                    types[i] = types[i].Substring(1);
                    //遍历每一个关键字
                    for (int j = 0; j < keyword.Length; j++)
                    {
                        //匹配上
                        if (types[i] == keyword[j])
                        {
                            list.Add(types[i]);
                        }
                    }
                }
            }
            //如果列中有无关的则返回空
            if (list.Count != types.Length)
                return null;

            Type = DataType.Type;
            return list;
        }

        /// <summary>
        /// 检查是否是标识行
        /// </summary>
        private List<string> CheckIdenLine()
        {
            //不是标识行
            if (content.IndexOf('@') == -1)
                return null;
            //是类型行
            if (content.IndexOf('~') != -1)
                return null;
            //是数值行
            if (content.IndexOf('#') != -1)
                return null;

            Type = DataType.Iden;

            List<string> list = new List<string>();

            string[] idens = content.Split(',');
            for (int i = 0; i < idens.Length; i++)
            {
                //去除空格
                idens[i] = idens[i].Trim();
                //检测 @ 符号
                if (idens[i].First() == '@')
                {
                    idens[i] = idens[i].Substring(1); //去除@符号
                    list.Add(idens[i]);
                }
            }
            return list;
        }

        /// <summary>
        /// 检查是否是数值行
        /// </summary>
        private List<string> CheckValueLine()
        {
            //不是数值行
            if (content.IndexOf('#') == -1)
                return null;
            //是类型行
            if (content.IndexOf('~') != -1)
                return null;
            //是标识行
            if (content.IndexOf('@') != -1)
                return null;

            Type = DataType.Value;

            List<string> list = new List<string>();

            string[] values = content.Split(',');
            for (int i = 0; i < values.Length; i++)
            {
                //去除空格
                values[i] = values[i].Trim();
                //检测 # 符号
                if (values[i].First() == '#')
                {
                    values[i] = values[i].Substring(1); //去除#符号
                    list.Add(values[i]);
                }
            }
            return list;
        }
    }
}