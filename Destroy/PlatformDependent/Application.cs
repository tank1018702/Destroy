namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static class Application
    {
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

        public static void OpenNewProcess(string path)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = path; //设置启动
                process.Start();
            }
        }

        public static void Close(bool safe = true)
        {
            if (safe)
                Process.GetCurrentProcess().Kill();
            else
                Environment.Exit(0);
        }

        public static void UseStandardSetting()
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