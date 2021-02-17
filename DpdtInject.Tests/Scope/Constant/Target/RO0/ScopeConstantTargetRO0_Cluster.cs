using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Scope.Constant.Target.RO0
{
    public partial class ScopeConstantTargetRO0_Cluster : DefaultCluster
    {
        private readonly string _roString = "readonly string";

        public override void Load()
        {
            Bind<string>()
                .WithConstScope(_roString)
                ;
        }

        public class ScopeConstantTargetRO0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetRO0_Cluster>(
                    null
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
                Assert.AreEqual("readonly string", s);
            }
        }

    }
}
