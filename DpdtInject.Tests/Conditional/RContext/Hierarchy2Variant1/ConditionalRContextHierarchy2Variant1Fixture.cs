using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Conditional.RContext.Hierarchy2Variant1
{

    [TestClass]
    public class ConditionalRContextHierarchy2Variant1Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConditionalRContextHierarchy2Variant1Module.ConditionalRContextHierarchy2Variant1ModuleTester),
                nameof(TestResources.ConditionalRContextHierarchy2Variant1Module),
                TestResources.ConditionalRContextHierarchy2Variant1Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
