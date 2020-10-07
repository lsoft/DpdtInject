using DpdtInject.Injector.Excp;
using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DpdtInject.Tests.Unsorted.SingletonTransient
{
    [TestClass] 
    public class UnsortedSingletonTransient_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedSingletonTransient_Cluster.UnsortedSingletonTransient_ClusterTester),
                nameof(TestResources.UnsortedSingletonTransient_Cluster),
                TestResources.UnsortedSingletonTransient_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
