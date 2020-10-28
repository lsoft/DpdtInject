using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DpdtInject.Generator;
using System;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0.Singleton
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

            TimeConsumeBigTree0_Cluster.ResolveGeneric(_cluster);
            TimeConsumeBigTree0_Cluster.ResolveNonGeneric(_cluster);
            TimeConsumeBigTree0_Cluster.ResolveFast(_cluster);
        }

        [Benchmark(Description = TimeConsumeBigTree0_Cluster.GenericTestName)]
        public void GenericTest()
        {
            TimeConsumeBigTree0_Cluster.ResolveGeneric(_cluster);
        }

        [Benchmark(Description = TimeConsumeBigTree0_Cluster.NonGenericTestName)]
        public void NonGenericTest()
        {
            TimeConsumeBigTree0_Cluster.ResolveNonGeneric(_cluster);
        }

        [Benchmark(Description = TimeConsumeBigTree0_Cluster.FastTestName)]
        public void FastTest()
        {
            TimeConsumeBigTree0_Cluster.ResolveFast(_cluster);
        }
    }

}
