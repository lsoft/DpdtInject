using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots1
{

    [TestClass]
    public class ActivationExcessiveTooManyRoots1_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyRoots1_Cluster.ActivationExcessiveTooManyRoots1_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyRoots1_Cluster),
                TestResources.ActivationExcessiveTooManyRoots1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
