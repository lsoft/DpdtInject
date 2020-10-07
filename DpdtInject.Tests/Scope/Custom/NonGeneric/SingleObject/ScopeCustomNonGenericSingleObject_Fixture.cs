using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Custom.NonGeneric.SingleObject
{
    [TestClass]
    public class ScopeCustomNonGenericSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeCustomNonGenericSingleObject_Cluster.ScopeCustomNonGenericSingleObject_ClusterTester),
                nameof(TestResources.ScopeCustomNonGenericSingleObject_Cluster),
                TestResources.ScopeCustomNonGenericSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
