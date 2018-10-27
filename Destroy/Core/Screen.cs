namespace Destroy.Graphics
{
    using System;
    using System.Text;

    public class Screen
    {
        public int Height { get; private set; }

        public int Width { get; private set; }

        public Screen(int width = 10, int height = 10, bool hideCursor = true)
        {
            Console.CursorVisible = !hideCursor;

            Height = height;
            Width = width;

            Console.WindowHeight = Height + 1;
            Console.BufferHeight = Height + 1;

            Console.WindowWidth = (Width + 1) * 2;
            Console.BufferWidth = (Width + 1) * 2;
        }

        public void FullScreen()
        {
            int height = Console.LargestWindowHeight;
            int width = Console.LargestWindowWidth;

            Console.WindowHeight = height;
            Console.BufferHeight = height;

            Console.WindowWidth = width;
            Console.BufferWidth = width;

            Height = height - 1;
            Width = width / 2 - 2;
        }

        public void IOEncoding(Encoding encoding)
        {
            Console.InputEncoding = encoding;
            Console.OutputEncoding = encoding;
        }

        public void SetTitle(string title) => Console.Title = title;
    }
}