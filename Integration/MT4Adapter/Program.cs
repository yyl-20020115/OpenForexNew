using System;
using System.Collections.Generic;
using System.Text;

namespace MT4Adapter
{
    /// <summary>
    /// Contains some testing code, that can be used by switching the project from dll to exe.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Test 2 integration managers conflicting on the same Uri.
        /// </summary>
        static void ConflictIntegrationsTest()
        {
            //IntegrationMT4Server _manager1 = new IntegrationMT4Server(new Uri("net.tcp://localhost:13129/TradingAPI"));
            //IntegrationMT4Server _manager2 = new IntegrationMT4Server(new Uri("net.tcp://localhost:13129/TradingAPI"));

            Console.ReadKey();
        }

        /// <summary>
        /// Some testing code for this module is stored here. If the module is switched to Console application, those tests can be directly and easily run.
        /// </summary>
        static void Main()
        {
            MT4RemoteAdapter adapter = new MT4RemoteAdapter(new Uri("net.tcp://localhost:13123/TradingAPI"));
            
            Console.ReadKey();
        }
    }
}
