using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.NonGeneric.TwoSameBindings
{

    [TestClass]
    public class NonGenericTwoSameBindingsFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(NonGenericTwoSameBindingsModule.NonGenericTwoSameBindingsModuleTester),
                nameof(TestResources.NonGenericTwoSameBindingsModule),
                TestResources.NonGenericTwoSameBindingsModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
