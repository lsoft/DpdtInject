using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.NonGeneric.Singleton
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Dryloc
    {
        protected Container _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = new Container();

            _kernel.Register<IA, A>(Reuse.Singleton);
            _kernel.Register<IB, B>(Reuse.Singleton);
            _kernel.Register<IC, C>(Reuse.Singleton);

            _kernel.Resolve(typeof(IA));
            _kernel.Resolve(typeof(IB));
            _kernel.Resolve(typeof(IC));
        }

        [Benchmark]
        public void NonGenericSingleton()
        {
            _kernel.Resolve(typeof(IA));
            _kernel.Resolve(typeof(IB));
            _kernel.Resolve(typeof(IC));
        }

    }
}
