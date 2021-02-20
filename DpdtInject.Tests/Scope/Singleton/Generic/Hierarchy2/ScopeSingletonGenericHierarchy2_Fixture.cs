using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Generic.Hierarchy2
{
    [TestClass]
    public class ScopeSingletonGenericHierarchy2_Fixture
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
                nameof(ScopeSingletonGenericHierarchy2_Cluster.ScopeSingletonGenericHierarchy2_ClusterTester),
                nameof(TestResources.ScopeSingletonGenericHierarchy2_Cluster),
                TestResources.ScopeSingletonGenericHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
