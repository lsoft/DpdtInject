using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots3
{

    [TestClass]
    public class ActivationExcessiveTooManyRoots3_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyRoots3_Cluster.ActivationExcessiveTooManyRoots3_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyRoots3_Cluster),
                TestResources.ActivationExcessiveTooManyRoots3_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
