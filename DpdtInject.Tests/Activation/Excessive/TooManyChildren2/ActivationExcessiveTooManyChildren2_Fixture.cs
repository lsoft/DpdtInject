using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren2
{
    [TestClass]
    public class ActivationExcessiveTooManyChildren2_Fixture
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
                nameof(ActivationExcessiveTooManyChildren2_Cluster.ActivationExcessiveTooManyChildren2_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyChildren2_Cluster),
                TestResources.ActivationExcessiveTooManyChildren2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
