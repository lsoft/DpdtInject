using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.MDArray2
{
    [TestClass]
    public class UnsortedMDArray2_Fixture
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
                nameof(UnsortedMDArray2_Cluster.UnsortedMDArray2_ClusterTester),
                nameof(TestResources.UnsortedMDArray2_Cluster),
                TestResources.UnsortedMDArray2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
