using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.GetAll.NonGeneric.Hierarchy2Conditional
{
    [TestClass]
    public class GetAllNonGenericHierarchy2Conditional_Fixture
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
                nameof(GetAllNonGenericHierarchy2Conditional_Cluster.GetAllNonGenericHierarchy2Conditional_ClusterTester),
                nameof(TestResources.GetAllNonGenericHierarchy2Conditional_Cluster),
                TestResources.GetAllNonGenericHierarchy2Conditional_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
