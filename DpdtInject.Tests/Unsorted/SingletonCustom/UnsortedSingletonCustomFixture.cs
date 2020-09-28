using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.SingletonCustom
{

    [TestClass]
    public class UnsortedSingletonCustomFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedSingletonCustomModule.UnsortedSingletonCustomTester),
                nameof(TestResources.UnsortedSingletonCustomModule),
                TestResources.UnsortedSingletonCustomModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
