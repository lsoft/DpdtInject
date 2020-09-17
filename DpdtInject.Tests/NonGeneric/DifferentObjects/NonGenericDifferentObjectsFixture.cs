using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.NonGeneric.DifferentObjects
{

    [TestClass]
    public class NonGenericDifferentObjectsFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(NonGenericDifferentObjectsModule.NonGenericDifferentObjectsModuleTester),
                nameof(TestResources.NonGenericDifferentObjectsModule),
                TestResources.NonGenericDifferentObjectsModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
