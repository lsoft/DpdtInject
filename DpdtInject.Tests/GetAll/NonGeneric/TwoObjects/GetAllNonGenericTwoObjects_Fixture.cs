using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.NonGeneric.TwoObjects
{
    [TestClass]
    public class GetAllNonGenericTwoObjects_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GetAllNonGenericTwoObjects_Cluster.GetAllNonGenericTwoObjects_ClusterTester),
                nameof(TestResources.GetAllNonGenericTwoObjects_Cluster),
                TestResources.GetAllNonGenericTwoObjects_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
