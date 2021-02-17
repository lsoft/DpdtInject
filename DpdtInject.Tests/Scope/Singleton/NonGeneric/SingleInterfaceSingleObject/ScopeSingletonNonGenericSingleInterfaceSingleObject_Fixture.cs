using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.NonGeneric.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeSingletonNonGenericSingleInterfaceSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeSingletonNonGenericSingleInterfaceSingleObject_Cluster.ScopeSingletonNonGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonNonGenericSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeSingletonNonGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
