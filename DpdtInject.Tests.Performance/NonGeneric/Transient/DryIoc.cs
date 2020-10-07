using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.NonGeneric.Transient
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
        public void NonGenericTransient()
        {
            _kernel!.Resolve(typeof(IA));
            _kernel!.Resolve(typeof(IB));
            _kernel!.Resolve(typeof(IC));
        }

    }
}
