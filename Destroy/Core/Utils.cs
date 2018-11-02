namespace Destroy
{
    using System;
    using System.Diagnostics;

    public class Utils
    {
        public static void OpenNewProcess(string path)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = path; //设置启动
                process.Start();
            }
        }

        public static string GetCurrentPath()
        {
            using (Process process = Process.GetCurrentProcess())
            {
                string path = $@"{Environment.CurrentDirectory}\{process.ProcessName}.exe";
                return path;
            }
        }


    }
}