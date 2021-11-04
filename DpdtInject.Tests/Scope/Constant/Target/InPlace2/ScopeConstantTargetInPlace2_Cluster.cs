using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace2
{
    public partial class ScopeConstantTargetInPlace2_Cluster : DefaultCluster
    {
        public const ulong MyConstant = 123456789UL;

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<ulong>()
                .WithConstScope(MyConstant)
                ;
        }

        public class ScopeConstantTargetInPlace2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetInPlace2_Cluster>(
                    null
                    );

                var ul0 = cluster.Get<ulong>();
                Assert.IsNotNull(ul0);
                Assert.AreEqual(MyConstant, ul0);

                var ul1 = cluster.Get(typeof(ulong));
                Assert.IsNotNull(ul1);
                Assert.AreEqual(MyConstant, ul1);

                var ul2 = cluster.GetFast(default(ulong));
                Assert.IsNotNull(ul2);
                Assert.AreEqual(MyConstant, ul2);
            }
        }
    }
}
