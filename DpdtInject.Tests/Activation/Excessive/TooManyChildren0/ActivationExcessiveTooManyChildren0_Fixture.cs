using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren0
{
    [TestClass]
    public class ActivationExcessiveTooManyChildren0_Fixture
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
                nameof(ActivationExcessiveTooManyChildren0_Cluster.ActivationExcessiveTooManyChildren0_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyChildren0_Cluster),
                TestResources.ActivationExcessiveTooManyChildren0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
