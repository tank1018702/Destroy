namespace Destroy.Script
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal static class Program
    {
        private const string classTemplate =
@"
public class {0}
{1}
    #
{2}
";
        /// <summary>
        /// 获取数据表
        /// </summary>
        internal static DataTable GetDataTable(string text, out bool genCS, out bool genId, out string className)
        {
            //默认不导出.cs文件, 不生成Id列
            genCS = false;
            genId = false;
            className = null;

            //检查文本
            if (string.IsNullOrEmpty(text))
                return null;

            //开始根据回车分割行
            text = text.Replace("\r", "");                //把换行符变为空字符串
            text = text.Replace("\n", "&");               //把\n变为&(连字符)
            List<string> list = text.Split('&').ToList(); //然后以连字符为标准进行拆分, 转换成List

            #region 分析第一行

            string firstLine = list.First();
            //第一行不为空并且首字符为 ! 符号
            if (!string.IsNullOrEmpty(firstLine) && firstLine.First() == '!')
            {
                //去除掉 ! 符号
                firstLine = firstLine.Substring(1);
                //把第一行以 , 符号进行分割
                string[] items = firstLine.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    //去除掉每一项前后的所有空格
                    items[i] = items[i].Trim();
                    //忽略空字符串
                    if (string.IsNullOrEmpty(items[i]))
                        continue;
                    //生成.cs模板文件
                    if (items[i] == "gencs")
                        genCS = true;
                    //生成Id列
                    else if (items[i] == "genid")
                        genId = true;
                    //指定类名
                    else if (items[i].Length > 8 && items[i].Substring(0, 5) == "class")
                    {
                        //去除class, 留下(xxxx)
                        items[i] = items[i].Substring(5);
                        //去除()
                        items[i] = items[i].Substring(1, items[i].Length - 2);
                        //如果开头有数组则加上下划线_
                        if (int.TryParse(items[i].First().ToString(), out int r))
                            items[i] = items[i].Insert(0, "_");

                        className = items[i];
                    }
                }
                //分析完毕, 移除该行。
                list.RemoveAt(0);
            }

            #endregion

            #region 保留 [ ] 行

            //特殊行
            List<string> empties = new List<string>();              //空白行
            List<string> notes = new List<string>();                //注释行
            List<string> errors = new List<string>();               //错误行
            //检测特殊行
            foreach (var each in list)
            {
                if (string.IsNullOrEmpty(each))                     //空白行
                    empties.Add(each);
                else if (each.First() == '#')                       //注释行
                    notes.Add(each);
                else if (each.First() != '[' || each.Last() != ']') //错误行:不包含[]
                    errors.Add(each);
                else if (each.First() == '[' && each.Last() == ']') //错误行:只包含[]
                {
                    if (each.Length == 2)
                        errors.Add(each);
                }
            }
            //删除
            empties.ForEach(each => list.Remove(each));             //空白行
            notes.ForEach(each => list.Remove(each));               //注释行
            errors.ForEach(each => list.Remove(each));              //错误行
            empties = null;
            notes = null;
            errors = null;

            #endregion

            #region 指定行数列数 (行数最大:int.MaxValue, 列数最大:100, 数值行起始ID:1)

            int valueLineIndex = 1, lineNumber, columnNumber;

            //检查所有行
            lineNumber = list.Count;
            if (lineNumber == 0)
                return null;

            //检查第一列
            string[] columns = new DataLine(list[0], 100).Columns;
            if (columns == null)
                return null;
            columnNumber = columns.Length;
            if (columnNumber == 0)
                return null;

            #endregion

            #region 编译所有行

            //string[] => List<DataLine>
            List<DataLine> dataList = new List<DataLine>();
            foreach (var each in list)
            {
                //创建数据行
                DataLine data = new DataLine(each, columnNumber);
                //只添加编译正确的行
                if (data.Type != DataLine.DataType.Error)
                    dataList.Add(data);
            }

            #endregion

            //数据表
            DataTable dataTable = new DataTable(valueLineIndex, dataList);

            //创建
            bool suc = dataTable.Creat();
            if (!suc)
                return null;

            return dataTable;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        internal static string Read4File(string path, string fileNameEx)
        {
            string absolutePath = $@"{path}\{fileNameEx}";

            string text = Read4File(absolutePath);
            return text;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        internal static string Read4File(string absolutePath)
        {
            //不存在就返回空
            if (!File.Exists(absolutePath))
                return null;

            //转换文件编码(ASCII -> UTF8)(如果是UTF8编码则不进行转换)
            Convert2UTF8(absolutePath);

            using (FileStream stream = new FileStream(absolutePath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    //同步读取
                    string text = reader.ReadToEnd();
                    return text;
                }
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        private static bool Write2File(string path, string fileNameEx, string content)
        {
            string absolutePath = $@"{path}\{fileNameEx}";

            //打开, 如果存在就覆盖该文件
            using (FileStream stream = new FileStream($@"{path}\{fileNameEx}", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    //同步写入
                    writer.Write(content);
                    return true;
                }
            }
        }

        /// <summary>
        /// 转换文件编码格式
        /// </summary>
        private static void Convert2UTF8(string absolutePath)
        {
            Encoding readEncoding = null;
            //先读取两个字节判断现文件是什么编码格式
            using (FileStream stream = new FileStream(absolutePath, FileMode.Open, access: FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] buffer = reader.ReadBytes(2);

                    if (buffer[0] >= 0xEF)
                    {
                        if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                            readEncoding = Encoding.UTF8;
                        else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                            readEncoding = Encoding.BigEndianUnicode;
                        else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                            readEncoding = Encoding.Unicode;
                        else
                            readEncoding = Encoding.Default;
                    }
                    else
                        readEncoding = Encoding.Default;
                }
            }
            //如果文件本身是UTF8格式则不需要转格式
            if (readEncoding == Encoding.UTF8)
                return;

            string text = null;
            using (StreamReader reader = new StreamReader(absolutePath, readEncoding, false))
            {
                text = reader.ReadToEnd();
            }
            using (StreamWriter writer = new StreamWriter(absolutePath, false, Encoding.UTF8))
            {
                writer.Write(text);
            }
        }

        /// <summary>
        /// 编译
        /// </summary>
        private static bool Compile(string fileName)
        {
            string text = Read4File(Environment.CurrentDirectory, fileName + ".txt");
            //获取数据表
            DataTable dataTable = GetDataTable(text, out bool genCS, out bool genId, out string className);
            if (dataTable == null)
                return false;

            //生成.cs模板
            if (genCS)
            {
                if (className == null)
                    className = fileName;
                //生成CS模板代码
                string code = Convert2CSPrefab(className, genId, dataTable.TypeLine.Columns, dataTable.IdenLine.Columns);
                //写入文件
                bool suc = Write2File(Environment.CurrentDirectory, fileName + ".cs", code);
                if (suc)
                    Console.WriteLine("创建.cs模板成功");
                else
                    Console.WriteLine("创建.cs模板失败");
            }

            return true;
        }

        /// <summary>
        /// 转换成C#代码
        /// </summary>
        private static string Convert2CSPrefab(string className, bool genId, string[] types, string[] idens)
        {
            string temp = classTemplate;            //复制一份C#代码模板

            int signIndex = temp.IndexOf('#');      //寻找#符号
            temp = temp.Replace("#", "");           //清除#符号

            //生成Id列
            if (genId)
            {
                string idStr = "public int Id;\n    ";  //Id字符串
                temp = temp.Insert(signIndex, idStr);   //插入Id字符串
                signIndex += idStr.Length;              //使相应的插入点提升相应长度
            }

            //在#符号的位置注入数据类型与字段
            for (int i = types.Length - 1; i >= 0; i--)
            {
                //把标识行第一个改为大写
                char[] idensChars = idens[i].ToCharArray();
                string upperFirt = idensChars[0].ToString().ToUpper();
                idensChars[0] = char.Parse(upperFirt);

                //插入代码
                temp = temp.Insert(signIndex, $"public {types[i]} {new string(idensChars)};\n    ");
            }
            //为了防止源字符串中包含{ }, 把源字符串中的{ }替换掉
            temp = string.Format(temp, className, "{", "}");

            return temp;
        }

        /// <summary>
        /// 接收用户输入
        /// </summary>
        private static string AcceptInput(string msg)
        {
            Console.WriteLine(msg);
            string input = Console.ReadLine();
            return input;
        }

        /// <summary>
        /// 打印输出
        /// </summary>
        private static void Print(string msg)
        {
            Console.WriteLine(msg);
            Console.ReadKey();
        }

        /// <summary>
        /// 程序入口
        /// </summary>
        private static void Main()
        {
            string input = AcceptInput("请输入Porsche源代码文件名:");
            bool result = Compile(input);
            if (result)
                Print("编译成功");
            else
                Print("编译失败");
        }
    }
}