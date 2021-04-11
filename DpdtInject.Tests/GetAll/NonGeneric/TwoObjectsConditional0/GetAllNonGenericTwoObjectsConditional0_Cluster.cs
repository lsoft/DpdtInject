using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.GetAll.NonGeneric.TwoObjectsConditional0
{
    public partial class GetAllNonGenericTwoObjectsConditional0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
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

        public class GetAllNonGenericTwoObjectsConditional0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllNonGenericTwoObjectsConditional0_Cluster>(
                    null
                    );

                var alist = (List<IA>) cluster.GetAll(typeof(IA));
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
