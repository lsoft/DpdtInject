using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator;

namespace DpdtInject.Tests.Performance.NonGeneric.Singleton
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Dpdt
    {
        private DpdtCluster _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new DpdtCluster(
                null
                );

            _kernel.Get(typeof(IA));
            _kernel.Get(typeof(IB));
            _kernel.Get(typeof(IC));
        }

        [Benchmark]
        public void NonGenericSingleton()
        {
            _kernel.Get(typeof(IA));
            _kernel.Get(typeof(IB));
            _kernel.Get(typeof(IC));
        }

    }

}
