namespace Destroy
{
    using System;

    public static class Debug
    {
        /// <summary>
        /// 通常用于测试调试
        /// </summary>
        public static void Log(object msg)
        { 
            System.Diagnostics.Debug.WriteLine(msg);
        }
        /// <summary>
        /// 通常用于引擎警告
        /// </summary>
        public static void Warning(object msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        /// <summary>
        /// 通常用于引擎严重错误
        /// </summary>
        public static void Error(object msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }
    }
}