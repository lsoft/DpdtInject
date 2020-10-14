using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run(
            //    typeof(Program).Assembly
            //    );

            //var summary0 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.Generic.Singleton.Microresolver)
            //    );
            //var summary1 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.NonGeneric.Singleton.Microresolver)
            //    );
            //var summary2 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.Generic.Singleton.DryIoc)
            //    );
            //var summary3 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.NonGeneric.Singleton.DryIoc)
            //    );


            var summary0 = BenchmarkRunner.Run(
                typeof(DpdtInject.Tests.Performance.TimeConsume.BigTree0.Dpdt)
                );
            //var summary1 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.TimeConsume.BigTree0.DryIoc)
            //    );
            //var summary2 = BenchmarkRunner.Run(
            //    typeof(DpdtInject.Tests.Performance.TimeConsume.BigTree0.Microresolver)
            //    );
        }
    }

}
