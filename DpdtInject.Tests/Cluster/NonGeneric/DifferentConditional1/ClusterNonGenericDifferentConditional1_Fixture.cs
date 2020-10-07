using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Cluster.NonGeneric.DifferentConditional1
{
    [TestClass]
    public class ClusterNonGenericDifferentConditional1_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ClusterNonGenericDifferentConditional1_ClusterTester),
                nameof(TestResources.ClusterNonGenericDifferentConditional1_Cluster),
                TestResources.ClusterNonGenericDifferentConditional1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
