namespace Destroy
{
    using System.Text;
    using System.Collections.Generic;

    public static class UIFactroy
    {
        public static TextBox CreateTextBox(Vector2Int pos, int height,int width)
        {

            GameObject gameObject = new GameObject("TextBox");
            gameObject.transform.Position = pos;

            //添加一个TextBox控件,用于寻找对应的Lable
            TextBox textBox = gameObject.AddComponent<TextBox>();
            //添加Label控件
            for (int i = height; i >0; i--)
            {
                textBox.Labels.Add(CreateLabel(pos + new Vector2Int(1, i),"",width));
            }
            #region 创建边框
            int boxWidth = width + 2, boxHeight = height + 2;
            //添加一个方框
            GameObject boxDrawing = new GameObject("BoxDrawing");
            boxDrawing.transform.Position = pos;
            Mesh mesh = boxDrawing.AddComponent<Mesh>();

            List<Vector2Int> meshList = new List<Vector2Int>();
            //添加上下边框的Mesh
            for (int i = 0; i < boxWidth; i++)
            {
                meshList.Add(new Vector2Int(i, 0));
                meshList.Add(new Vector2Int(i, boxHeight -1));
            }
            //添加左右边框的Mesh
            for(int i = 0;i<boxHeight;i++)
            {
                meshList.Add(new Vector2Int(0, i));
                meshList.Add(new Vector2Int(boxWidth-1, i));
            }
            mesh.Init(meshList);

            Renderer renderer = boxDrawing.AddComponent<Renderer>();

            //添加边框的贴图
            StringBuilder sb = new StringBuilder();
            sb.Append(BoxDrawingSupply.GetFirstLine(boxWidth));
            for(int i = 0;i<boxHeight-2;i++)
            {
                sb.Append(' ');
                sb.Append(BoxDrawingSupply.boxVertical);
                sb.Append(BoxDrawingSupply.boxVertical);
                sb.Append(' ');
            }
            sb.Append(BoxDrawingSupply.GetLastLine(boxWidth));

            renderer.Init(sb.ToString(), -1);
            #endregion
            //("wtf:" + textBox.labels[1].GetComponent<Renderer>().Pos_RenderPoint[new Vector2Int(2,0)].Depth);

            return textBox;
        }

        /// <summary>
        /// 创建一个Lable组件,不带有默认文字
        /// </summary>
        public static Label CreateLabel(Vector2Int pos ,int width)
        {
            GameObject lable = new GameObject("Label");
            //初始化位置
            lable.transform.Position = pos;
            //添加一个Label组件
            Label labelCom = lable.AddComponent<Label>();
            //添加一个宽度等同于width的Mesh
            Mesh mesh = lable.AddComponent<Mesh>();
            List<Vector2Int> meshList = new List<Vector2Int>();
            for (int i = 0; i < width; i++)
            {
                meshList.Add(new Vector2Int(i, 0));
            }
            mesh.Init(meshList);
            //添加一个Renderer组件
            Renderer renderer = lable.AddComponent<Renderer>();
            renderer.Depth = -1;
            //不进行初始化,手动进行添加
            return labelCom;
        }

        /// <summary>
        /// 创建一个Lable
        /// </summary>
        public static Label CreateLabel(Vector2Int pos,string text, int width)
        {
            Label lableCom = CreateLabel(pos, width);
            Renderer renderer = lableCom.GetComponent<Renderer>();
            renderer.Texture =  new Texture(text);
            return lableCom;
        }
    }

    /// <summary>
    /// 单行Lebel控件
    /// Label对象上的label脚本.默认不通过这个创建
    /// </summary>
    public class Label : Component
    {
        //默认初始化
        internal override void Initialize()
        {
            depth = -1;
            foreColor = RendererSystem.DefaultColorFore;
            backColor = RendererSystem.DefaultColorBack;
        }

        //自定义初始化
        public void Init(int depth,EngineColor foreColor, EngineColor backColor)
        {
            this.depth = depth;
            this.foreColor = foreColor;
            this.backColor = backColor;
        }

        public EngineColor foreColor, backColor;

        public int depth = -1;

        //当改动Text变量的时候重新渲染renderer
        public string Text
        {
            get { return GetComponent<Renderer>().Texture.pic; }
            set
            {
                GetComponent<Renderer>().Init(value, depth, foreColor, backColor);
            }
        }
    }

    /// <summary>
    /// 文本框组件
    /// </summary>
    public class TextBox : Component
    {
        //最后返回的应该是TextBox组件,可以通过label更改每一条的信息,
        public List<Label> Labels = new List<Label>();
        
        /// <summary>
        /// 更改对应行上面的字符,也可以通过获取Labels自己找对应的Label组件
        /// </summary>
        public bool SetText(string str,int line)
        {
            if(line > Labels.Count )
            {
                return false;
            }
            else if(line <= 0)
            {
                return false;
            }
            else
            {
                Labels[line - 1].Text = str;
                return true;
            }
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
        //┘└──┘└
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
            if(RendererSystem.charWidth == 1)
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
            else
            {
                StringBuilder sb = new StringBuilder();
                //左上角
                sb.Append(' ');
                sb.Append(boxDownRight);
                //上部
                for (int i = 0; i < width-2; i++)
                {
                    sb.Append(boxHorizontal);
                    sb.Append(boxHorizontal);
                }
                //右上角
                sb.Append(boxDownLeft);
                sb.Append(' ');

                return sb.ToString();
            }
        }

        /// <summary>
        /// 获取一个方框最后一行的字符串
        /// </summary>
        public static string GetLastLine(int width)
        {
            if (RendererSystem.charWidth == 1)
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
            else
            {
                StringBuilder sb = new StringBuilder();
                //左下角
                sb.Append(' ');
                sb.Append(boxUpRight);
                //上部
                for (int i = 0; i < width-2; i++)
                {
                    sb.Append(boxHorizontal);
                    sb.Append(boxHorizontal);
                }
                //右下角
                sb.Append(boxUpLeft);
                sb.Append(' ');
                return sb.ToString();
            }
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