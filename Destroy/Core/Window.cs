namespace Destroy.Graphics
{
    using System;
    using System.Text;

    public class Window
    {
        private int charWidth;

        public int Height { get; private set; }

        public int Width { get; private set; }

        public Window(int charWidth, int width = 20, int height = 20, bool hideCursor = true)
        {
            Console.CursorVisible = !hideCursor;

            this.charWidth = charWidth;
            Height = height;
            Width = width;

            Console.WindowHeight = height;
            Console.BufferHeight = height;
            Console.WindowWidth = width * charWidth;
            Console.BufferWidth = width * charWidth;
        }

        public void FullScreen()
        {
            int height = Console.LargestWindowHeight;
            int width = Console.LargestWindowWidth;

            Console.WindowHeight = height;
            Console.BufferHeight = height;

            Console.WindowWidth = width;
            Console.BufferWidth = width;

            Height = height;
            Width = width / charWidth;
        }

        public void IOEncoding(Encoding encoding)
        {
            Console.InputEncoding = encoding;
            Console.OutputEncoding = encoding;
        }

        public void SetTitle(string title) => Console.Title = title;
    }
}