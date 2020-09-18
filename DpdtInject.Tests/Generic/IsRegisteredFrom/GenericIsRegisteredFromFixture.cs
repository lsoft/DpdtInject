using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Generic.IsRegisteredFrom
{

    [TestClass]
    public class GenericIsRegisteredFromFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GenericIsRegisteredFromModule.GenericIsRegisteredFromModuleTester),
                nameof(TestResources.GenericIsRegisteredFromModule),
                TestResources.GenericIsRegisteredFromModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
