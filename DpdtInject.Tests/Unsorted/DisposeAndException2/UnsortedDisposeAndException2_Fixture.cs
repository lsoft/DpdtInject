using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.DisposeAndException2
{
    [TestClass]
    public class UnsortedDisposeAndException2_Fixture
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
                nameof(UnsortedDisposeAndException2_Cluster.UnsortedDisposeAndException2_ClusterTester),
                nameof(TestResources.UnsortedDisposeAndException2_Cluster),
                TestResources.UnsortedDisposeAndException2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
