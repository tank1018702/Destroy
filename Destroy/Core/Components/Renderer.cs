namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        public string Str;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;
        public uint Order;

        public Renderer()
        {
            Str = "  ";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
            Order = uint.MaxValue;
        }
    }
}