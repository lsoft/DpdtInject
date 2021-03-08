using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.DisposeAndException
{
    [TestClass]
    public class UnsortedDisposeAndException_Fixture
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
                nameof(UnsortedDisposeAndException_Cluster.UnsortedDisposeAndException_ClusterTester),
                nameof(TestResources.UnsortedDisposeAndException_Cluster),
                TestResources.UnsortedDisposeAndException_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
