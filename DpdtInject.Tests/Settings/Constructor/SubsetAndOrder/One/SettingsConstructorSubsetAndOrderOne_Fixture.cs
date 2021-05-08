using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.SubsetAndOrder.One
{
    [TestClass]
    public class SettingsConstructorSubsetAndOrderOne_Fixture
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
                nameof(SettingsConstructorSubsetAndOrderOne_Cluster.SettingsConstructorSubsetAndOrderOne_ClusterTester),
                nameof(TestResources.SettingsConstructorSubsetAndOrderOne_Cluster),
                TestResources.SettingsConstructorSubsetAndOrderOne_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
