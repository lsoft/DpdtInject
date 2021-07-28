using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.Tuple1
{
    public partial class UnsortedTuple1_Cluster : DefaultCluster
    {
        public static readonly (nuint, int[,]) Constant = (123, new int[,]{ { 1, 2 }, { 3, 4 } });

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<(nuint, int[,])>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedTuple1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedTuple1_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
                Assert.AreEqual(Constant.Item1, a.Constant.Item1);
                Assert.AreEqual(Constant.Item2[0, 0], a.Constant.Item2[0, 0]);
                Assert.AreEqual(Constant.Item2[0, 1], a.Constant.Item2[0, 1]);
                Assert.AreEqual(Constant.Item2[1, 0], a.Constant.Item2[1, 0]);
                Assert.AreEqual(Constant.Item2[1, 1], a.Constant.Item2[1, 1]);
            }
        }
    }


    public class A 
    {
        public A(
            (nuint, int[,]) constant
            )
        {
            Constant = constant;
        }

        public (nuint, int[,]) Constant
        {
            get;
        }
    }

}
