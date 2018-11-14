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
            
            Transform transform = AddComponent<Transform>();
            transform.Position = Vector2Int.Zero;
            transform.Coordinate = CoordinateType.Window;

            CharBlock charBlock = new CharBlock(10, 10, ' ');
            ColorBlock fore = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Red);
            ColorBlock back = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Blue);

            Renderer renderer = AddComponent<Renderer>();
            renderer.Chars = charBlock.Chars;
            renderer.ForeColors = fore.Colors;
            renderer.BackColors = back.Colors;
            renderer.CharWidth = 2;
        }

        private float timer = 0;
        public override void Update()
        {
            timer += Time.DeltaTime;
            if (Input.GetKey(KeyCode.D) && timer >= 1)
            {
                timer = 0;
                var t = GetComponent<Transform>();
                t.Position += new Vector2Int(1, 0);
            }
        }

        public override void OnCollision(Collider collision)
        {
        }
    }

    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(60);
        }
    }
}