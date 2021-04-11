using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Singleton.NonGeneric.SingleInterfaceSingleObject
{
    public partial class ScopeSingletonNonGenericSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class ScopeSingletonNonGenericSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeSingletonNonGenericSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = (IA) cluster.Get(typeof(IA));
                Assert.IsNotNull(a0);

                var a1 = (IA) cluster.Get(typeof(IA));
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
