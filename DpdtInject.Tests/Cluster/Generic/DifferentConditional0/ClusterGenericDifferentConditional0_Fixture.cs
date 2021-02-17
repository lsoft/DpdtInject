using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional0
{
    [TestClass]
    public class ClusterGenericDifferentConditional0_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterGenericDifferentConditional0_ClusterTester),
                nameof(TestResources.ClusterGenericDifferentConditional0_Cluster),
                TestResources.ClusterGenericDifferentConditional0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
