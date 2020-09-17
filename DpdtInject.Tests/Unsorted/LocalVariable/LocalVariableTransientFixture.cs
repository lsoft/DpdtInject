using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.LocalVariable
{

    [TestClass]
    public class LocalVariableTransientFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(LocalVariableTransientModule.LocalVariableTransientTester),
                nameof(TestResources.LocalVariableTransientModule),
                TestResources.LocalVariableTransientModule
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
