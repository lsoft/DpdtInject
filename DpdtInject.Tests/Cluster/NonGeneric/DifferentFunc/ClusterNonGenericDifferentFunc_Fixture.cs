using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentFunc
{
    [TestClass]
    public class ClusterNonGenericDifferentFunc_Fixture
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
                nameof(ClusterNonGenericDifferentFunc_ClusterTester),
                nameof(TestResources.ClusterNonGenericDifferentFunc_Cluster),
                TestResources.ClusterNonGenericDifferentFunc_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
