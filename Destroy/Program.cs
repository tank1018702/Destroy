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
            CharBlock charBlock = new CharBlock(10, 10, ' ');
            ColorBlock colorBlock = new ColorBlock(10, 10, ConsoleColor.Red);
            RendererData rendererData = new RendererData(charBlock.Chars, 2, colorBlock.Colors, colorBlock.Colors);
            RendererSystem.Init(10, 10);

            Renderer renderer = AddComponent<Renderer>();
            renderer.Init(rendererData);

            //CharBlock charBlock = new CharBlock(10, 10, ' ');
            //ColorBlock fore = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Red);
            //ColorBlock back = new ColorBlock(charBlock.Width, charBlock.Height, ConsoleColor.Blue);
            //RendererData data = new RendererData(charBlock.Chars, 2, fore.Colors, back.Colors);
            //Renderer renderer = AddComponent<Renderer>();
            //RectCollider rectCollider = AddComponent<RectCollider>();
            //rectCollider.Init(Vector2Int.Zero);
            //renderer.Init(data);
        }

        public override void Update()
        {
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