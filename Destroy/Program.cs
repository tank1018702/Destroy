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

            Screen screen = new Screen();
            screen.FullScreen();

            Coordinate coordinate = new Coordinate(Coordinate.Mode.RightX__DownY, screen.Width, screen.Height);

            Block block = new Block(items, Point2D.Zero);

            RendererSystem.RenderBlock(block, coordinate);



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