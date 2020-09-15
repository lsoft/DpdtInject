using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Binding.Example.Zero;

namespace DpdtInject.Tests.Performance.Zero
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class DpdtZero
    {
        private D2pdt2Module0 _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new D2pdt2Module0(
                );

            _kernel.Get<IA1>();
            _kernel.Get<IA2>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

        [Benchmark]
        public void Actual()
        {
            _kernel.Get<IA1>();
            _kernel.Get<IB>();
            _kernel.Get<IC>();
        }

    }


}
