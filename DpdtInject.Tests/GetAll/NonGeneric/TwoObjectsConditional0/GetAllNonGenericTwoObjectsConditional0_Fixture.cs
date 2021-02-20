using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.NonGeneric.TwoObjectsConditional0
{
    [TestClass]
    public class GetAllNonGenericTwoObjectsConditional0_Fixture
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
                nameof(GetAllNonGenericTwoObjectsConditional0_Cluster.GetAllNonGenericTwoObjectsConditional0_ClusterTester),
                nameof(TestResources.GetAllNonGenericTwoObjectsConditional0_Cluster),
                TestResources.GetAllNonGenericTwoObjectsConditional0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
