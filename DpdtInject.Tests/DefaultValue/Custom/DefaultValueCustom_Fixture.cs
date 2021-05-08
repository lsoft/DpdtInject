using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.DefaultValue.Custom
{
    [TestClass]
    public class DefaultValueCustom_Fixture
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
                nameof(DefaultValueCustom_Cluster.DefaultValueCustom_ClusterTester),
                nameof(TestResources.DefaultValueCustom_Cluster),
                TestResources.DefaultValueCustom_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
