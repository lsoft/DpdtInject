using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.NativeNumber0
{
    public partial class UnsortedNativeNumber0_Cluster : DefaultCluster
    {
        public static readonly nuint Constant = 123;

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<nuint>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedNativeNumber0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedNativeNumber0_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
                Assert.AreEqual(Constant, a.Constant);
            }
        }
    }


    public class A 
    {
        public A(
            nuint constant
            )
        {
            Constant = constant;
        }

        public nuint Constant
        {
            get;
        }
    }

}
