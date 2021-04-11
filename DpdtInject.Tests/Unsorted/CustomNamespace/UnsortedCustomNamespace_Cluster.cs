using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UnknownCustomNamespace;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.CustomNamespace
{
    public partial class UnsortedCustomNamespace_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                .Configure(
                    new ConstructorArgument(
                        "bFactory",
                        new Func<FakeResource>(() => (FakeResource) null)
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
