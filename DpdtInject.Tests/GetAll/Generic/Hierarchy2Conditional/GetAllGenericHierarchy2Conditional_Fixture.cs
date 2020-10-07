using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.GetAll.Generic.Hierarchy2Conditional
{
    [TestClass]
    public class GetAllGenericHierarchy2Conditional_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(GetAllGenericHierarchy2Conditional_Cluster.GetAllGenericHierarchy2Conditional_ClusterTester),
                nameof(TestResources.GetAllGenericHierarchy2Conditional_Cluster),
                TestResources.GetAllGenericHierarchy2Conditional_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
