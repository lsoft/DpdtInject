using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Linq;

namespace DpdtInject.Tests.Unsorted.Dynamic1
{
    public partial class UnsortedDynamic1_Cluster : DefaultCluster
    {
        public static readonly (nuint, int[,]) Constant = (123, new int[,]{ { 1, 2 }, { 3, 4 } });

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<object, dynamic>()
                .WithConstScope(Constant)
                ;

            Bind<A>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedDynamic1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedDynamic1_Cluster>(
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
