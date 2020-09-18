using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Activation.Excessive.TooManyRoots3
{

    [TestClass]
    public class ActivationExcessiveTooManyRoots3Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ActivationExcessiveTooManyRoots3Module.ActivationExcessiveTooManyRoots3Tester),
                nameof(TestResources.ActivationExcessiveTooManyRoots3Module),
                TestResources.ActivationExcessiveTooManyRoots3Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
