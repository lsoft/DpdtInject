using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.GetAll.Generic.Hierarchy2Conditional
{
    public partial class GetAllGenericHierarchy2Conditional_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IZ>()
                .To<Z>()
                .WithSingletonScope()
                .When(rt => rt.IsGetAllResolution)
                ;

            Bind<IA>()
                .To<A0>()
                .WithSingletonScope()
                ;

            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;
        }

        public class GetAllGenericHierarchy2Conditional_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllGenericHierarchy2Conditional_Cluster>(
                    null
                    );

                var alist = cluster.GetAll<IA>();
                Assert.IsNotNull(alist);
                Assert.AreEqual(2, alist.Count);
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A0)));
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A1)));
            }
        }
    }

    public interface IZ
    {
    }

    public class Z : IZ
    {
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

        public IZ Z
        {
            get;
        }

        public A0(
            IZ z
            )
        {
            Z = z;
        }
    }

    public class A1 : IA
    {
        public Type ImplementationType => typeof(A1);
    }
}
