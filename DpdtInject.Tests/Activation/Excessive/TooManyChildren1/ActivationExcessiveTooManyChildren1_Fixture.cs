using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren1
{
    [TestClass]
    public class ActivationExcessiveTooManyChildren1_Fixture
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
                nameof(ActivationExcessiveTooManyChildren1_Cluster.ActivationExcessiveTooManyChildren1_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyChildren1_Cluster),
                TestResources.ActivationExcessiveTooManyChildren1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
