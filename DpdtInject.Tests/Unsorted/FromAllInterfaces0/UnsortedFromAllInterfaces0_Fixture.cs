using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.FromAllInterfaces0
{
    [TestClass]
    public class UnsortedFromAllInterfaces0_Fixture
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
                nameof(UnsortedFromAllInterfaces0_Cluster.UnsortedFromAllInterfaces0_ClusterTester),
                nameof(TestResources.UnsortedFromAllInterfaces0_Cluster),
                TestResources.UnsortedFromAllInterfaces0_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
