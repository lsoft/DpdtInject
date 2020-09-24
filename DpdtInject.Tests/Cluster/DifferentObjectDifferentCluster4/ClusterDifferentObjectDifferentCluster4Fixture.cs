using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.DifferentObjectDifferentCluster4
{
    [TestClass]
    public class ClusterDifferentObjectDifferentCluster4Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterDifferentObjectDifferentCluster4Module.ClusterDifferentObjectDifferentCluster4ModuleTester),
                nameof(TestResources.ClusterDifferentObjectDifferentCluster4Module),
                TestResources.ClusterDifferentObjectDifferentCluster4Module
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
