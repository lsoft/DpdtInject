using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.CannotBeCasted
{

    [TestClass]
    public class UnsortedCannotBeCastedFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedCannotBeCastedModule.UnsortedCannotBeCastedTester),
                nameof(TestResources.UnsortedCannotBeCastedModule),
                TestResources.UnsortedCannotBeCastedModule
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount);
        }

    }
}
