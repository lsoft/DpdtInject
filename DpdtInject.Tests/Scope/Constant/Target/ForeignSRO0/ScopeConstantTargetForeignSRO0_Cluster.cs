using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.ForeignSRO0
{
    public partial class ScopeConstantTargetForeignSRO0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope(ConstantContainer.ROString)
                ;
        }

        public class ScopeConstantTargetForeignSRO0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetForeignSRO0_Cluster>(
                    null
                    );

                var s = cluster.Get<string>();
                Assert.IsNotNull(s);
                Assert.AreEqual("readonly string", s);
            }
        }
    }

    public class ConstantContainer
    {
        public static readonly string ROString = "readonly string";
    }
}
