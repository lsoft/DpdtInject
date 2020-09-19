using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.NonGeneric.Transient.Hierarchy2
{

    [TestClass]
    public class FuncNonGenericTransientHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncNonGenericTransientHierarchy2Module.FuncNonGenericTransientHierarchy2ModuleTester),
                nameof(TestResources.FuncNonGenericTransientHierarchy2Module),
                TestResources.FuncNonGenericTransientHierarchy2Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
