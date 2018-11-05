namespace Destroy
{
    using System;
    using System.Diagnostics;

    public static class Utils
    {
        public static void OpenNewProcess(string path)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = path; //设置启动
                process.Start();
            }
        }

        public static string ProgramPath
        {
            get
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    string path = $@"{Environment.CurrentDirectory}\{process.ProcessName}.exe";
                    return path;
                }
            }
        }

        public static string ProgramDirectory => Environment.CurrentDirectory + "\\";
    }
}