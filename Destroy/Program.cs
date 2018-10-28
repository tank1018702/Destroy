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
            //screen.FullScreen();

            Coordinate coordinate = new Coordinate(Coordinate.Mode.RightX_DownY, screen.Height);

            Block block = new Block(items, 1, new Point2D());

            Block block2 = new Block(items, 1, new Point2D(block.Width, 10 + block.Height));

            //RendererSystem.RenderBlock(block2, coordinate);

            int i = 0;
            RendererSystem system = new RendererSystem();
            while (true)
            {
                i++;
                system.RenderBlockBuffer(block, coordinate);
                Thread.Sleep(1000);
                block = new Block(items, 1, new Point2D(0, i));
            }

            Console.ReadKey();
        }
    }
}