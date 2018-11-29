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

        ///// <summary>
        ///// 矩形碰撞检测
        ///// </summary>
        //private static bool RectIntersects(Vector2Int selfPos, Vector2Int otherPos, RectCollider self, RectCollider other)
        //{
        //    if (selfPos.X >= otherPos.X + other.Size.X || otherPos.X >= selfPos.X + self.Size.X)
        //        return false;
        //    else if (selfPos.Y >= otherPos.Y + other.Size.Y || otherPos.Y >= selfPos.Y + self.Size.Y)
        //        return false;
        //    else
        //        return true;
        //}
    }
}