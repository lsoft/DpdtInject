using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;
using System.Text.RegularExpressions;

namespace DpdtInject.Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(
                typeof(Program).Assembly
                );

            //var dpdt = new Dpdt();
            //dpdt.Setup();
            //dpdt.GenerateTest();
        }
    }

}
