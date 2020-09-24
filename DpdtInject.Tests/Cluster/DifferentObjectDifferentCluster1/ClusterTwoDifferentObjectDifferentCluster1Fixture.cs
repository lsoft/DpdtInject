using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster1
{
    [TestClass]
    public class ClusterTwoDifferentObjectDifferentCluster1Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterTwoDifferentObjectDifferentCluster1Module.ClusterTwoDifferentObjectDifferentCluster1ModuleTester),
                nameof(TestResources.ClusterTwoDifferentObjectDifferentCluster1Module),
                TestResources.ClusterTwoDifferentObjectDifferentCluster1Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
