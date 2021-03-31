using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Proxy.Indexer0
{
    [TestClass]
    public class ProxyIndexer0_Fixture
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
                nameof(ProxyIndexer0_Cluster.ProxyIndexer0_ClusterTester),
                nameof(TestResources.ProxyIndexer0_Cluster),
                TestResources.ProxyIndexer0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
