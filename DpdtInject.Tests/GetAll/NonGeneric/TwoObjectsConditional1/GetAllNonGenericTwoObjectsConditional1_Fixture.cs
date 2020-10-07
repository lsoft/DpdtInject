using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.GetAll.NonGeneric.TwoObjectsConditional1
{
    [TestClass]
    public class GetAllNonGenericTwoObjectsConditional1_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GetAllNonGenericTwoObjectsConditional1_Cluster.GetAllNonGenericTwoObjectsConditional1_ClusterTester),
                nameof(TestResources.GetAllNonGenericTwoObjectsConditional1_Cluster),
                TestResources.GetAllNonGenericTwoObjectsConditional1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
