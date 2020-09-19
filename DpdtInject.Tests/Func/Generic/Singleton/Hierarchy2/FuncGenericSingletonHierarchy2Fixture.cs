using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Func.Generic.Singleton.Hierarchy2
{

    [TestClass]
    public class FuncGenericSingletonHierarchy2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(FuncGenericSingletonHierarchy2Module.FuncGenericSingletonHierarchy2ModuleTester),
                nameof(TestResources.FuncGenericSingletonHierarchy2Module),
                TestResources.FuncGenericSingletonHierarchy2Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
