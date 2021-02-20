using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.NonGeneric.Hierarchy2SameConstant
{
    [TestClass]
    public class ConditionalNonGenericHierarchy2SameConstant_Fixture
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
                nameof(ConditionalNonGenericHierarchy2SameConstant_Cluster.ConditionalNonGenericHierarchy2SameConstant_ClusterTester),
                nameof(TestResources.ConditionalNonGenericHierarchy2SameConstant_Cluster),
                TestResources.ConditionalNonGenericHierarchy2SameConstant_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
