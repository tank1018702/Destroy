namespace Destroy.Example
{
    using Destroy;

    internal class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(20);
        }
    }
}