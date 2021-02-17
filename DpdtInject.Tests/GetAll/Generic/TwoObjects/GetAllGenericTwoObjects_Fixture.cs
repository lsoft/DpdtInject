using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.Generic.TwoObjects
{
    [TestClass]
    public class GetAllGenericTwoObjects_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GetAllGenericTwoObjects_Cluster.GetAllGenericTwoObjects_ClusterTester),
                nameof(TestResources.GetAllGenericTwoObjects_Cluster),
                TestResources.GetAllGenericTwoObjects_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
