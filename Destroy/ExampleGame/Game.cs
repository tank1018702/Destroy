﻿namespace Destroy.ExampleGame
{
    using System;
    using System.Text;
    using Destroy;
    using Destroy.Graphics;

    [UpdateOrder(1)]
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

        float timer;

        public override void Update(float deltaTime)
        {
            if (Input.GetKey(KeyCode.A))
                timer += deltaTime * 5;
            //else
            //    timer = 0;

            if (timer >= 1)
            {
                timer = 0;
                Console.Write(1);
            }
        }
    }

    public class FUC : Script
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