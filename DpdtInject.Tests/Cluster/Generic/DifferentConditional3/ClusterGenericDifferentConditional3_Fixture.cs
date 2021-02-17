using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.Generic.DifferentConditional3
{
    [TestClass]
    public class ClusterGenericDifferentConditional3_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterGenericDifferentConditional3_ClusterTester),
                nameof(TestResources.ClusterGenericDifferentConditional3_Cluster),
                TestResources.ClusterGenericDifferentConditional3_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
