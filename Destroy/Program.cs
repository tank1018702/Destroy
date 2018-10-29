namespace Destroy
{
    using System;
    using System.Threading;
    using Destroy.Graphics;

    class Test : Scriptable
    {
        public override void Start()
        {
            Window window = new Window();

            Coordinate coordinate = new Coordinate(CoordinateType.RightX_DownY, window.BufferHeight);

            char[,] items =
            {
                {'1', '2', '3'},
                {'4', '5', '6'},
                {'7', '8', '9'}
            };

            char[,] maskArray =
            {
                {'1', '1', '1'},
                {'1', ' ', '1'},
                {'1', '1', '1'}
            };

            ColorBlock colorBlock = new ColorBlock(3, 3, ConsoleColor.Red);

            Block block = new Block(items, 2, new Point2D(), colorBlock.Colors);

            Block mask = RendererSystem.MaskCulling(block, maskArray, ' ');

            Block occlusion = new Block(new char[,] { { '5' } }, 2);

            Block render = RendererSystem.OcclusionCulling(mask, occlusion, new Point2D(1, 1),
                CoordinateType.RightX_DownY);

            Block cut = new Block(' ', 2, new Point2D(0, 0));

            RendererSystem.CutBlock(render, ref cut, new Point2D(0, 0), CoordinateType.RightX_DownY);

            RendererSystem.RenderBlock(cut, coordinate);
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