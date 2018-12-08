namespace Destroy.Scripting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Porsche助手 <see langword="static"/>
    /// </summary>
    public static class PorscheHelper
    {
        /// <summary>
        /// 获取DLL路径下的数据表
        /// </summary>
        public static DataTable GetDataTable(string fileName)
        {
            string text = Program.Read4File(Environment.CurrentDirectory, fileName + ".txt");
            //创建数据表
            DataTable dataTable = Program.GetDataTable(text, out bool exportCS, out bool genId, out string className);
            return dataTable;
        }

        /// <summary>
        /// 获取指定路径下的数据表
        /// </summary>
        public static DataTable GetDataTableDirect(string absolutePath)
        {
            string text = Program.Read4File(absolutePath);
            //创建数据表
            DataTable dataTable = Program.GetDataTable(text, out bool exportCS, out bool genId, out string className);
            return dataTable;
        }

        /// <summary>
        /// 获取指定文本的数据表
        /// </summary>
        public static DataTable GetDataTableByText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;
            DataTable dataTable = Program.GetDataTable(text, out bool exportCS, out bool genId, out string className);
            return dataTable;
        }


        /// <summary>
        /// 反序列化
        /// </summary>
        public static T Deserialize<T>(DataLine idenLine, DataLine typeLine, DataLine valueLine) where T : class, new()
        {
            try
            {
                T instance = new T();   //创建实例
                Type t = typeof(T);     //获取类型
                //给Id赋值
                FieldInfo idField = t.GetField("Id");
                //如果有Id字段就赋值
                if (idField != null)
                    idField.SetValue(instance, valueLine.Id);

                //循环标识列次数, 给其他字段赋值
                for (int i = 0; i < idenLine.Columns.Length; i++)
                {
                    //ID首字母大写
                    string iden = idenLine.Columns[i];
                    char[] charArray = iden.ToCharArray();
                    string upper = charArray[0].ToString().ToUpper();
                    charArray[0] = char.Parse(upper);
                    iden = new string(charArray);

                    string type = typeLine.Columns[i];
                    string value = valueLine.Columns[i];

                    object typeValue = null;

                    FieldInfo field = t.GetField(iden);
                    switch (type)
                    {
                        case "int":
                            typeValue = int.Parse(value);
                            break;
                        case "float":
                            typeValue = float.Parse(value);
                            break;
                        case "string":
                            typeValue = value;
                            break;
                        case "bool":
                            typeValue = bool.Parse(value);
                            break;
                    }
                    //注入值
                    field.SetValue(instance, typeValue);
                }

                return instance;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// DLL路径下的反序列化
        /// </summary>
        public static List<T> Deserialize<T>(string fileName) where T : class, new()
        {
            DataTable dataTable = GetDataTable(fileName);
            //数据表不能为空且有数据行
            if (dataTable == null || dataTable.ValueLines.Count == 0)
                return null;

            List<T> instances = new List<T>();
            foreach (var valueLine in dataTable.ValueLines)
            {
                T instance = Deserialize<T>(dataTable.IdenLine, dataTable.TypeLine, valueLine);
                instances.Add(instance);
            }
            return instances;
        }

        /// <summary>
        /// Porsche反序列化
        /// </summary>
        public static List<T> DeserializeByText<T>(string text) where T : class, new()
        {
            DataTable dataTable = GetDataTableByText(text);
            if (dataTable == null || dataTable.ValueLines.Count == 0)
                return null;

            List<T> instances = new List<T>();
            foreach (var valueLine in dataTable.ValueLines)
            {
                T instance = Deserialize<T>(dataTable.IdenLine, dataTable.TypeLine, valueLine);
                instances.Add(instance);
            }
            return instances;
        }


        /// <summary>
        /// Porsche序列化
        /// </summary>
        public static string Serialize<T>(T instance) where T : class => Serialize(new List<T> { instance });

        /// <summary>
        /// Porsche序列化
        /// </summary>
        public static string Serialize<T>(List<T> list) where T : class
        {
            try
            {
                if (list.Count == 0)
                    return null;

                Type t = typeof(T);

                //获取类所有字段
                FieldInfo[] classFields = t.GetFields();

                string idenSplit = ", @";
                string typeSplit = ", ~";
                string valueSplit = ", #";

                StringBuilder idenLine = new StringBuilder();
                StringBuilder typeLine = new StringBuilder();
                List<StringBuilder> valueLines = new List<StringBuilder>();
                foreach (var each in list)
                {
                    valueLines.Add(new StringBuilder());
                }

                foreach (var field in classFields)
                {
                    //标识行
                    string identity = field.Name;
                    idenLine.Append(idenSplit + identity);
                    //类型行
                    string type = null;
                    string dotNetName = field.GetValue(list[0]).GetType().Name;
                    switch (dotNetName)
                    {
                        case "String":
                            type = "string";
                            break;
                        case "Int32":
                            type = "int";
                            break;
                        case "Single":
                            type = "float";
                            break;
                        case "Boolean":
                            type = "bool";
                            break;
                    }
                    typeLine.Append(typeSplit + type);
                    //数值行
                    for (int i = 0; i < list.Count; i++)
                    {
                        string value = field.GetValue(list[i]).ToString();
                        valueLines[i].Append(valueSplit + value);
                    }
                }

                //切割首部2个长度
                idenLine.Remove(0, 2);
                //插入[ ]
                idenLine.Insert(0, "[");
                idenLine.Insert(idenLine.Length, "]");

                //切割首部2个长度
                typeLine.Remove(0, 2);
                //插入[ ]
                typeLine.Insert(0, "[");
                typeLine.Insert(typeLine.Length, "]");

                StringBuilder result = new StringBuilder();
                //插入标识行
                result.Append(idenLine.ToString());
                result.Append("\r\n");
                //插入类型行
                result.Append(typeLine.ToString());
                result.Append("\r\n");
                //插入数据行
                foreach (var each in valueLines)
                {
                    //切割首部2个长度
                    each.Remove(0, 2);
                    //插入[ ]
                    each.Insert(0, "[");
                    each.Insert(each.Length, "]");
                    //添加行
                    result.Append(each.ToString());

                    //换行
                    result.Append("\r\n");
                }

                return result.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 二进制序列化
        /// </summary>
        public static byte[] Serialize2Bit<T>(T instance) where T : class
        {
            try
            {
                if (instance == null)
                    return null;
                List<byte> list = new List<byte>();
                //获取类型
                Type t = instance.GetType();
                //获取类所有字段
                FieldInfo[] classFields = t.GetFields();
                //顺序遍历类的字段
                foreach (var field in classFields)
                {
                    byte[] data = null;
                    //该字段的值
                    object value = field.GetValue(instance);
                    //该字段类型
                    string dotNetName = field.FieldType.Name;
                    //支持4种基础类型
                    switch (dotNetName)
                    {
                        case "String": //2 + n个字节
                            {
                                byte[] str = Encoding.UTF8.GetBytes((string)value);
                                byte[] strLen = BitConverter.GetBytes((ushort)str.Length); //两个字节(最多允许65535个字)
                                //合并两个数组
                                data = new byte[str.Length + strLen.Length];
                                strLen.CopyTo(data, 0);          //先2个字节
                                str.CopyTo(data, strLen.Length); //再加上字符串
                            }
                            break;
                        case "Int32": //4个字节
                            {
                                data = BitConverter.GetBytes((int)value);
                            }
                            break;
                        case "Single": //4个字节
                            {
                                data = BitConverter.GetBytes((float)value);
                            }
                            break;
                        case "Boolean": //1个字节
                            {
                                data = BitConverter.GetBytes((bool)value);
                            }
                            break;
                    }
                    //添加
                    list.AddRange(data);
                }
                //返回结果
                return list.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 二进制反序列化
        /// </summary>
        public static T Deserialize4Bit<T>(byte[] data) where T : class, new()
        {
            try
            {
                if (data == null)
                    return null;
                T instance = new T();
                //获取类型
                Type t = typeof(T);
                //获取类所有字段
                FieldInfo[] classFields = t.GetFields();
                //通过流的方式读取数据
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                    {
                        //顺序遍历类的字段
                        foreach (var field in classFields)
                        {
                            object value = null;
                            //字段类型
                            string dotNetName = field.FieldType.Name;
                            switch (dotNetName)
                            {
                                case "String": //2 + n个字节
                                    {
                                        ushort len = reader.ReadUInt16();
                                        byte[] strBytes = reader.ReadBytes(len);
                                        value = Encoding.UTF8.GetString(strBytes);
                                    }
                                    break;
                                case "Int32": //4个字节
                                    {
                                        value = reader.ReadInt32();
                                    }
                                    break;
                                case "Single": //4个字节
                                    {
                                        value = reader.ReadSingle();
                                    }
                                    break;
                                case "Boolean": //1个字节
                                    {
                                        value = reader.ReadBoolean();
                                    }
                                    break;
                            }
                            //注入值
                            field.SetValue(instance, value);
                        }
                    }
                }
                //返回结果
                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}