namespace Destroy.ExampleGame
{
    using System;
    using System.Text;
    using Destroy;
    using Destroy.Graphics;

    [CreatGameObject]
    public class Game : Script
    {
        public override void Start()
        {
            Console.Title = "Game";

            Window window = new Window(40, 20);
            window.SetIOEncoding(Encoding.Unicode);

            string[,] items =
            {
                {"┌", "─", "┐"},
                {"│", " ", "│"},
                {"│", " ", "│"},
                {"└", "─", "┘"}
            };

            Block block = new Block(items, 2, CoordinateType.RightX_DownY);

            RendererSystem.RenderBlock(block);
        }

        public override void Update(float deltaTime)
        {
        }
    }

    [CreatGameObject]
    public class Test : Script
    {
        public override void Start()
        {
        }

        public override void Update(float deltaTime)
        {
        }
    }
}