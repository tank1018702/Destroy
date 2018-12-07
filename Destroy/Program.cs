#if Debug
namespace Destroy
{
    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(20);
        }
    }
}
#endif