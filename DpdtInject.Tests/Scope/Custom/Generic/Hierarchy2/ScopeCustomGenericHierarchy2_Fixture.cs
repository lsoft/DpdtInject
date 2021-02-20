using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.Generic.Hierarchy2
{
    [TestClass]
    public class ScopeCustomGenericHierarchy2_Fixture
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
                nameof(ScopeCustomGenericHierarchy2_Cluster.ScopeCustomGenericHierarchy2_ClusterTester),
                nameof(TestResources.ScopeCustomGenericHierarchy2_Cluster),
                TestResources.ScopeCustomGenericHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
