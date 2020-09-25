using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.Generic.Singleton
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class DryIoc
    {
        protected Container _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new Container();

            _kernel.Register<IA, A>(Reuse.Singleton);
            _kernel.Register<IB, B>(Reuse.Singleton);
            _kernel.Register<IC, C>(Reuse.Singleton);

            _kernel.Resolve<IA>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

        [Benchmark]
        public void GenericSingleton()
        {
            _kernel.Resolve<IA>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

    }
}
