using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.Hierarchy3First
{

    [TestClass]
    public class ConditionalHierarchy3FirstFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalHierarchy3FirstModule.ConditionalHierarchy3FirstModuleTester),
                nameof(TestResources.ConditionalHierarchy3FirstModule),
                TestResources.ConditionalHierarchy3FirstModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
