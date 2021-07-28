using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.Dynamic2
{
    public partial class UnsortedDynamic2_Cluster : DefaultCluster
    {
        public static readonly dynamic Constant = (123, new int[,]{ { 1, 2 }, { 3, 4 } });

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<object>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedDynamic2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedDynamic2_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
            }
        }
    }


    public class A 
    {
        public A(
            object constant
            )
        {
            Constant = constant;
        }

        public object Constant
        {
            get;
        }
    }

}
