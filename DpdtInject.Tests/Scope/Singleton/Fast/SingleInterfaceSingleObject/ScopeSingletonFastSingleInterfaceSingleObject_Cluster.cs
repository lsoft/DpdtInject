using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Fast.SingleInterfaceSingleObject
{
    public partial class ScopeSingletonFastSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class ScopeSingletonFastSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonFastSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = cluster.GetFast(default(IA));
                Assert.IsNotNull(a0);

                var a1 = cluster.GetFast(default(IA));
                Assert.IsNotNull(a1);

                Assert.AreSame(a0, a1);
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA
    {
    }
}
