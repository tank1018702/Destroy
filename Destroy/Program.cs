namespace Destroy
{
#if Debug

    using System;
    using System.Net;
    using System.Collections.Generic;
    using ProtoBuf;

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(20);
        }
    }
#endif
}