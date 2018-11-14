namespace Destroy
{
    using System;
    using System.Text;
    using System.Diagnostics;

    public static class Window
    {
        public static int BufferHeight { get; private set; }

        public static int BufferWidth { get; private set; }

        public static void SetBufferSize(int bufferWidth, int bufferHeight)
        {
            BufferHeight = bufferHeight;
            BufferWidth = bufferWidth;

            Console.WindowHeight = BufferHeight;
            Console.BufferHeight = BufferHeight;
            Console.WindowWidth = BufferWidth;
            Console.BufferWidth = BufferWidth;
        }

        public static void SetIOEncoding(Encoding encoding)
        {
            Console.InputEncoding = encoding;
            Console.OutputEncoding = encoding;
        }

        public static void Close(bool safe = true)
        {
            if (safe)
                Process.GetCurrentProcess().Kill();
            else
                Environment.Exit(0);
        }
    }
}