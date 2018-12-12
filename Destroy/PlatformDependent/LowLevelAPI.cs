namespace Destroy
{
    using System;
    using System.Runtime.InteropServices;

    internal class LowLevelAPI
    {
        /// <summary>
        /// 获取当前键盘的状态
        /// </summary>
        [DllImport("User32.dll", EntryPoint = "GetAsyncKeyState")]
        public static extern short GetAsyncKeyState(int key);

        /// <summary>
        /// 获取当前选中控制台的句柄
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 从一个程序取得一个标准的输入输出句柄
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle")]
        public static extern IntPtr GetStdHandle(int stdHandle);

        /// <summary>
        /// 设置当前控制台字体
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "SetCurrentConsoleFontEx")]
        public static extern int SetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public Coord dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord
        {
            public short X;
            public short Y;

            public Coord(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };
    }
}