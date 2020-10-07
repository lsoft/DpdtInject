using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Transient.Generic.Hierarchy2
{
    [TestClass]
    public class ScopeTransientGenericHierarchy2_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeTransientGenericHierarchy2_Cluster.ScopeTransientGenericHierarchy2_ClusterTester),
                nameof(TestResources.ScopeTransientGenericHierarchy2_Cluster),
                TestResources.ScopeTransientGenericHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
