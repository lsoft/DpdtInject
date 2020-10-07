using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Singleton.Generic.SingleObjectDefinedArgument
{
    [TestClass]
    public class ScopeSingletonGenericSingleObjectDefinedArgument_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeSingletonGenericSingleObjectDefinedArgument_Cluster.ScopeSingletonGenericSingleObjectDefinedArgument_ClusterTester),
                nameof(TestResources.ScopeSingletonGenericSingleObjectDefinedArgument_Cluster),
                TestResources.ScopeSingletonGenericSingleObjectDefinedArgument_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
