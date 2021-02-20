using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots0
{
    [TestClass]
    public class ActivationExcessiveTooManyRoots0_Fixture
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
                nameof(ActivationExcessiveTooManyRoots0_Cluster.ActivationExcessiveTooManyRoots0_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyRoots0_Cluster),
                TestResources.ActivationExcessiveTooManyRoots0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
