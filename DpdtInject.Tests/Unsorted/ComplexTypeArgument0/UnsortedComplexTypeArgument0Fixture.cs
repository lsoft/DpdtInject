using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.ComplexTypeArgument0
{

    [TestClass]
    public class UnsortedComplexTypeArgument0Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedComplexTypeArgument0Module.UnsortedComplexTypeArgument0ModuleTester),
                nameof(TestResources.UnsortedComplexTypeArgument0Module),
                TestResources.UnsortedComplexTypeArgument0Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
