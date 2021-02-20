using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Singleton.Fast.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeSingletonFastSingleInterfaceSingleObject_Fixture
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
                nameof(ScopeSingletonFastSingleInterfaceSingleObject_Cluster.ScopeSingletonFastSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeSingletonFastSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeSingletonFastSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
