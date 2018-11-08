namespace Destroy
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    public class Program
    {
        private static void Main(string[] args)
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(10);
            //Console.Title = "Game";
            //Window window = new Window(40, 20);
            //window.SetIOEncoding(Encoding.Unicode);
            //string[,] items =
            //{
            //    {"┌", "─", "┐"},
            //    {"│", " ", "│"},
            //    {"│", " ", "│"},
            //    {"└", "─", "┘"}
            //};
            //Block block = new Block(items, 2, CoordinateType.Window);
            //RendererSystem.RenderBlock(block);
        }
    }
}