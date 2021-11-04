using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Constant.Target.ForeignSRO0
{
    [TestClass]
    public class ScopeConstantTargetForeignSRO0_Fixture
    {
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeConstantTargetForeignSRO0_Cluster.ScopeConstantTargetForeignSRO0_ClusterTester),
                nameof(TestResources.ScopeConstantTargetForeignSRO0_Cluster),
                TestResources.ScopeConstantTargetForeignSRO0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
