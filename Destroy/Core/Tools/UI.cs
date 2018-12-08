using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy.Core.Tools
{
    public class TextBox : Script
    {
        private GroupRenderer groupRenderer;
        public Vector2Int Position;
        public int Width;
        public int Height;

        public void Init(Vector2Int position, int w, int h)
        {
            this.Position = position;
            groupRenderer = GetComponent<GroupRenderer>();
        }

        public override void Update()
        {
            transform.Position = Camera.main.transform.Position + Position;
        }
    }

    public static class UI
    {
        /// <summary>
        /// 创建一个TextBox
        /// </summary>
        public static GameObject CreateTextBox(string name,Vector2Int pos,int w,int height)
        {
            int width = w * Camera.main.CharWidth;
            if(width < 4 || height < 3)
            {
                Debug.Error("创建TextBox需要更大的空间");
                return null;
            }

            GameObject gameObject = new GameObject("TextBox");
            //添加默认的Renderer组件
            GroupRenderer groupRenderer = gameObject.AddComponent<GroupRenderer>();
            groupRenderer.AddRenderer(new StringRenderer(BoxDrawingSupply.GetFirstLine(width)), 0, 0);
            for(int i = 1;i<height-1;i++)
            {
                groupRenderer.AddRenderer(new StringRenderer(BoxDrawingSupply.GetMiddleLine(width)), 0, -i);
            }
            groupRenderer.AddRenderer(new StringRenderer(BoxDrawingSupply.GetLastLine(width)), 0, -height+1);
            groupRenderer.Depth = -1;

            //添加TextBox组件
            TextBox textBox = gameObject.AddComponent<TextBox>();
            textBox.Init(pos,width,height);

            return gameObject;
        }
    }
    /// <summary>
    /// 用于制表符加法运算的一个辅助类
    /// </summary>
    public class BoxDrawingCharacter
    {
        private bool down, up, left, right;
        public BoxDrawingCharacter(bool up, bool down, bool left, bool right)
        {
            this.down = down;
            this.up = up;
            this.left = left;
            this.right = right;
        }
        /// <summary>
        /// 从一个字符c解析一个对象,从而可以进行运算
        /// </summary>
        public static BoxDrawingCharacter Prase(char c)
        {
            switch (c)
            {
                case '┌':
                    return BoxDownRight();
                case '┐':
                    return BoxDownLeft();
                case '└':
                    return BoxUpRight();
                case '┘':
                    return BoxUpLeft();
                case '─':
                    return BoxHorizontal();
                case '│':
                    return BoxVertical();
                case '├':
                    return BoxVerticalRight();
                case '┤':
                    return BoxVerticalLeft();
                case '┬':
                    return BoxHorizontalDown();
                case '┴':
                    return BoxHorizontalUp();
                case '┼':
                    return BoxVerticalHorizontal();
                default:
                    return new BoxDrawingCharacter(false, false, false, false);
            }

        }

        /// <summary>
        /// 从一个对象转化为一个字符,进行输出
        /// </summary>
        public char ToChar()
        {
            if (!up && down && !left && right)
            {
                return '┌';
            }
            else if (!up && down && left && !right)
            {
                return '┐';
            }
            else if (up && !down && !left && right)
            {
                return '└';
            }
            else if (up && !down && left && !right)
            {
                return '┘';
            }
            else if (!up && !down && left && right)
            {
                return '─';
            }
            else if (up && down && !left && !right)
            {
                return '│';
            }

            else if (up && down && !left && right)
            {
                return '├';
            }
            else if (up && down && left && !right)
            {
                return '┤';
            }
            else if (!up && down && left && right)
            {
                return '┬';
            }
            else if (up && !down && left && right)
            {
                return '┴';
            }
            else if (up && down && left && right)
            {
                return '┼';
            }

            else
            {
                return ' ';
            }
        }

        /// <summary>
        /// 说到底都是为了进行加法叠加运算...
        /// </summary>
        public static BoxDrawingCharacter operator +(BoxDrawingCharacter a, BoxDrawingCharacter b)
        {
            return new BoxDrawingCharacter(a.up || b.up, a.down || b.down, a.left || b.left, a.right || b.right);
        }
        #region 创建制表符对象
        public static BoxDrawingCharacter BoxDownRight()
        {
            return new BoxDrawingCharacter(false, true, false, true);
        }

        public static BoxDrawingCharacter BoxDownLeft()
        {
            return new BoxDrawingCharacter(false, true, true, false);
        }

        public static BoxDrawingCharacter BoxUpRight()
        {
            return new BoxDrawingCharacter(true, false, false, true);
        }

        public static BoxDrawingCharacter BoxUpLeft()
        {
            return new BoxDrawingCharacter(true, false, true, false);
        }

        public static BoxDrawingCharacter BoxHorizontal()
        {
            return new BoxDrawingCharacter(false, false, true, true);
        }

        public static BoxDrawingCharacter BoxVertical()
        {
            return new BoxDrawingCharacter(true, true, false, false);
        }


        public static BoxDrawingCharacter BoxVerticalRight()
        {
            return new BoxDrawingCharacter(true, true, false, true);
        }

        public static BoxDrawingCharacter BoxVerticalLeft()
        {
            return new BoxDrawingCharacter(true, true, true, false);
        }

        public static BoxDrawingCharacter BoxHorizontalDown()
        {
            return new BoxDrawingCharacter(false, true, true, true);
        }

        public static BoxDrawingCharacter BoxHorizontalUp()
        {
            return new BoxDrawingCharacter(true, false, true, true);
        }

        public static BoxDrawingCharacter BoxVerticalHorizontal()
        {
            return new BoxDrawingCharacter(true, true, true, true);
        }
        #endregion
    }

    /// <summary>
    /// 制表符绘制的辅助类
    /// </summary>
    public static class BoxDrawingSupply
    {
        // https://www.oreilly.com/openbook/docbook/book/iso-box.html
        public static char boxDownRight = '┌';
        public static char boxDownLeft = '┐';
        public static char boxUpRight = '└';
        public static char boxUpLeft = '┘';

        public static char boxHorizontal = '─';
        public static char boxVertical = '│';

        public static char boxVerticalRight = '├';
        public static char boxVerticalLeft = '┤';
        public static char boxHorizontalDown = '┬';
        public static char boxHorizontalUp = '┴';
        public static char boxVerticalHorizontal = '┼';

        /// <summary>
        /// 获取一个方框第一行的字符串
        /// </summary>
        public static string GetFirstLine(int width)
        {
            StringBuilder sb = new StringBuilder();
            //左上角
            sb.Append(boxDownRight);
            //上部
            for (int i = 1; i < width; i++)
            {
                sb.Append(boxHorizontal);
            }
            //右上角
            sb.Append(boxDownLeft);

            return sb.ToString();
        }

        /// <summary>
        /// 获取一个方框最后一行的字符串
        /// </summary>
        public static string GetLastLine(int width)
        {
            StringBuilder sb = new StringBuilder();
            //左上角
            sb.Append(boxUpRight);
            //上部
            for (int i = 1; i < width; i++)
            {
                sb.Append(boxHorizontal);
            }
            //右上角
            sb.Append(boxUpLeft);

            return sb.ToString();
        }

        /// <summary>
        /// 获取一个方框中间行的字符串
        /// </summary>
        public static string GetMiddleLine(int width,string str)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(boxVertical);
            string newstr = Print.SubStr(str, width);
            sb.Append(newstr);
            sb.Append(boxVertical);
            return sb.ToString();
        }

        /// <summary>
        /// 获取一个方框中间行的字符串
        /// </summary>
        public static string GetMiddleLine(int width)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(boxVertical);
            for (int i = 1;i<width;i++)
            {
                sb.Append(' ');
            }
            sb.Append(boxVertical);
            //Debug.Warning(sb.ToString());
            return sb.ToString();

        }

    }
}
