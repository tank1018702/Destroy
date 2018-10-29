namespace Destroy
{
    using System;

    public class Program
    {
        private static void Main()
        {
            Bootstrap bootstrap = new Bootstrap(50, true);
            bootstrap.Start();
            bootstrap.Tick();
        }
    }
}