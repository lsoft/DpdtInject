using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.RO0
{
    [TestClass] 
    public class ScopeConstantTargetRO0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeConstantTargetRO0_Cluster.ScopeConstantTargetRO0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetRO0_Cluster),
                TestResources.ScopeConstantTargetRO0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
