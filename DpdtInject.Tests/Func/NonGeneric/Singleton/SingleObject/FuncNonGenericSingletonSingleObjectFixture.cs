using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.NonGeneric.Singleton.SingleObject
{

    [TestClass]
    public class FuncNonGenericSingletonSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncNonGenericSingletonSingleObjectModule.FuncNonGenericSingletonSingleObjectModuleTester),
                nameof(TestResources.FuncNonGenericSingletonSingleObjectModule),
                TestResources.FuncNonGenericSingletonSingleObjectModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
