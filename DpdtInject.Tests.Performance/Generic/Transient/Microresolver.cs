using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MicroResolver;

namespace DpdtInject.Tests.Performance.Generic.Transient
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

            _kernel.Resolve<IA>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

        [Benchmark]
        public void GenericTransient()
        {
            _kernel.Resolve<IA>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

    }
}
