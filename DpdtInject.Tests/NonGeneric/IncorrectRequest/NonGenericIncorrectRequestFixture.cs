using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.NonGeneric.IncorrectRequest
{

    [TestClass]
    public class NonGenericIncorrectRequestFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(NonGenericIncorrectRequestModule.NonGenericIncorrectRequestModuleTester),
                nameof(TestResources.NonGenericIncorrectRequestModule),
                TestResources.NonGenericIncorrectRequestModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
