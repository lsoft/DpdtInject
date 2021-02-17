using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.NonGeneric.Hierarchy2DifferentCluster
{
    [TestClass]
    public class ScopeCustomNonGenericHierarchy2DifferentCluster_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeCustomNonGenericHierarchy2DifferentCluster_ClusterTester),
                nameof(TestResources.ScopeCustomNonGenericHierarchy2DifferentCluster_Cluster),
                TestResources.ScopeCustomNonGenericHierarchy2DifferentCluster_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
