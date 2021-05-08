using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.MirroredArguments0
{
    [TestClass]
    public class UnsortedMirroredArguments0_Fixture
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
                nameof(UnsortedMirroredArguments0_Cluster.UnsortedMirroredArguments0_ClusterTester),
                nameof(TestResources.UnsortedMirroredArguments0_Cluster),
                TestResources.UnsortedMirroredArguments0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
