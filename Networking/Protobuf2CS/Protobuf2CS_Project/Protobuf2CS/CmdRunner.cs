using System.Diagnostics;

namespace Protobuf2CS
{
    internal static class CmdRunner
    {
        internal static string PATH = @"C:\Windows\System32\cmd.exe";

        /// <summary>
        /// 调用cmd窗口,执行cmd命令
        /// </summary>
        internal static void Execute(string cmd, out string result)
        {
            //去除命令前置与后置空格
            cmd = cmd.Trim() + " && exit";

            using (Process cmdExe = new Process())
            {
                var info = cmdExe.StartInfo;

                info.FileName = PATH; //执行进程路径
                info.UseShellExecute = false; //是否使用操作系统shell启动
                info.RedirectStandardInput = true; //是否接受来自调用程序的输入信息
                info.RedirectStandardOutput = true; //是否由调用程序获取输出信息
                info.RedirectStandardError = true; //是否重定向标准错误输出
                info.CreateNoWindow = true; //是否不显示程序窗口

                cmdExe.Start(); //开始执行
                cmdExe.StandardInput.AutoFlush = true; //是否开启自动刷新(必须在开始执行后调用)
                //向cmd窗口写入命令
                cmdExe.StandardInput.WriteLine(cmd);

                //等待程序执行完(同步)
                cmdExe.WaitForExit();

                //得到结果
                result = cmdExe.StandardOutput.ReadToEnd();

                //退出进程
                cmdExe.Close();
            }
        }
    }
}