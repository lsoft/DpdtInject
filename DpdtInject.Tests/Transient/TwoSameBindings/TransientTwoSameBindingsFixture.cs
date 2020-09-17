using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Transient.TwoSameBindings
{

    [TestClass]
    public class TransientTwoSameBindingsFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(TransientTwoSameBindingsModule.TransientTwoSameBindingsModuleTester),
                nameof(TestResources.TransientTwoSameBindingsModule),
                TestResources.TransientTwoSameBindingsModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
