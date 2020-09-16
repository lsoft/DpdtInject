using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MicroResolver;

namespace DpdtInject.Tests.Performance.NonGeneric.Transient
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Microresolver
    {
        protected ObjectResolver _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = ObjectResolver.Create();

            _kernel.Register<IA, A>(Lifestyle.Transient);
            _kernel.Register<IB, B>(Lifestyle.Transient);
            _kernel.Register<IC, C>(Lifestyle.Transient);

            _kernel.Compile();

            _kernel.Resolve(typeof(IA));
            _kernel.Resolve(typeof(IB));
            _kernel.Resolve(typeof(IC));
        }

        [Benchmark]
        public void Baseline()
        {
            _kernel.Resolve(typeof(IA));
            _kernel.Resolve(typeof(IB));
            _kernel.Resolve(typeof(IC));
        }

    }
}
