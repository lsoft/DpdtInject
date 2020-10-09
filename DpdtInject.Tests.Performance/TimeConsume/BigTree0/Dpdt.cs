using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator;
using System;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
{

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class Dpdt
    {
        private TimeConsumeBigTree0_Cluster _cluster;

        [GlobalSetup]
        public void Setup()
        {
            _cluster = new TimeConsumeBigTree0_Cluster(
                null
                );

            TimeConsumeBigTree0_Cluster.ResolveDpdt(_cluster);
        }

        [Benchmark(Description = TimeConsumeBigTree0_Cluster.TestPrefix)]
        public void DoTest()
        {
            TimeConsumeBigTree0_Cluster.ResolveDpdt(_cluster);
        }

    }

}
