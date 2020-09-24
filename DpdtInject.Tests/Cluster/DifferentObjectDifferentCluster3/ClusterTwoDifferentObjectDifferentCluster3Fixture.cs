using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster3
{
    [TestClass]
    public class ClusterTwoDifferentObjectDifferentCluster3Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterTwoDifferentObjectDifferentCluster3Module.ClusterTwoDifferentObjectDifferentCluster3ModuleTester),
                nameof(TestResources.ClusterTwoDifferentObjectDifferentCluster3Module),
                TestResources.ClusterTwoDifferentObjectDifferentCluster3Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
