namespace Destroy.Script
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 数据表
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// 下标
        /// </summary>
        private int dataLineIndex;

        /// <summary>
        /// 行数
        /// </summary>
        public int LineNumber
        {
            get
            {
                if (AllLines != null)
                    return AllLines.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnNumber
        {
            get
            {
                if (AllLines != null && AllLines.Count > 0 && AllLines[0].Columns != null)
                    return AllLines[0].Columns.Length;
                else
                    return 0;
            }
        }

        /// <summary>
        /// 所有行
        /// </summary>
        private List<DataLine> AllLines;

        /// <summary>
        /// 标识行
        /// </summary>
        public DataLine IdenLine;

        /// <summary>
        /// 类型行
        /// </summary>
        public DataLine TypeLine;

        /// <summary>
        /// 所有数据行
        /// </summary>
        public List<DataLine> ValueLines;

        /// <summary>
        /// 初始化
        /// </summary>
        public DataTable(int valueLineIndex, List<DataLine> allLines)
        {
            this.dataLineIndex = valueLineIndex;

            this.AllLines = allLines;
            this.ValueLines = new List<DataLine>();
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        public bool Creat()
        {
            if (AllLines == null)
                return false;
            //遍历数据行
            for (int i = 0; i < this.AllLines.Count; i++)
            {
                //检测类型
                switch (this.AllLines[i].Type)
                {
                    case DataLine.DataType.Iden:
                        this.IdenLine = this.AllLines[i];
                        break;
                    case DataLine.DataType.Type:
                        this.TypeLine = this.AllLines[i];
                        break;
                    case DataLine.DataType.Value:
                        {
                            //设置Id(必须借助中间变量)
                            var temp = this.AllLines[i];
                            temp.Id = this.dataLineIndex++; //先赋值后+1
                            this.AllLines[i] = temp;
                            //Id只作用于数据行
                            this.ValueLines.Add(this.AllLines[i]);
                        }
                        break;
                }
            }
            //检查数据表
            if (this.IdenLine == null)
            {
                Console.WriteLine("没有标识行!");
                return false;
            }
            if (this.TypeLine == null)
            {
                Console.WriteLine("没有类型行!");
                return false;
            }
            return true;
        }
    }
}