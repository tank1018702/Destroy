namespace Destroy
{
#if Debug
    public class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine(new RuntimeDebugger());
            runtimeEngine.Run(20);
        }
    }
#endif
}