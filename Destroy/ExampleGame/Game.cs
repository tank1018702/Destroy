namespace Destroy.ExampleGame
{
    using System;
    using System.Text;
    using Destroy;
    using Destroy.Graphics;

    [CreatGameObject(1, "Player", typeof(BlockRenderer))]
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

            Debug.Log(gameObject.GetComponent<BlockRenderer>());
        }

        public override void Update(float deltaTime)
        {

        }
    }
}