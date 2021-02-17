﻿using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.Generic.IsRegistered
{
    public partial class UnsortedGenericIsRegistered_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class UnsortedGenericIsRegistered_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedGenericIsRegistered_Cluster>(
                    null
                    );

                var a = cluster.IsRegisteredFrom<IA>();
                Assert.IsTrue(a);

                var b = cluster.IsRegisteredFrom<IB>();
                Assert.IsTrue(b);

                var c = cluster.IsRegisteredFrom<IC>();
                Assert.IsFalse(c);
            }
        }

    }



    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB
    {
    }

    public class B : IB
    {
    }

    public interface IC
    {
    }
}
