using DpdtInject.Tests.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Unsorted.BindFrom3And4
{
    [TestClass]
    public class UnsortedBindFrom3And4_Fixture
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
                nameof(UnsortedBindFrom3And4_Cluster.UnsortedBindFrom3And4_ClusterTester),
                nameof(TestResources.UnsortedBindFrom3And4_Cluster),
                TestResources.UnsortedBindFrom3And4_Cluster
                );

            preparation.Check();

            Assert.AreEqual(0, preparation.DiagnosticReporter.WarningCount, "Warning count");
            Assert.AreEqual(0, preparation.DiagnosticReporter.Exceptions.Count, "Error count");
        }
    }
}
