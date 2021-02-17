using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Factory.UnknownConstructorArgument1
{
    [TestClass]
    public class FactoryUnknownConstructorArgument1_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FactoryUnknownConstructorArgument1_Cluster.FactoryUnknownConstructorArgument1_ClusterTester),
                nameof(TestResources.FactoryUnknownConstructorArgument1_Cluster),
                TestResources.FactoryUnknownConstructorArgument1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
