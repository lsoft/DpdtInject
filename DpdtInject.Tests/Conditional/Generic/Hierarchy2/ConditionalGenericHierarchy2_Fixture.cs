using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy2
{
    [TestClass]
    public class ConditionalGenericHierarchy2_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalGenericHierarchy2_Cluster.ConditionalGenericHierarchy2_ClusterTester),
                nameof(TestResources.ConditionalGenericHierarchy2_Cluster),
                TestResources.ConditionalGenericHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
