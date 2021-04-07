using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Proxy.SimpleWithOut0
{
    [TestClass]
    public class ProxySimpleWithOut0_Fixture
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
                nameof(ProxySimpleWithOut0_Cluster.ProxySimpleWithOut0_ClusterTester),
                nameof(TestResources.ProxySimpleWithOut0_Cluster),
                TestResources.ProxySimpleWithOut0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
