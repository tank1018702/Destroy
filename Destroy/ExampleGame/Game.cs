namespace Destroy.ExampleGame
{
    using System;
    using System.Text;
    using Destroy;
    using Destroy.Graphics;

    [CreatGameObject(typeof(Test))]
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

            GameObject.RemoveComponent<Game>();
            Console.WriteLine(GameObject.GetComponent<Test>());
        }

        public override void Update(float deltaTime)
        {
        }
    }

    public class Test : Script
    {
        public override void Start()
        {
            Console.WriteLine("1231233");
        }

        public override void Update(float deltaTime)
        {
        }
    }
}