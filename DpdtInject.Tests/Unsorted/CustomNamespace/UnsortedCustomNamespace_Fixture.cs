using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.CustomNamespace
{
    [TestClass] 
    public class UnsortedCustomNamespace_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedCustomNamespace_Cluster.UnsortedCustomNamespace_ClusterTester),
                nameof(TestResources.UnsortedCustomNamespace_Cluster),
                TestResources.UnsortedCustomNamespace_Cluster + TestResources.UnsortedCustomNamespace_FakeResource
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
