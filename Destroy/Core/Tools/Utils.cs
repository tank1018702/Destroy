namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class Utils
    {
        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        private static extern IntPtr GetForegroundWindow();

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
                IntPtr activeWindowThreadHandle = GetForegroundWindow();
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