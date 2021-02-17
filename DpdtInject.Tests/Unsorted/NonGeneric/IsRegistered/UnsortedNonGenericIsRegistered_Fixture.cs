using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.NonGeneric.IsRegistered
{
    [TestClass] 
    public class UnsortedNonGenericIsRegistered_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedNonGenericIsRegistered_Cluster.UnsortedNonGenericIsRegistered_ClusterTester),
                nameof(TestResources.UnsortedNonGenericIsRegistered_Cluster),
                TestResources.UnsortedNonGenericIsRegistered_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.ErrorCount, "Error count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
        }

    }
}
