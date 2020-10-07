using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Custom.Generic.Hierarchy2DifferentCluster
{
    [TestClass]
    public class ScopeCustomGenericHierarchy2DifferentCluster_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeCustomGenericHierarchy2DifferentCluster_ClusterTester),
                nameof(TestResources.ScopeCustomGenericHierarchy2DifferentCluster_Cluster),
                TestResources.ScopeCustomGenericHierarchy2DifferentCluster_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
