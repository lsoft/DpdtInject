using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.NonGeneric.SingleObject
{
    [TestClass]
    public class GetAllNonGenericSingleObject_Fixture
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
                nameof(GetAllNonGenericSingleObject_Cluster.GetAllNonGenericSingleObject_ClusterTester),
                nameof(TestResources.GetAllNonGenericSingleObject_Cluster),
                TestResources.GetAllNonGenericSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
