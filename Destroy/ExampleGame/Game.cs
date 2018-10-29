namespace Destroy.ExampleGame
{
    using System;
    using System.Text;
    using Destroy;
    using Destroy.Graphics;

    public class Game : Script
    {
        public override void Start()
        {
            Console.Title = "Game";

            Window window = new Window();
            window.SetIOEncoding(Encoding.Unicode);

            Coordinate coordinate = new Coordinate(CoordinateType.RightX_DownY, window.BufferHeight);

            string[,] items =
            {
                {"┌", "─", "┐"},
                {"│", " ", "│"},
                {"│", " ", "│"},
                {"└", "─", "┘"}
            };

            ColorBlock colorBlock = new ColorBlock(3, 4, ConsoleColor.Red);

            Block block = new Block(items, 2, new Point2D(), colorBlock.Colors);

            RendererSystem.RenderBlock(block, coordinate);

            Console.WriteLine();
        }

        float timer;

        public override void Update(float deltaTime)
        {
            if (Input.GetKey(KeyCode.A))
                timer += deltaTime * 5;
            else
                timer = 0;

            if (timer >= 1)
            {
                timer = 0;
                Console.Write(1);
            }
        }
    }
}