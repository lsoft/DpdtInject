using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.MDArray2
{
    public partial class UnsortedMDArray2_Cluster : DefaultCluster
    {
        public static readonly int[][][] Constant = new int[2][][] { new int[2][] { new int[] { 1, 2 }, new int[] { 2, 3 } }, new int[2][] { new int[] { 1, 2 }, new int[] { 2, 3 } } };

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<int[][][]>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedMDArray2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedMDArray2_Cluster>(
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
            int[][][] constant
            )
        {
            Constant = constant;
        }

        public int[][][] Constant
        {
            get;
        }
    }

}
