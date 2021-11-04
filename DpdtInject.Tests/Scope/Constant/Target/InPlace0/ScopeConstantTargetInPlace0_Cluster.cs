using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.InPlace0
{
    public partial class ScopeConstantTargetInPlace0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope("my const string") //do not extract it! we checks in-place constant in this test!
                ;
        }

        public class ScopeConstantTargetInPlace0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetInPlace0_Cluster>(
                    null
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
                Assert.AreEqual("my const string", s);
            }
        }
    }
}
