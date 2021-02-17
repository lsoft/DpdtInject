using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.SingletonCustom
{
    [TestClass] 
    public class UnsortedSingletonCustom_Fixture
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test()
        {
            var preparation = new Preparator(
                TestContext,
                nameof(UnsortedSingletonCustom_Cluster.UnsortedSingletonCustom_ClusterTester),
                nameof(TestResources.UnsortedSingletonCustom_Cluster),
                TestResources.UnsortedSingletonCustom_Cluster
                );

            preparation.Check();

            Assert.AreEqual(1, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }

    }
}
