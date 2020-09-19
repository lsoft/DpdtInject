using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.Generic.Singleton.SingleObject
{

    [TestClass]
    public class FuncGenericSingletonSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncGenericSingletonSingleObjectModule.FuncGenericSingletonSingleObjectModuleTester),
                nameof(TestResources.FuncGenericSingletonSingleObjectModule),
                TestResources.FuncGenericSingletonSingleObjectModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
