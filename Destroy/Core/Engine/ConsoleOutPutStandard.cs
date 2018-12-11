using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Destroy
{
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
