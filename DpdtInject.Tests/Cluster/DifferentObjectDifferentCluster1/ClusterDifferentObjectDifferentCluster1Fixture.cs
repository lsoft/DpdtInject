using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster1
{
    [TestClass]
    public class ClusterDifferentObjectDifferentCluster1Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterDifferentObjectDifferentCluster1Module.ClusterDifferentObjectDifferentCluster1ModuleTester),
                nameof(TestResources.ClusterDifferentObjectDifferentCluster1Module),
                TestResources.ClusterDifferentObjectDifferentCluster1Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
