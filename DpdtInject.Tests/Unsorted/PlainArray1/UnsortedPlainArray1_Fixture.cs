using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.PlainArray1
{
    [TestClass]
    public class UnsortedPlainArray1_Fixture
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
                nameof(UnsortedPlainArray1_Cluster.UnsortedPlainArray1_ClusterTester),
                nameof(TestResources.UnsortedPlainArray1_Cluster),
                TestResources.UnsortedPlainArray1_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
