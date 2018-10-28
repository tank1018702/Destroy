namespace Destroy.Graphics
{
    using System;
    using System.Text;

    public class Window
    {
        public int BufferHeight { get; private set; }

        public int BufferWidth { get; private set; }

        public Window(int bufferWidth = 40, int bufferHeight = 20, bool hideCursor = true)
        {
            Console.CursorVisible = !hideCursor;

            BufferHeight = bufferHeight;
            BufferWidth = bufferWidth;

            Console.WindowHeight = bufferHeight;
            Console.BufferHeight = bufferHeight;
            Console.WindowWidth = bufferWidth;
            Console.BufferWidth = bufferWidth;
        }

        public void FullScreen()
        {
            int height = Console.LargestWindowHeight;
            int width = Console.LargestWindowWidth;

            Console.WindowHeight = height;
            Console.BufferHeight = height;

            Console.WindowWidth = width;
            Console.BufferWidth = width;

            BufferHeight = height;
            BufferWidth = width;
        }

        public void IOEncoding(Encoding encoding)
        {
            Console.InputEncoding = encoding;
            Console.OutputEncoding = encoding;
        }
    }
}