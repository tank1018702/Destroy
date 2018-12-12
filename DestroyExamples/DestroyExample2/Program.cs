namespace Destroy.Example2
{
    using Destroy;

    internal class Program
    {
        public static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            
            runtimeEngine.Run(100);
        }
    }
}