using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MicroResolver;

namespace DpdtInject.Tests.Performance.NonGeneric.Singleton
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Dpdt
    {
        private DpdtPerformanceModule _dpdt;

        [GlobalSetup]
        public void Setup()
        {
            _dpdt = new DpdtPerformanceModule(
                );

            _dpdt.Get(typeof(IA));
            _dpdt.Get(typeof(IB));
            _dpdt.Get(typeof(IC));
        }

        [Benchmark]
        public void NonGenericSingleton()
        {
            _dpdt.Get(typeof(IA));
            _dpdt.Get(typeof(IB));
            _dpdt.Get(typeof(IC));
        }

    }

}
