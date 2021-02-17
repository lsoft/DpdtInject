using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Factory.UnknownConstructorArgument0
{
    [TestClass]
    public class FactoryUnknownConstructorArgument0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FactoryUnknownConstructorArgument0_Cluster.FactoryUnknownConstructorArgument0_ClusterTester),
                nameof(TestResources.FactoryUnknownConstructorArgument0_Cluster),
                TestResources.FactoryUnknownConstructorArgument0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
