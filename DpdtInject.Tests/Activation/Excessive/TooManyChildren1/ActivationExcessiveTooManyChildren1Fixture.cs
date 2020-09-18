using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren1
{

    [TestClass]
    public class ActivationExcessiveTooManyChildren1Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyChildren1Module.ActivationExcessiveTooManyChildren1Tester),
                nameof(TestResources.ActivationExcessiveTooManyChildren1Module),
                TestResources.ActivationExcessiveTooManyChildren1Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
