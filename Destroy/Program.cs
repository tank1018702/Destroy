using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Destroy
{
    public class Program
    {
        private static void Main(string[] args)
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(50, false);
            runtimeEngine.Run();
        }
    }
}