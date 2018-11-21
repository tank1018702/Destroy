namespace Destroy
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Graphics;

    [CreatGameObject]
    public class Map : Script
    {
        void Init()
        {
            Window.SetIOEncoding(Encoding.UTF8);
            Window.SetBufferSize(40, 20);
        }

        public override void Start()
        {
            Console.WriteLine(0);
        }

        public override void Update()
        {
            AddComponent<Bullet>();
        }
    }

    public class Bullet : Script
    {
        public override void Start()
        {
            Console.WriteLine(1);
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(1);
        }
    }
}