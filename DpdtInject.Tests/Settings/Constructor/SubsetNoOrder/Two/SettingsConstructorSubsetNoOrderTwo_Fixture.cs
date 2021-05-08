using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.SubsetNoOrder.Two
{
    [TestClass]
    public class SettingsConstructorSubsetNoOrderTwo_Fixture
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
                nameof(SettingsConstructorSubsetNoOrderTwo_Cluster.SettingsConstructorSubsetNoOrderTwo_ClusterTester),
                nameof(TestResources.SettingsConstructorSubsetNoOrderTwo_Cluster),
                TestResources.SettingsConstructorSubsetNoOrderTwo_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
