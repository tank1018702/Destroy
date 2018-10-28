namespace Destroy
{
    using System;
    using System.Threading;
    using Destroy.Graphics;

    class Test : Scriptable
    {
        public override void Start()
        {
            Window window = new Window(2);

            Coordinate coordinate = new Coordinate(Coordinate.Mode.RightX_DownY, window.Height);

            char[,] items =
            {
                {'1', '2', '3'},
                {'4', '5', '6'},
                {'7', '8', '9'}
            };

            char[,] maskArray =
            {
                {' ', ' ', ' '},
                {' ', '1', ' '},
                {' ', ' ', ' '}
            };

            Block block = new Block(items, 2, new Point2D());

            CharBlock charBlock = new CharBlock(block.Width, block.Height, ' ');
            Block buffer = new Block(charBlock.Chars, 2);

            Block b = RendererSystem.RenderMask(block, maskArray, ' ');

            RendererSystem.RenderBlockBuffer(b, ref buffer, coordinate);
        }

        public override void Update()
        {

        }
    }

    public class Program
    {
        private static void Main()
        {
            Bootstrap bootstrap = new Bootstrap();
            bootstrap.Start();
            bootstrap.Tick();

            Console.ReadKey();
        }
    }
}