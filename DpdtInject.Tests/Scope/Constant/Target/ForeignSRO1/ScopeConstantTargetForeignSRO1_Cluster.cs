using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Scope.Constant.Target.ForeignSRO1
{
    public partial class ScopeConstantTargetForeignSRO1_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<string>()
                .WithConstScope(ConstantContainer.InnerConstantContainer.ROString)
                ;
        }

        public class ScopeConstantTargetForeignSRO1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeConstantTargetForeignSRO1_Cluster>(
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
        public class InnerConstantContainer
        {
            public static readonly string ROString = "readonly string";
        }
    }
}
