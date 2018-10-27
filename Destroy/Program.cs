namespace Destroy
{
    using System;
    using System.Threading;
    using Destroy.Graphics;

    public class Program
    {
        private static void Main()
        {
            char[,] items =
            {
                {'1', '2', '3'},
                {'4', '5', '6'},
                {'7', '8', '9'}
            };

            Screen screen = new Screen(2);
            screen.FullScreen();

            Coordinate coordinate = new Coordinate(Coordinate.Mode.RightX__UpY, screen.Width, screen.Height);

            Block block = new Block(items, 2, new Point2D(0, 2));

            Block block2 = new Block(items, 2, new Point2D());

            RendererSystem.RenderBlock(block, coordinate);
            //RendererSystem.RenderBlock(block2, coordinate);


            //while (true)
            //{
            //    Print.DrawBlock(block, screen);
            //    Thread.Sleep(100);
            //    Console.Clear();
            //}

            Console.ReadKey();
        }
    }
}