using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
    /// <summary>
    /// 标准控制台输出,用于处理DrawCall
    /// TODO 改成多线程的
    /// </summary>
    public static class ConsoleOutPutStandard
    {
        public static int num;
        public static void Draw(DrawCall call)
        {
            Console.SetCursorPosition(call.X, call.Y);
            Console.ForegroundColor = call.ForeColor.ToConsoleColor();
            Console.BackgroundColor = call.BackColor.ToConsoleColor();
            Console.Write(call.Str);

            //ConsoleOutPutStandard.num = num + 1;
            //Debug.Log(num.ToString()+call.ToString());
        }
    }
}
