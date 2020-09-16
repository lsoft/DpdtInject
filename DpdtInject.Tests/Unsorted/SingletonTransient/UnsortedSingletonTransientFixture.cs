using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.SingletonTransient
{

    [TestClass]
    public class UnsortedSingletonTransientFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedSingletonTransientModule.UnsortedSingletonTransientTester),
                nameof(TestResources.UnsortedSingletonTransientModule),
                TestResources.UnsortedSingletonTransientModule
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.WarningCount);
        }

    }
}
