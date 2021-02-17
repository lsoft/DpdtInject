using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Bind;
using UnknownCustomNamespace;

namespace DpdtInject.Tests.Unsorted.CustomNamespace
{
    public partial class UnsortedCustomNamespace_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                .Configure(
                    new ConstructorArgument(
                        "bFactory",
                        new Func<FakeResource>(() => (FakeResource)null)
                        )
                    )
                ;
        }

        public class UnsortedCustomNamespace_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedCustomNamespace_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
            }
        }

    }

    public class A
    {
        private readonly Func<FakeResource> _bFactory;

        public A(
            Func<FakeResource> bFactory
            )
        {
            _bFactory = bFactory;
        }
    }

}
