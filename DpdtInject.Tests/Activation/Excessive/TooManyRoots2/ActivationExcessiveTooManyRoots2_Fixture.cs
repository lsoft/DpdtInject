using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots2
{
    [TestClass]
    public class ActivationExcessiveTooManyRoots2_Fixture
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
                nameof(ActivationExcessiveTooManyRoots2_Cluster.ActivationExcessiveTooManyRoots2_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyRoots2_Cluster),
                TestResources.ActivationExcessiveTooManyRoots2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }
    }
}
