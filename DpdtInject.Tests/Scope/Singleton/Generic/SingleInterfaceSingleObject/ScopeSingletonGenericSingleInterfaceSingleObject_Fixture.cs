using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Generic.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeSingletonGenericSingleInterfaceSingleObject_Fixture
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
                nameof(ScopeSingletonGenericSingleInterfaceSingleObject_Cluster.ScopeSingletonGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonGenericSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeSingletonGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
