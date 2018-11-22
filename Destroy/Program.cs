namespace Destroy
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    [CreatGameObject]
    public class Map : Script
    {
        public override void Start()
        {
            GameObject camera = new GameObject("Camera");
            camera.AddComponent<Camera>();
            RendererSystem.Init(camera);
            Console.CursorVisible = false;

            Renderer renderer = AddComponent<Renderer>();
            renderer.Str = "1";
            renderer.ForeColor = ConsoleColor.Red;
            renderer.BackColor = ConsoleColor.Blue;
        }

        public override void Update()
        {
            int x = Input.GetDirectInput(KeyCode.A, KeyCode.D);
            int y = Input.GetDirectInput(KeyCode.S, KeyCode.W);
            transform.Translate(new Vector2Int(x, y));

            if (Input.GetKey(KeyCode.J))
            {
                GameObject bullet = new GameObject("Bullet");
                bullet.transform.Position = transform.Position;
                bullet.AddComponent<Collider>();
                Bullet b = bullet.AddComponent<Bullet>();
                b.Init();
            }
        }

        public override void OnCollision(Collider collision)
        {
        }
    }

    public class Bullet : Script
    {
        public void Init()
        {
            Renderer renderer = AddComponent<Renderer>();
            renderer.Str = "0";
        }
        public override void Update()
        {
            transform.Translate(new Vector2Int(1, 0));
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(20);
        }
    }
}