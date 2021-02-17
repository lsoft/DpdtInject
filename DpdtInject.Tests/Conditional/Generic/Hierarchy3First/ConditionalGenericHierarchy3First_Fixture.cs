using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.Hierarchy3First
{
    [TestClass]
    public class ConditionalGenericHierarchy3First_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalGenericHierarchy3First_Cluster.ConditionalGenericHierarchy3First_ClusterTester),
                nameof(TestResources.ConditionalGenericHierarchy3First_Cluster),
                TestResources.ConditionalGenericHierarchy3First_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
