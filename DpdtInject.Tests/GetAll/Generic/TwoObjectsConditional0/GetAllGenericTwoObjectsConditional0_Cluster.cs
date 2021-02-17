using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.GetAll.Generic.TwoObjectsConditional0
{
    public partial class GetAllGenericTwoObjectsConditional0_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A0>()
                .WithSingletonScope()
                .When(rt => false)
                ;

            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;
        }

        public class GetAllGenericTwoObjectsConditional0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllGenericTwoObjectsConditional0_Cluster>(
                    null
                    );

                var alist = cluster.GetAll<IA>();
                Assert.IsNotNull(alist);
                Assert.AreEqual(1, alist.Count);
                Assert.IsTrue(alist[0].ImplementationType == typeof(A1));
            }
        }

    }


    public interface IA
    {
        Type ImplementationType
        {
            get;
        }
    }

    public class A0 : IA
    {
        public Type ImplementationType => typeof(A0);
    }

    public class A1 : IA
    {
        public Type ImplementationType => typeof(A1);
    }
}
