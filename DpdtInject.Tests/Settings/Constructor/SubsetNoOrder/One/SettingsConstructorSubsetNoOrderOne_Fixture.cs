using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.SubsetNoOrder.One
{
    [TestClass]
    public class SettingsConstructorSubsetNoOrderOne_Fixture
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
                nameof(SettingsConstructorSubsetNoOrderOne_Cluster.SettingsConstructorSubsetNoOrderOne_ClusterTester),
                nameof(TestResources.SettingsConstructorSubsetNoOrderOne_Cluster),
                TestResources.SettingsConstructorSubsetNoOrderOne_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
