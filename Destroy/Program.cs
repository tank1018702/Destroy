namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    [CreatGameObject]
    public class A : Script
    {
        public override void Start()
        {
            ThisGameObject.AddComponent<Renderer>();
            ThisGameObject.AddComponent<Transform>();
            Console.WriteLine(ThisGameObject.ComponentCount);
        }

        public override void Update()
        {
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