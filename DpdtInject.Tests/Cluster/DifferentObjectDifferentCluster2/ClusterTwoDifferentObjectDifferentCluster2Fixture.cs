using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster2
{
    [TestClass]
    public class ClusterTwoDifferentObjectDifferentCluster2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterTwoDifferentObjectDifferentCluster2Module.ClusterTwoDifferentObjectDifferentCluster2ModuleTester),
                nameof(TestResources.ClusterTwoDifferentObjectDifferentCluster2Module),
                TestResources.ClusterTwoDifferentObjectDifferentCluster2Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
