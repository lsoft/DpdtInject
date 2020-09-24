using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster2
{
    [TestClass]
    public class ClusterDifferentObjectDifferentCluster2Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterDifferentObjectDifferentCluster2Module.ClusterDifferentObjectDifferentCluster2ModuleTester),
                nameof(TestResources.ClusterDifferentObjectDifferentCluster2Module),
                TestResources.ClusterDifferentObjectDifferentCluster2Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
