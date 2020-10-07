using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Transient.Generic.TwoInterfacesSingleObject
{
    [TestClass]
    public class ScopeTransientGenericTwoInterfacesSingleObject_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeTransientGenericTwoInterfacesSingleObject_Cluster.ScopeTransientGenericTwoInterfacesSingleObject_ClusterTester),
                nameof(TestResources.ScopeTransientGenericTwoInterfacesSingleObject_Cluster),
                TestResources.ScopeTransientGenericTwoInterfacesSingleObject_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
