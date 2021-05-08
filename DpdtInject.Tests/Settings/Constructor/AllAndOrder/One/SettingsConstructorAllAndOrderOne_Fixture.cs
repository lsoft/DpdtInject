using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Settings.Constructor.AllAndOrder.One
{
    [TestClass]
    public class SettingsConstructorAllAndOrderOne_Fixture
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
                nameof(SettingsConstructorAllAndOrderOne_Cluster.SettingsConstructorAllAndOrderOne_ClusterTester),
                nameof(TestResources.SettingsConstructorAllAndOrderOne_Cluster),
                TestResources.SettingsConstructorAllAndOrderOne_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
