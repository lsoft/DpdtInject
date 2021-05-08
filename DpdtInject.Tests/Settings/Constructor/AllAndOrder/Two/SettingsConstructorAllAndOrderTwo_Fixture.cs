using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.AllAndOrder.Two
{
    [TestClass]
    public class SettingsConstructorAllAndOrderTwo_Fixture
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
                nameof(SettingsConstructorAllAndOrderTwo_Cluster.SettingsConstructorAllAndOrderTwo_ClusterTester),
                nameof(TestResources.SettingsConstructorAllAndOrderTwo_Cluster),
                TestResources.SettingsConstructorAllAndOrderTwo_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
