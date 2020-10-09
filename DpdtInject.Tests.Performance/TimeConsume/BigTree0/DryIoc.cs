using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DryIoc;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
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

            DryIocRelated.Resolve(_container);
        }

        [Benchmark(Description = DryIocRelated.TestPrefix)]
        public void DoTest()
        {
            DryIocRelated.Resolve(_container);
        }

    }
}
