using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.Generic.Transient
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

            _kernel.Register<IA, A>(Reuse.Transient);
            _kernel.Register<IB, B>(Reuse.Transient);
            _kernel.Register<IC, C>(Reuse.Transient);

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
