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

            MicroResolverRelated.Resolve(_container);
        }

        [Benchmark(Description = MicroResolverRelated.TestPrefix)]
        public void DoTest()
        {
            MicroResolverRelated.Resolve(_container);
        }

    }
}
