using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Custom.NonGenericSingleObject
{
    [TestClass]
    public class ScopeCustomNonGenericSingleObjectFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeCustomNonGenericSingleObjectModule.ScopeCustomNonGenericSingleObjectModuleTester),
                nameof(TestResources.ScopeCustomNonGenericSingleObjectModule),
                TestResources.ScopeCustomNonGenericSingleObjectModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
