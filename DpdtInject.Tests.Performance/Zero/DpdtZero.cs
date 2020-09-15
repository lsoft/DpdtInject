using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace DpdtInject.Tests.Performance.Zero
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class DpdtZero
    {
        private DpdtModule0 _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new DpdtModule0(
                );

            _kernel.Get<IA>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

        [Benchmark]
        public void Actual()
        {
            _kernel.Get<IA>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

    }

}
