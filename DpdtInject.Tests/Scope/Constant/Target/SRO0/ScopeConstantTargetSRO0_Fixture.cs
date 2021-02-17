using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.SRO0
{
    [TestClass] 
    public class ScopeConstantTargetSRO0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeConstantTargetSRO0_Cluster.ScopeConstantTargetSRO0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetSRO0_Cluster),
                TestResources.ScopeConstantTargetSRO0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
