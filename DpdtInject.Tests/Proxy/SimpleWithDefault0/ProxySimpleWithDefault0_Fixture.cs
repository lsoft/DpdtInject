using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Proxy.SimpleWithDefault0
{
    [TestClass]
    public class ProxySimpleWithDefault0_Fixture
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
                nameof(ProxySimpleWithDefault0_Cluster.ProxySimpleWithDefault0_ClusterTester),
                nameof(TestResources.ProxySimpleWithDefault0_Cluster),
                TestResources.ProxySimpleWithDefault0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
