namespace Destroy.Test
{
    using System;
    using System.Runtime.InteropServices;

    public class Expand
    {
        public static void UseStdFontStyle()
        {
            IntPtr stdOutputHandle = LowLevelAPI.GetStdHandle(-11);

            LowLevelAPI.CONSOLE_FONT_INFO_EX fontInfo = new LowLevelAPI.CONSOLE_FONT_INFO_EX();
            fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
            fontInfo.nFont = 0;
            fontInfo.dwFontSize.X = 20;
            fontInfo.dwFontSize.Y = 20;
            fontInfo.FaceName = "Consolas";

            LowLevelAPI.SetCurrentConsoleFontEx(stdOutputHandle, true, ref fontInfo);
        }
    }
}