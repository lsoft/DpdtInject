using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.GetAll.Generic.TwoObjectsConditional1
{
    [TestClass]
    public class GetAllGenericTwoObjectsConditional1_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GetAllGenericTwoObjectsConditional1_Cluster.GetAllGenericTwoObjectsConditional1_ClusterTester),
                nameof(TestResources.GetAllGenericTwoObjectsConditional1_Cluster),
                TestResources.GetAllGenericTwoObjectsConditional1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
