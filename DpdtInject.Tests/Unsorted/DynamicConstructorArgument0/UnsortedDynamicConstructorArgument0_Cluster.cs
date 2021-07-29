using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Unsorted.DynamicConstructorArgument0
{
    public partial class UnsortedDynamicConstructorArgument0_Cluster : DefaultCluster
    {
        public static readonly dynamic Constant = (123, new int[,]{ { 1, 2 }, { 3, 4 } });

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("constant", Constant))
                ;
        }

        public class UnsortedDynamicConstructorArgument0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedDynamicConstructorArgument0_Cluster>(
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
            dynamic constant
            )
        {
            Constant = constant;
        }

        public dynamic Constant
        {
            get;
        }
    }

}
