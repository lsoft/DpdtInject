using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.NonGeneric.Hierarchy3First
{
    [TestClass]
    public class ConditionalNonGenericHierarchy3First_Fixture
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
                nameof(ConditionalNonGenericHierarchy3First_Cluster.ConditionalNonGenericHierarchy3First_ClusterTester),
                nameof(TestResources.ConditionalNonGenericHierarchy3First_Cluster),
                TestResources.ConditionalNonGenericHierarchy3First_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
