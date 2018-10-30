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
            Console.CursorVisible = !hideCursor;
            SetBufferSize(bufferWidth, bufferHeight);
        }

        public void SetBufferSize(int bufferWidth, int bufferHeight)
        {
            BufferHeight = bufferHeight;
            BufferWidth = bufferWidth;

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