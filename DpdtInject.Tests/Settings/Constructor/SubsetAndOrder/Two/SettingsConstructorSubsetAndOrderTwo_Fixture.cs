using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.SubsetAndOrder.Two
{
    [TestClass]
    public class SettingsConstructorSubsetAndOrderTwo_Fixture
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
                nameof(SettingsConstructorSubsetAndOrderTwo_Cluster.SettingsConstructorSubsetAndOrderTwo_ClusterTester),
                nameof(TestResources.SettingsConstructorSubsetAndOrderTwo_Cluster),
                TestResources.SettingsConstructorSubsetAndOrderTwo_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
