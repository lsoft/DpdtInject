using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.NonGeneric.Transient.SingleObject
{

    [TestClass]
    public class FuncNonGenericTransientSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncNonGenericTransientSingleObjectModule.FuncNonGenericTransientSingleObjectModuleTester),
                nameof(TestResources.FuncNonGenericTransientSingleObjectModule),
                TestResources.FuncNonGenericTransientSingleObjectModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
