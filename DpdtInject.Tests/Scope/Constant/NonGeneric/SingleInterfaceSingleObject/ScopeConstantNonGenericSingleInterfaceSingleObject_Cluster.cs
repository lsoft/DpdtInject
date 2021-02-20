using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.NonGeneric.SingleInterfaceSingleObject
{
    public partial class ScopeConstantNonGenericSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        private static readonly A AInstance = new();

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .WithConstScope(AInstance)
                ;
        }

        public class ScopeConstantNonGenericSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantNonGenericSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = (IA) cluster.Get(typeof(IA));
                Assert.IsNotNull(a0);
                Assert.AreSame(AInstance, a0);

                var a1 = (IA) cluster.Get(typeof(IA));
                Assert.IsNotNull(a1);
                Assert.AreSame(AInstance, a1);
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
