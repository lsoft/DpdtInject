using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.CircularDependency3
{

    [TestClass]
    public class UnsortedCircularDependency3Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedCircularDependency3Module.UnsortedCircularDependency3ModuleTester),
                nameof(TestResources.UnsortedCircularDependency3Module),
                TestResources.UnsortedCircularDependency3Module
                );

            preparation.Check();

            //no check for warning because here is a lot of warnings due to circular dependency
            Assert.AreEqual(1, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
            Assert.AreEqual(DpdtExceptionTypeEnum.CircularDependency, preparation.DiagnosticReporter.GetDpdtException().Type);
        }

    }
}
