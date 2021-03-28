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

                var s = cluster.Get<ulong>();
                Assert.IsNotNull(s);
                Assert.AreEqual(123456789UL, s);
            }
        }
    }
}
