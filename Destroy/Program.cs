namespace Destroy
{
    public class Program
    {
        private static void Main()
        {
            RuntimeEngine bootstrap = new RuntimeEngine(50, true);
            bootstrap.Run();
        }
    }
}