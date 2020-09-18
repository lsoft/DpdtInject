using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var module = new DpdtConsoleModule();

            var c0 = module.Get<IC>();
        }
    }

}

