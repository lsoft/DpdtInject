using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.NonGeneric.Singleton.Hierarchy2
{

    [TestClass]
    public class FuncNonGenericSingletonHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncNonGenericSingletonHierarchy2Module.FuncNonGenericSingletonHierarchy2ModuleTester),
                nameof(TestResources.FuncNonGenericSingletonHierarchy2Module),
                TestResources.FuncNonGenericSingletonHierarchy2Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
