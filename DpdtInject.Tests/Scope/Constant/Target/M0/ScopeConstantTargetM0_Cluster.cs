using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.M0
{
    public partial class ScopeConstantTargetM0_Cluster : DefaultCluster
    {
        private string GetSomeString() => Guid.NewGuid().ToString();

        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope(this.GetSomeString())
                ;
        }

        public class ScopeConstantTargetM0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetM0_Cluster>(
                    null
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
            }
        }
    }
}
