namespace Destroy
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Net;

    [CreatGameObject]
    public class A : Script
    {
        public override void Start()
        {
            Invoke(this, "S", 1);
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(new RuntimeDebugger());
            runtimeEngine.Run(20);
        }
    }
}