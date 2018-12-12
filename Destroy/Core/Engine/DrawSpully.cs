using System;
using System.Drawing;
using System.Text;

namespace Destroy
{
    /// <summary>
    /// 生成一个DrawCall给渲染引擎处理
    /// 包含位置,前后颜色,和要输出的字符,引擎再说处理的问题
    /// </summary>
    public struct DrawCall
    { 
        public int X;

        public int Y;

        public EngineColor ForeColor;

        public EngineColor BackColor;

        public string Str;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DrawCall~: X:");
            sb.Append(X.ToString());
            sb.Append(" Y:");
            sb.Append(Y.ToString());
            sb.Append(" FC:");
            sb.Append(ForeColor.ToString());
            sb.Append(" BC:");
            sb.Append(BackColor.ToString());
            sb.Append(" S:");
            sb.Append(Str);
            sb.Append(" SL:");
            sb.Append(Str.Length);
            return sb.ToString();
        }
    }

    /// <summary>
    /// 使用方法同ConsoleColor
    /// </summary>
    public struct EngineColor
    {
        internal enum ColorType
        {
            Black, Blue, Green, Cyan, Red, Gray, Magenta, Yellow,
            White, DarkBlue, DarkGreen, DarkCyan, DarkRed, DarkMagenta, DarkYellow, DarkGray, Unknown
        }
        internal ColorType colorType;

        public static EngineColor Black = new EngineColor(ColorType.Black);
        public static EngineColor Blue = new EngineColor(ColorType.Blue);
        public static EngineColor Green = new EngineColor(ColorType.Green);
        public static EngineColor Cyan = new EngineColor(ColorType.Cyan);
        public static EngineColor Red = new EngineColor(ColorType.Red);
        public static EngineColor Gray = new EngineColor(ColorType.Gray);
        public static EngineColor Magenta = new EngineColor(ColorType.Magenta);
        public static EngineColor Yellow = new EngineColor(ColorType.Yellow);
        public static EngineColor White = new EngineColor(ColorType.White);
        public static EngineColor DarkBlue = new EngineColor(ColorType.DarkBlue);
        public static EngineColor DarkGreen = new EngineColor(ColorType.DarkGreen);
        public static EngineColor DarkCyan = new EngineColor(ColorType.DarkCyan);
        public static EngineColor DarkRed = new EngineColor(ColorType.DarkRed);
        public static EngineColor DarkMagenta = new EngineColor(ColorType.DarkMagenta);
        public static EngineColor DarkYellow = new EngineColor(ColorType.DarkYellow);
        public static EngineColor DarkGray = new EngineColor(ColorType.DarkGray);

        //通过种类初始化,种类不对外暴漏,只使用默认颜色创建
        private EngineColor(ColorType type)
        {
            colorType = type;
        }

        public override string ToString()
        {
            return colorType.ToString();
        }

        public static bool operator ==(EngineColor left, EngineColor right)
        {
            return left.colorType == right.colorType;
        }

        public static bool operator !=(EngineColor left, EngineColor right)
        {
            return left.colorType != right.colorType;
        }

        /// <summary>
        /// 通过Drawing.Color创建一种颜色
        /// </summary>
        /// <param name="color"></param>
        public EngineColor(Color color)
        {
            colorType = ColorType.Unknown;
            //this.color = color;
        }

        /// <summary>
        /// 转换为ConsoleColor供C#原生渲染器调用. 从外部来看就是使用的EngineColor
        /// </summary>
        /// <returns></returns>
        internal ConsoleColor ToConsoleColor()
        {
            switch (colorType)
            {
                case ColorType.Black:
                    return ConsoleColor.Black;
                case ColorType.Blue:
                    return ConsoleColor.Blue;
                case ColorType.Green:
                    return ConsoleColor.Green;
                case ColorType.Cyan:
                    return ConsoleColor.Cyan;
                case ColorType.Red:
                    return ConsoleColor.Red;
                case ColorType.Gray:
                    return ConsoleColor.Gray;
                case ColorType.Magenta:
                    return ConsoleColor.Magenta;
                case ColorType.Yellow:
                    return ConsoleColor.Yellow;
                case ColorType.White:
                    return ConsoleColor.White;

                case ColorType.DarkBlue:
                    return ConsoleColor.DarkBlue;
                case ColorType.DarkGreen:
                    return ConsoleColor.DarkGreen;
                case ColorType.DarkCyan:
                    return ConsoleColor.DarkCyan;
                case ColorType.DarkRed:
                    return ConsoleColor.DarkRed;
                case ColorType.DarkMagenta:
                    return ConsoleColor.DarkMagenta;
                case ColorType.DarkYellow:
                    return ConsoleColor.DarkYellow;
                case ColorType.DarkGray:
                    return ConsoleColor.DarkGray;

                default:
                    return ConsoleColor.Black;
                    //return ToNearestConsoleColor(color);
            }
        }

        /// <summary>
        /// Converts the specified <see cref="Color"/> to it's nearest <see cref="ConsoleColor"/> equivalent.
        /// </summary>
        /// <remarks>Code taken from Glenn Slayden at https://stackoverflow.com/questions/1988833/converting-color-to-consolecolor</remarks>
        public static ConsoleColor ToNearestConsoleColor(Color color)
        {
            ConsoleColor closestConsoleColor = 0;
            double delta = double.MaxValue;

            foreach (ConsoleColor consoleColor in Enum.GetValues(typeof(ConsoleColor)))
            {
                string consoleColorName = Enum.GetName(typeof(ConsoleColor), consoleColor);
                consoleColorName = string.Equals(consoleColorName, nameof(ConsoleColor.DarkYellow), StringComparison.Ordinal) ? nameof(Color.Orange) : consoleColorName;
                Color rgbColor = Color.FromName(consoleColorName);
                double sum = Math.Pow(rgbColor.R - color.R, 2.0) + Math.Pow(rgbColor.G - color.G, 2.0) + Math.Pow(rgbColor.B - color.B, 2.0);

                double epsilon = 0.001;
                if (sum < epsilon)
                {
                    return consoleColor;
                }

                if (sum < delta)
                {
                    delta = sum;
                    closestConsoleColor = consoleColor;
                }
            }
            return closestConsoleColor;
        }
    }
}