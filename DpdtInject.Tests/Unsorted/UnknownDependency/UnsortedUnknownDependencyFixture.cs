using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.UnknownDependency
{

    [TestClass]
    public class UnsortedUnknownDependencyFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedUnknownDependencyModule.UnsortedUnknownDependencyModuleTester),
                nameof(TestResources.UnsortedUnknownDependencyModule),
                TestResources.UnsortedUnknownDependencyModule
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(DpdtExceptionTypeEnum.NoBindingAvailable, preparation.DiagnosticReporter.GetDpdtException().Type);
        }

    }
}
