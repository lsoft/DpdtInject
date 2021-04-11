using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.GetAll.NonGeneric.TwoObjects
{
    public partial class GetAllNonGenericTwoObjects_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A0>()
                .WithSingletonScope()
                ;

            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;
        }

        public class GetAllNonGenericTwoObjects_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllNonGenericTwoObjects_Cluster>(
                    null
                    );

                var alist = (List<IA>) cluster.GetAll(typeof(IA));
                Assert.IsNotNull(alist);
                Assert.AreEqual(2, alist.Count);
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A0)));
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A1)));
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
