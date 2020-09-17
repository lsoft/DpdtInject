using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Singleton.UnknownRequest
{

    [TestClass]
    public class SingletonUnknownRequestFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(SingletonUnknownRequestModule.SingletonUnknownRequestModuleTester),
                nameof(TestResources.SingletonUnknownRequestModule),
                TestResources.SingletonUnknownRequestModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
