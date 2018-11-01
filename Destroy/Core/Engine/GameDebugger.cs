using System;
using System.Diagnostics;
using System.Threading;

namespace Destroy
{
    public class GameDebugger
    {
        public static void UseSafeMode(string[] args)
        {
            if (args.Length == 0)
            {
                Debug.Warning("进入调试模式");

                Process debugger = Process.GetCurrentProcess();
                Process game = new Process();

                string debuggerPath = $@"{Environment.CurrentDirectory}\{debugger.ProcessName}.exe";
                //启动设置
                game.StartInfo.FileName = debuggerPath;             //指定路径
                game.StartInfo.Arguments = "debug";                 //传递参数
                //把game进程的输出重定向到此进程, 并且隐藏game进程窗口
                game.StartInfo.UseShellExecute = false;
                game.StartInfo.RedirectStandardOutput = true;
                game.Start();                                       //开始进程

                game.BeginOutputReadLine();

                game.OutputDataReceived += (obj, e) =>
                {
                    string str2 = e.Data;
                    Console.WriteLine(str2);
                };

                Console.ReadKey();

                if (!game.HasExited)
                    game.Kill();
                game.Close();
                debugger.Close();
            }
            else if (args.Length == 1 && args[0] == "debug")
            {

                while (true)
                {
                    Console.WriteLine("123");
                    Thread.Sleep(1000);
                }
                Console.ReadKey();
            }
        }
    }
}