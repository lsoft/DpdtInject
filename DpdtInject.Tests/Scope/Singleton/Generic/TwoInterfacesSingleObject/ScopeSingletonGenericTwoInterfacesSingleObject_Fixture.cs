using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Generic.TwoInterfacesSingleObject
{
    [TestClass]
    public class ScopeSingletonGenericTwoInterfacesSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeSingletonGenericTwoInterfacesSingleObject_Cluster.ScopeSingletonGenericTwoInterfacesSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonGenericTwoInterfacesSingleObject_Cluster),
                TestResources.ScopeSingletonGenericTwoInterfacesSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
