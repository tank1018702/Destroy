namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /*
     * 12/7 by Kyasever
     * Renderer通过继承分为了三个组件:
     * StringRenderer 用于渲染一句字符串
     * PosRenderer 用于渲染一个点
     * GroupRenderer 用于渲染一组图形
     * 之后版本要重新优化
     */

    public abstract class Renderer : Component
    {
        /// <summary>
        /// 为0时脚本显示优先级最高(最后被渲染), 然后向着数轴正方向递减。
        /// </summary>
        public uint Depth;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;



        public Renderer()
        {
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public virtual string GetStr()
        {
            return "";
        }
    }

    /// <summary>
    /// 字符串渲染器 用于渲染一行
    /// </summary>
    public class StringRenderer : Renderer
    {
        private string str;
        public string Str
        {
            get => str;
            //一个Renderer相当于一个点,这两个都相当于点集
            //赋值的时候自动计算这个字符串的占位长度,并生成对应数量的Renderer
            set
            {
                int sum = 0;
                foreach (char v in value)
                {
                    sum += Print.CharWide(v);
                }
                length = sum;
                str = value;
            }
        }
        //保存着这个字符串的长度
        public int length;

        public StringRenderer(string s)
        {
            Str = s;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public StringRenderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public override string GetStr()
        {
            return str;
        }
    }

    /// <summary>
    /// 点渲染器 用于提供最多不超过2宽度的一个点
    /// </summary>
    public class PosRenderer : Renderer
    {
        private string str;
        public string Str
        {
            get => str;
            set
            {
                StringBuilder builder = new StringBuilder();
                int sum = 0;
                foreach (char v in value)
                {
                    sum += Print.CharWide(v);
                    //强制截断长度一个标准Renderer单位的字符
                    if (sum > Camera.main.CharWidth)
                    {
                        break;
                    }
                    else
                    {
                        builder.Append(v);
                    }
                }
                str = builder.ToString();
            }
        }

        public PosRenderer(string s)
        {
            Str = s;
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public PosRenderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public override string GetStr()
        {
            return str;
        }
    }

    /// <summary>
    /// 偏移渲染器 用于渲染一个点,其中有一个偏移值表示渲染的偏移
    /// </summary>
    internal class DeviationRenderer : Renderer
    {
        private string str;
        public string Str
        {
            get => str;
            set
            {
                StringBuilder builder = new StringBuilder();
                int sum = 0;
                foreach (char v in value)
                {
                    sum += Print.CharWide(v);
                    //强制截断长度一个标准Renderer单位的字符
                    if (sum > Camera.main.CharWidth)
                    {
                        break;
                    }
                    else
                    {
                        builder.Append(v);
                    }
                }
                str = builder.ToString();
            }
        }
        public Vector2Int deviation;

        public DeviationRenderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
            deviation = new Vector2Int(0, 0);
        }
    }

    /// <summary>
    /// 组渲染器,包含一系列点
    /// </summary>
    public class GroupRenderer : Renderer
    {
        public List<KeyValuePair<Renderer, Vector2Int>> list;

        public GroupRenderer()
        {
            list = new List<KeyValuePair<Renderer, Vector2Int>>();
        }
    }


}