﻿namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        public string Str;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;
        /// <summary>
        /// Order为0时脚本显示优先级最高(最后被渲染), 然后向着数轴正方向递减。
        /// </summary>
        public uint Order;
        public int Width;

        public Renderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Order = uint.MaxValue;
            Width = 0;
        }

        public override Component Clone()
        {
            Renderer renderer = new Renderer();
            renderer.Name = Name;
            renderer.Active = Active;
            renderer.Str = Str;
            renderer.ForeColor = ForeColor;
            renderer.BackColor = BackColor;
            renderer.Order = Order;
            renderer.Width = Width;
            return renderer;
        }
    }
}