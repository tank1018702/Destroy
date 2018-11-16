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
            RendererSystem.Init(40, 40, 2);

            var r = AddComponent<Renderer>();
            RendererData data = new RendererData(5, 5, 2, ' ', ConsoleColor.Gray, ConsoleColor.Red);
            r.Init(data);
            transform.Translate(new Vector2Int(5, 0));

            //CharBlock charBlock = new CharBlock(10, 10, ' ');
            //ColorBlock fore = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Red);
            //ColorBlock back = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Blue);
            //RendererData data = new RendererData(charBlock.Chars, 2, fore.Colors, back.Colors);
            //Renderer renderer = AddComponent<Renderer>();
            //RectCollider rectCollider = AddComponent<RectCollider>();
            //rectCollider.Init(Vector2Int.Zero);
            //renderer.Init(data);
        }
        //int i = 0;
        public override void Update()
        {
            //if (i == 1)
            //{
            //    transform.Translate(new Vector2Int(0, 1));
            //}
            //i++;

            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector2Int(0, 1));
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
            runtimeEngine.Run(10);
        }
    }
}