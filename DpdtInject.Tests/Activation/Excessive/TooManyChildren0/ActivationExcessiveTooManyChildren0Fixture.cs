using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Activation.Excessive.TooManyChildren0
{

    [TestClass]
    public class ActivationExcessiveTooManyChildren0Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyChildren0Module.ActivationExcessiveTooManyChildren0Tester),
                nameof(TestResources.ActivationExcessiveTooManyChildren0Module),
                TestResources.ActivationExcessiveTooManyChildren0Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
