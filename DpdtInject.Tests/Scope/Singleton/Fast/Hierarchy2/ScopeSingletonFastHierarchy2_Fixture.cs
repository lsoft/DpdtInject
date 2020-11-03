using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Scope.Singleton.Fast.Hierarchy2
{
    [TestClass]
    public class ScopeSingletonFastHierarchy2_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(ScopeSingletonFastHierarchy2_Cluster.ScopeSingletonFastHierarchy2_ClusterTester),
                nameof(TestResources.ScopeSingletonFastHierarchy2_Cluster),
                TestResources.ScopeSingletonFastHierarchy2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
