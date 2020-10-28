using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0.Transient
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class DryIoc
    {
        protected Container _container;

        [GlobalSetup]
        public void Setup()
        {
            _container = new Container();

            DryIocRelated.Bind(_container);

            DryIocRelated.ResolveGeneric(_container);
            DryIocRelated.ResolveNonGeneric(_container);
        }

        [Benchmark(Description = DryIocRelated.GenericTestName)]
        public void GenericTest()
        {
            DryIocRelated.ResolveGeneric(_container);
        }

        [Benchmark(Description = DryIocRelated.NonGenericTestName)]
        public void NonGenericTest()
        {
            DryIocRelated.ResolveNonGeneric(_container);
        }
    }
}
