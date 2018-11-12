namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    [CreatGameObject]
    public class A : Script
    {
        public override void Start()
        {
            Console.WriteLine(GO.GameObjectCount);
        }

        public override void Update()
        {
            Console.WriteLine(Time.TickTime);
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(10);
        }
    }
}