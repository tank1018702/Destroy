namespace Destroy
{
    using System;

    public class Renderer : Component
    {
        public string Str;
        public ConsoleColor ForeColor;
        public ConsoleColor BackColor;

        public Renderer()
        {
            Str = "";
            ForeColor = ConsoleColor.Gray;
            BackColor = ConsoleColor.Black;
        }

        public override Component Clone()
        {
            Renderer renderer = new Renderer();
            renderer.Name = Name;
            renderer.Active = Active;
            renderer.Str = Str;
            renderer.ForeColor = ForeColor;
            renderer.BackColor = BackColor;
            return renderer;
        }
    }
}