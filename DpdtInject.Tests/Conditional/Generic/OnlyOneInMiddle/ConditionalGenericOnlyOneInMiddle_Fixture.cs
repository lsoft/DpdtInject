using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Conditional.Generic.OnlyOneInMiddle
{
    [TestClass]
    public class ConditionalGenericOnlyOneInMiddle_Fixture
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
                nameof(ConditionalGenericOnlyOneInMiddle_Cluster.ConditionalGenericOnlyOneInMiddle_ClusterTester),
                nameof(TestResources.ConditionalGenericOnlyOneInMiddle_Cluster),
                TestResources.ConditionalGenericOnlyOneInMiddle_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
