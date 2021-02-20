using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.Generic.SingleObject
{
    [TestClass]
    public class ScopeCustomGenericSingleObject_Fixture
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
                nameof(ScopeCustomGenericSingleObject_Cluster.ScopeCustomGenericSingleObject_ClusterTester),
                nameof(TestResources.ScopeCustomGenericSingleObject_Cluster),
                TestResources.ScopeCustomGenericSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
