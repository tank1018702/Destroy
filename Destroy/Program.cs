namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Destroy.Graphics;

    [CreatGameObject(0, "GameObject", typeof(Collider), typeof(Renderer))]
    public class A : Script
    {
        public override void Start()
        {
            GameObject go = new GameObject("Camera");
            Camera camera = go.AddComponent<Camera>();
            RendererSystem.Init(go);

            Collider collider = GetComponent<Collider>();
            Renderer renderer = GetComponent<Renderer>();
            renderer.Str = "网";
        }

        public override void Update()
        {
            transform.Position += new Vector2Int(0, 1);
            //Console.WriteLine(transform.Position.ToString());
        }

        public override void OnCollisionEnter(Collider collision)
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