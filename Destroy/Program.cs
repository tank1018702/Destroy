namespace Destroy
{
    //[CreatGameObject]
    //public class A : Script
    //{
    //    Camera c;

    //    public override void Start()
    //    {
    //        GameObject camera = new GameObject();
    //        c = camera.AddComponent<Camera>();
    //        c.CharWidth = 2;
    //        RendererSystem.Init(camera);

    //        Renderer renderer = AddComponent<Renderer>();
    //        renderer.Order = 0;
    //        renderer.Str = "吊";
    //        renderer.ForeColor = ConsoleColor.Red;

    //        GameObject go = new GameObject();
    //        go.transform.Position = new Vector2Int(0, 0);
    //        Renderer renderer2 = go.AddComponent<Renderer>();
    //        renderer2.Order = 1;
    //        renderer2.Str = "2";
    //        renderer2.ForeColor = ConsoleColor.Blue;
    //    }

    //    public override void Update()
    //    {
    //        if (Input.GetKey(KeyCode.A))
    //            transform.Translate(Vector2Int.Left);
    //        if (Input.GetKey(KeyCode.D))
    //            transform.Translate(Vector2Int.Right);
    //        if (Input.GetKey(KeyCode.W))
    //            transform.Translate(Vector2Int.Up);
    //        if (Input.GetKey(KeyCode.S))
    //            transform.Translate(Vector2Int.Down);
    //        c.Center(gameObject);
    //    }
    //}

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(new RuntimeDebugger());
            runtimeEngine.Run(20);
        }
    }
}