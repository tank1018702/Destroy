namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        public string Str;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;
        /// <summary>
        /// 为0时脚本显示优先级最高(最后被渲染), 然后向着数轴正方向递减。
        /// </summary>
        public uint Depth;

        public Renderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Depth = uint.MaxValue;
        }

        public override Component Clone()
        {
            Renderer renderer = new Renderer();
            renderer.Name = Name;
            renderer.Active = Active;
            renderer.Str = Str;
            renderer.ForeColor = ForeColor;
            renderer.BackColor = BackColor;
            renderer.Depth = Depth;
            return renderer;
        }
    }
}