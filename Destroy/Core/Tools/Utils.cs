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

        public static bool IsBackground
        {
            get
            {
                IntPtr curWindowThreadHandle = Process.GetCurrentProcess().MainWindowHandle;
                IntPtr activeWindowThreadHandle = LowLevelAPI.GetForegroundWindow();
                return !curWindowThreadHandle.Equals(activeWindowThreadHandle); //返回不相等的结果
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