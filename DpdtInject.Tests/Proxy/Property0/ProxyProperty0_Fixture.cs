using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Proxy.Property0
{
    [TestClass]
    public class ProxyProperty0_Fixture
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
                nameof(ProxyProperty0_Cluster.ProxyProperty0_ClusterTester),
                nameof(TestResources.ProxyProperty0_Cluster),
                TestResources.ProxyProperty0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
