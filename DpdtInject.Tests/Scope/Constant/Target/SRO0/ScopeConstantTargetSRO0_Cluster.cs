using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Scope.Constant.Target.SRO0
{
    public partial class ScopeConstantTargetSRO0_Cluster : DefaultCluster
    {
        private static readonly string _roString = "readonly string";

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope(_roString)
                ;
        }

        public class ScopeConstantTargetSRO0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetSRO0_Cluster>(
                    null
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
                Assert.AreEqual("readonly string", s);
            }
        }
    }
}
