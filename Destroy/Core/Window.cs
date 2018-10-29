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

        public void SetFullScreen()
        {
            BufferHeight = Console.LargestWindowHeight;
            BufferWidth = Console.LargestWindowWidth;

            Console.WindowHeight = BufferHeight;
            Console.BufferHeight = BufferHeight;
            Console.WindowWidth = BufferWidth;
            Console.BufferWidth = BufferWidth;
        }

        public void SetIOEncoding(Encoding encoding)
        {
            Console.InputEncoding = encoding;
            Console.OutputEncoding = encoding;
        }
    }
}