using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.PlainArray2
{
    [TestClass]
    public class UnsortedPlainArray2_Fixture
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
                nameof(UnsortedPlainArray2_Cluster.UnsortedPlainArray2_ClusterTester),
                nameof(TestResources.UnsortedPlainArray2_Cluster),
                TestResources.UnsortedPlainArray2_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
