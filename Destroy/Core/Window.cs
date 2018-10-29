namespace Destroy.Graphics
{
    using System;
    using System.Text;

    public class Window
    {
        public int BufferHeight { get; private set; }

        public int BufferWidth { get; private set; }

        public Window(int bufferWidth, int bufferHeight, bool hideCursor = true)
        {
            BufferHeight = bufferHeight;
            BufferWidth = bufferWidth;

            Console.CursorVisible = !hideCursor;
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