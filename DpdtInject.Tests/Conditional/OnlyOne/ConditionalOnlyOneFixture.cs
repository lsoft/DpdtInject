using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.OnlyOne
{

    [TestClass]
    public class ConditionalOnlyOneFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalOnlyOneModule.ConditionalOnlyOneModuleTester),
                nameof(TestResources.ConditionalOnlyOneModule),
                TestResources.ConditionalOnlyOneModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
