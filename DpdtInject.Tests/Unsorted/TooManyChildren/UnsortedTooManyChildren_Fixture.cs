using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.TooManyChildren
{
    [TestClass] 
    public class UnsortedTooManyChildren_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedTooManyChildren_Cluster.UnsortedTooManyChildren_ClusterTester),
                nameof(TestResources.UnsortedTooManyChildren_Cluster),
                TestResources.UnsortedTooManyChildren_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
