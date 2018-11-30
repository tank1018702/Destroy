namespace Destroy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RuntimeDebugger
    {
        public RuntimeDebugger()
        {
            
        }

        public void Watch()
        {
        }

        public void HandleException()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }
    }
}