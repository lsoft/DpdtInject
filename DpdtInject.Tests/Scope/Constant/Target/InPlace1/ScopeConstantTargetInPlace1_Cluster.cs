using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace1
{
    public partial class ScopeConstantTargetInPlace1_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<ulong>()
                .WithConstScope(123456789UL) //no not extract it! we checks in-place constant in this test!
                ;
        }

        public class ScopeConstantTargetInPlace1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetInPlace1_Cluster>(
                    null
                    );

                var ul0 = cluster.Get<ulong>();
                Assert.IsNotNull(ul0);
                Assert.AreEqual(123456789UL, ul0);

                var ul1 = cluster.Get(typeof(ulong));
                Assert.IsNotNull(ul1);
                Assert.AreEqual(123456789UL, ul1);

                var ul2 = cluster.GetFast(default(ulong));
                Assert.IsNotNull(ul2);
                Assert.AreEqual(123456789UL, ul2);
            }
        }
    }
}
