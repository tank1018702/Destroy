namespace Destroy
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    //[CreatGameObject]
    //public class Player : Script
    //{
    //    enum Direction
    //    {
    //        Up,
    //        Down,
    //        Left,
    //        Right,
    //    }

    //    private Direction direction;
    //    private float speed;

    //    void Init()
    //    {
    //        Console.CursorVisible = false;
    //        Window.SetBufferSize(40, 20);
    //        //摄像机
    //        GameObject camera = new GameObject { Name = "Camera" };
    //        camera.AddComponent<Camera>();
    //        RendererSystem.Init(camera);
    //        //墙
    //        for (int i = 0; i < Console.BufferHeight - 2; i++)
    //        {
    //            GameObject wall = new GameObject { Name = "Wall" };
    //            wall.transform.Position = new Vector2Int(Console.BufferWidth - 1, -i);
    //            Renderer wallRenderer = wall.AddComponent<Renderer>();
    //            wallRenderer.Str = " ";
    //            wallRenderer.BackColor = ConsoleColor.White;
    //            wall.AddComponent<Collider>();
    //        }
    //        //添加组件
    //        Renderer renderer = AddComponent<Renderer>();
    //        renderer.Str = "1";
    //        renderer.ForeColor = ConsoleColor.Red;
    //        AddComponent<Collider>();
    //        //初始化
    //        direction = Direction.Right;
    //        speed = 1;
    //    }

    //    public override void Start()
    //    {
    //        Init();
    //    }

    //    public override void Update()
    //    {
    //        int x = Input.GetDirectInput(KeyCode.A, KeyCode.D);
    //        int y = Input.GetDirectInput(KeyCode.S, KeyCode.W);
    //        transform.Translate(new Vector2Int(x, y));

    //        if (Input.GetKey(KeyCode.J))
    //        {
    //            GameObject bullet = new GameObject { Name = "Bullet" };
    //            bullet.transform.Position = transform.Position + new Vector2Int(1, 0);
    //            bullet.AddComponent<Collider>();
    //            Bullet b = bullet.AddComponent<Bullet>();
    //            b.Init();
    //        }
    //    }
    //}

    //public class Bullet : Script
    //{
    //    public void Init()
    //    {
    //        Renderer renderer = AddComponent<Renderer>();
    //        renderer.Str = "0";
    //    }

    //    public override void Update()
    //    {
    //        transform.Translate(new Vector2Int(1, 0));
    //    }

    //    public override void OnCollision(Collider collision)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    [CreatGameObject]
    public class A : Script
    {
        public override void Start()
        {

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