using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MicroResolver;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Microresolver
    {
        protected ObjectResolver _container;

        [GlobalSetup]
        public void Setup()
        {
            _container = ObjectResolver.Create();

            MicroResolverRelated.Bind(_container);

            _container.Compile();

            MicroResolverRelated.ResolveGeneric(_container);
            MicroResolverRelated.ResolveNonGeneric(_container);
        }

        [Benchmark(Description = MicroResolverRelated.GenericTestName)]
        public void GenericTest()
        {
            MicroResolverRelated.ResolveGeneric(_container);
        }

        [Benchmark(Description = MicroResolverRelated.NonGenericTestName)]
        public void NonGenericTest()
        {
            MicroResolverRelated.ResolveNonGeneric(_container);
        }

    }
}
