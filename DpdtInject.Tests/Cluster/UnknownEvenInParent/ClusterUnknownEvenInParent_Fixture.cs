using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.UnknownEvenInParent
{
    [TestClass]
    public class ClusterUnknownEvenInParent_Fixture
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
                nameof(ClusterUnknownEvenInParent_ClusterTester),
                nameof(TestResources.ClusterUnknownEvenInParent_Cluster),
                TestResources.ClusterUnknownEvenInParent_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
