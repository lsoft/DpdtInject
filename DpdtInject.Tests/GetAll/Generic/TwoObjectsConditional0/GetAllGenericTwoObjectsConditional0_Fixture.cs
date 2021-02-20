using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.Generic.TwoObjectsConditional0
{
    [TestClass]
    public class GetAllGenericTwoObjectsConditional0_Fixture
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
                nameof(GetAllGenericTwoObjectsConditional0_Cluster.GetAllGenericTwoObjectsConditional0_ClusterTester),
                nameof(TestResources.GetAllGenericTwoObjectsConditional0_Cluster),
                TestResources.GetAllGenericTwoObjectsConditional0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
