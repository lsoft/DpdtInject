using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Transient.Generic.SingleInterfaceSingleObject
{
    [TestClass]
    public class ScopeTransientGenericSingleInterfaceSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeTransientGenericSingleInterfaceSingleObject_Cluster.ScopeTransientGenericSingleInterfaceSingleObject_ClusterTester),
                nameof(TestResources.ScopeTransientGenericSingleInterfaceSingleObject_Cluster),
                TestResources.ScopeTransientGenericSingleInterfaceSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
