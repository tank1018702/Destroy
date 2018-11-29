using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Destroy;

namespace DevTest
{
    class Program
    {
        private static void Main()
        {
            RuntimeEngine runtimeEngine = new RuntimeEngine();
            runtimeEngine.Run(50);
        }
    }
}