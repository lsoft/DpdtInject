using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.NativeTypeArgument0
{

    [TestClass]
    public class UnsortedNativeTypeArgument0Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedNativeTypeArgument0Module.UnsortedNativeTypeArgument0ModuleTester),
                nameof(TestResources.UnsortedNativeTypeArgument0Module),
                TestResources.UnsortedNativeTypeArgument0Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
