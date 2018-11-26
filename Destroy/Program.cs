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
            GameObject camera = new GameObject();
            camera.AddComponent<Camera>();
            RendererSystem.Init(camera);

            Renderer renderer = AddComponent<Renderer>();
            renderer.Order = 0;
            renderer.Str = "1";
            renderer.ForeColor = ConsoleColor.Red;

            GameObject go = new GameObject();
            go.transform.Position = new Vector2Int(0, 0);
            Renderer renderer2 = go.AddComponent<Renderer>();
            renderer2.Order = 1;
            renderer2.Str = "2";
            renderer2.ForeColor = ConsoleColor.Blue;
        }

        public override void Update()
        {
            if (Input.GetKey(KeyCode.A))
                transform.Translate(new Vector2Int(-1, 0));
            if (Input.GetKey(KeyCode.D))
                transform.Translate(new Vector2Int(1, 0));
            if (Input.GetKey(KeyCode.S))
                transform.Translate(new Vector2Int(0, -1));
            if (Input.GetKey(KeyCode.W))
                transform.Translate(new Vector2Int(0, 1));
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