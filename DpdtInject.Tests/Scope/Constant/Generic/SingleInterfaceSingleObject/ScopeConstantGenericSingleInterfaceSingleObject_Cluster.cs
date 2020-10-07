using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Scope.Constant.Generic.SingleInterfaceSingleObject
{
    public partial class ScopeConstantGenericSingleInterfaceSingleObject_Cluster : DefaultCluster
    {
        private static readonly A AInstance = new A();

        public override void Load()
        {
            Bind<IA>()
                .WithConstScope(AInstance)
                ;

        }

        public class ScopeConstantGenericSingleInterfaceSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantGenericSingleInterfaceSingleObject_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);
                Assert.AreSame(AInstance, a0);
                
                var a1 = cluster.Get<IA>();
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
