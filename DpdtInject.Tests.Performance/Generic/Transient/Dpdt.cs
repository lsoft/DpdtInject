using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator;

namespace DpdtInject.Tests.Performance.Generic.Transient
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

            _kernel.Get<IA>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

        [Benchmark]
        public void GenericTransient()
        {
            _kernel!.Get<IA>();
            _kernel!.Get<IB>();
            _kernel!.Get<IC>();
        }

    }

}
