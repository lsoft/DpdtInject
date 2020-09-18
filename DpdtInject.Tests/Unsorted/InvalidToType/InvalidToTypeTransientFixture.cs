using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.InvalidToType
{

    [TestClass]
    public class InvalidToTypeTransientFixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(InvalidToTypeTransientModule.InvalidToTypeTransientTester),
                nameof(TestResources.InvalidToTypeTransientModule),
                TestResources.InvalidToTypeTransientModule
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.IncorrectBinding_IncorrectTarget, preparation.DiagnosticReporter.GetDpdtException().Type);
        }

    }
}
