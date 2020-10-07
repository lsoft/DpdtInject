using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren3
{

    [TestClass]
    public class ActivationExcessiveTooManyChildren3_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyChildren3_Cluster.ActivationExcessiveTooManyChildren3_ClusterTester),
                nameof(TestResources.ActivationExcessiveTooManyChildren3_Cluster),
                TestResources.ActivationExcessiveTooManyChildren3_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
