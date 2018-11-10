namespace Destroy
{
    using System;
    using System.Runtime.InteropServices;

    internal class LowLevelAPI
    {
        /// <summary>
        /// 获取当前键盘的状态
        /// </summary>
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int key);

        /// <summary>
        /// 获取当前选中控制台的句柄
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();
    }
}