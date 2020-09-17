using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DpdtInject.Tests.Performance.Conditional.Transient
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Dpdt
    {
        private DpdtPerformanceModule _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new DpdtPerformanceModule(
                );

            _kernel.Get<IA>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

        [Benchmark]
        public void ConditionalTransient()
        {
            _kernel.Get<IA>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

    }

}
