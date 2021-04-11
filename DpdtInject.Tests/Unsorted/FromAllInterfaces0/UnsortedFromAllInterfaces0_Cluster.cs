using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.FromAllInterfaces0
{
    public partial class UnsortedFromAllInterfaces0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            BindAllInterfaces()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedFromAllInterfaces0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedFromAllInterfaces0_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA0>();
                Assert.IsNotNull(a0);

                var a1 = cluster.Get<IA1>();
                Assert.IsNotNull(a1);

                var a2 = cluster.Get<IA2>();
                Assert.IsNotNull(a2);

                Assert.AreSame(a0, a1);
                Assert.AreSame(a1, a2);
            }
        }
    }


    public interface IA0
    {
    }

    public interface IA1
    {
    }

    public interface IA2
    {
    }

    public class A : IA0, IA1, IA2
    {
    }
}
