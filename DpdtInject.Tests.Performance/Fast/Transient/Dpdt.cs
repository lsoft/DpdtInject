using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator;

namespace DpdtInject.Tests.Performance.Fast.Transient
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

            _kernel.GetFast(default(IA));
            _kernel.GetFast(default(IB));
            _kernel.GetFast(default(IC));
        }

        [Benchmark]
        public void FastTransient()
        {
            _kernel!.GetFast(default(IA));
            _kernel!.GetFast(default(IB));
            _kernel!.GetFast(default(IC));
        }

    }

}
