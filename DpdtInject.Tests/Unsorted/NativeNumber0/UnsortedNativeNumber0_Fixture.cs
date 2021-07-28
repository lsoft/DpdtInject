using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.NativeNumber0
{
    [TestClass]
    public class UnsortedNativeNumber0_Fixture
    {
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedNativeNumber0_Cluster.UnsortedNativeNumber0_ClusterTester),
                nameof(TestResources.UnsortedNativeNumber0_Cluster),
                TestResources.UnsortedNativeNumber0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
