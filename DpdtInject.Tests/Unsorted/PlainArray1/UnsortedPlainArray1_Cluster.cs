using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.PlainArray1
{
    public partial class UnsortedPlainArray1_Cluster : DefaultCluster
    {
        public static readonly int[,] Constant = new int[,] { { 1, 2, 3, 4, 5 }, { 5, 6, 7, 8, 9 } };

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<int[,]>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedPlainArray1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedPlainArray1_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
                Assert.IsTrue(ReferenceEquals(Constant, a.Constant));
            }
        }
    }


    public class A 
    {
        public A(
            int[,] constant
            )
        {
            Constant = constant;
        }

        public int[,] Constant
        {
            get;
        }
    }

}
