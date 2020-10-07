using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.Generic.IsRegistered
{
    [TestClass] 
    public class UnsortedGenericIsRegistered_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedGenericIsRegistered_Cluster.UnsortedGenericIsRegistered_ClusterTester),
                nameof(TestResources.UnsortedGenericIsRegistered_Cluster),
                TestResources.UnsortedGenericIsRegistered_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
