using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MicroResolver;
using System;
using System.Collections.Generic;
using System.Text;
using DpdtInject.Binding.Example.Zero;

namespace DpdtInject.Tests.Performance.Zero
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class MicroresolverZero
    {
        protected ObjectResolver _kernel;

        [GlobalSetup]
        public void Setup()
        {
            _kernel = ObjectResolver.Create();

            _kernel.Register<IA1, A>(Lifestyle.Singleton)
                ;
            _kernel.Register<IB, B>(Lifestyle.Singleton);

            _kernel.Register<IC, C>(Lifestyle.Singleton);

            _kernel.Compile();

            _kernel.Resolve<IA1>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

        [Benchmark]
        public void Baseline()
        {
            _kernel.Resolve<IA1>();
            _kernel.Resolve<IB>();
            _kernel.Resolve<IC>();
        }

    }
}
