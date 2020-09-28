using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Custom.TwoSameBindings
{
    [TestClass]
    public class ScopeCustomTwoSameBindingsFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeCustomTwoSameBindingsModule.ScopeCustomTwoSameBindingsModuleTester),
                nameof(TestResources.ScopeCustomTwoSameBindingsModule),
                TestResources.ScopeCustomTwoSameBindingsModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
