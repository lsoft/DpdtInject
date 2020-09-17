using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.TwoSameBindings
{

    [TestClass]
    public class SingletonTwoSameBindingsFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonTwoSameBindingsModule.SingletonTwoSameBindingsModuleTester),
                nameof(TestResources.SingletonTwoSameBindingsModule),
                TestResources.SingletonTwoSameBindingsModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
