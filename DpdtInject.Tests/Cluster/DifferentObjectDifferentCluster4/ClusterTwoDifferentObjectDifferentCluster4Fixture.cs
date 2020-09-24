using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster4
{
    [TestClass]
    public class ClusterTwoDifferentObjectDifferentCluster4Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterTwoDifferentObjectDifferentCluster4Module.ClusterTwoDifferentObjectDifferentCluster4ModuleTester),
                nameof(TestResources.ClusterTwoDifferentObjectDifferentCluster4Module),
                TestResources.ClusterTwoDifferentObjectDifferentCluster4Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
