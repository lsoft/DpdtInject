using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Factory.UnrelatedMethod
{
    [TestClass]
    public class FactoryUnrelatedMethod_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FactoryUnrelatedMethod_Cluster.FactoryUnrelatedMethod_ClusterTester),
                nameof(TestResources.FactoryUnrelatedMethod_Cluster),
                TestResources.FactoryUnrelatedMethod_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
