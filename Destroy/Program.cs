namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Graphics;

    [CreatGameObject]
    public class A : Script
    {
        public override void Start()
        {
            GO.AddComponent<Transform>();
            GO.AddComponent<Collider>();
            GameObject gameObject = new GameObject();
            gameObject.AddComponent<Transform>();
            gameObject.AddComponent<Collider>();
        }

        public override void Update()
        {
        }

        public override void OnCollision(Collider collision)
        {
            Console.WriteLine(333);
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