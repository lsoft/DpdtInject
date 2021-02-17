using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.NonGeneric.Hierarchy3Second
{
    [TestClass]
    public class ConditionalNonGenericHierarchy3Second_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalNonGenericHierarchy3Second_Cluster.ConditionalNonGenericHierarchy3Second_ClusterTester),
                nameof(TestResources.ConditionalNonGenericHierarchy3Second_Cluster),
                TestResources.ConditionalNonGenericHierarchy3Second_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
