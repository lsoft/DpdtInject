using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.NonGeneric.NoBinding
{
    [TestClass]
    public class GetAllNonGenericNoBinding_Fixture
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
                nameof(GetAllNonGenericNoBinding_Cluster.GetAllNonGenericNoBinding_ClusterTester),
                nameof(TestResources.GetAllNonGenericNoBinding_Cluster),
                TestResources.GetAllNonGenericNoBinding_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
