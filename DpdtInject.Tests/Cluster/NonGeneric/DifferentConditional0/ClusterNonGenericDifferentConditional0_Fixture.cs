using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentConditional0
{
    [TestClass]
    public class ClusterNonGenericDifferentConditional0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterNonGenericDifferentConditional0_ClusterTester),
                nameof(TestResources.ClusterNonGenericDifferentConditional0_Cluster),
                TestResources.ClusterNonGenericDifferentConditional0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
