using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Constant.Hierarchy2WithSame
{

    [TestClass]
    public class ConstantHierarchy2WithSameFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ConstantHierarchy2WithSameModule.ConstantHierarchy2WithSameModuleTester),
                nameof(TestResources.ConstantHierarchy2WithSameModule),
                TestResources.ConstantHierarchy2WithSameModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
